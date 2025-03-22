using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using todolist.Models;
using todolist.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace todolist.DAO;

public class UserDAO : IUserDAO
{
    private readonly ToDoContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public UserDAO(ToDoContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddUser(User user)
    {
        // Hash della password prima di salvarla
        user.Password = HashPassword(user.Password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        // Se la password è stata modificata, hash della nuova password
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser != null && existingUser.Password != user.Password)
        {
            user.Password = HashPassword(user.Password);
        }
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User> ValidateUser(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null && VerifyPassword(password, user.Password))
        {
            return user;
        }
        return null;
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    private bool VerifyPassword(string inputPassword, string storedHash)
    {
        string inputHash = HashPassword(inputPassword);
        return inputHash == storedHash;
    }

    public async Task<User> RegisterUser(RegisterModel registerModel)
    {
        // 1. Registra l'utente in Keycloak
        var keycloakUser = await RegisterUserInKeycloak(registerModel);
        
        // 2. Crea l'utente nel database locale
        var user = new User
        {
            Username = registerModel.Username,
            Email = registerModel.Email,
            FirstName = registerModel.FirstName,
            LastName = registerModel.LastName,
            // Nota: non salviamo la password nel nostro database
            // poiché l'autenticazione è gestita da Keycloak
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    private async Task<object> RegisterUserInKeycloak(RegisterModel registerModel)
    {
        // Ottieni un token di accesso per l'admin
        var adminToken = await GetAdminToken();
        
        // Prepara i dati dell'utente per Keycloak
        var keycloakUser = new
        {
            username = registerModel.Username,
            email = registerModel.Email,
            enabled = true,
            firstName = registerModel.FirstName,
            lastName = registerModel.LastName,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = registerModel.Password,
                    temporary = false
                }
            }
        };
        
        // Converti in JSON
        var content = new StringContent(
            JsonSerializer.Serialize(keycloakUser),
            Encoding.UTF8,
            "application/json");
        
        // Imposta l'header di autorizzazione
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", adminToken);
        
        // URL per creare un utente nel realm todolist
        var url = $"{_configuration["Keycloak:AdminUrl"]}/admin/realms/todolist/users";
        
        // Invia la richiesta
        var response = await _httpClient.PostAsync(url, content);
        
        // Verifica se la richiesta è andata a buon fine
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Errore durante la registrazione in Keycloak: {errorContent}");
        }
        
        return keycloakUser;
    }

    private async Task<string> GetAdminToken()
    {
        var tokenRequest = new
        {
            client_id = "admin-cli",
            username = _configuration["Keycloak:AdminUsername"],
            password = _configuration["Keycloak:AdminPassword"],
            grant_type = "password"
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(tokenRequest),
            Encoding.UTF8,
            "application/json");
        
        var response = await _httpClient.PostAsync(
            $"{_configuration["Keycloak:TokenUrl"]}/realms/master/protocol/openid-connect/token", 
            content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Errore durante l'ottenimento del token admin: {errorContent}");
        }
        
        var tokenResponse = await JsonSerializer.DeserializeAsync<JsonElement>(
            await response.Content.ReadAsStreamAsync());
        
        return tokenResponse.GetProperty("access_token").GetString();
    }
}