using todolist.DAO;
using todolist.Models;

namespace todolist.BL;

public class UserBL : IUserBL
{
    private readonly IUserDAO _userDAO;

    public UserBL(IUserDAO userDAO)
    {
        _userDAO = userDAO;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _userDAO.GetUserById(id);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _userDAO.GetUsers();
    }

    public async Task AddUser(User user)
    {
        // Verifica se l'username è già in uso
        var existingUser = await _userDAO.GetUserByUsername(user.Username);
        if (existingUser != null)
        {
            throw new Exception("Username already exists");
        }
        
        await _userDAO.AddUser(user);
    }

    public async Task UpdateUser(User user)
    {
        // Verifica se l'utente esiste
        var existingUser = await _userDAO.GetUserById(user.Id);
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }
        
        await _userDAO.UpdateUser(user);
    }

    public async Task DeleteUser(int id)
    {
        await _userDAO.DeleteUser(id);
    }

    public async Task<User> Login(string username, string password)
    {
        return await _userDAO.ValidateUser(username, password);
    }

    public async Task<User> RegisterUser(RegisterModel registerModel)
    {
        // Verifica se l'utente esiste già
        var existingUser = await _userDAO.GetUserByUsername(registerModel.Username);
        if (existingUser != null)
        {
            throw new Exception("Username già in uso");
        }
        
        // Verifica se l'email esiste già
        var existingEmail = await _userDAO.GetUserByEmail(registerModel.Email);
        if (existingEmail != null)
        {
            throw new Exception("Email già in uso");
        }
        
        // Registra l'utente
        return await _userDAO.RegisterUser(registerModel);
    }
}