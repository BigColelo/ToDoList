using todolist.Models;

namespace todolist.DAO;

public interface IUserDAO
{
    Task<User> GetUserById(int id);
    
    /// <summary>
    /// Ottiene un utente tramite username
    /// </summary>
    /// <param name="username">Username dell'utente</param>
    /// <returns>L'utente, se esiste</returns>
    Task<User> GetUserByUsername(string username);
    
    /// <summary>
    /// Ottiene un utente tramite email
    /// </summary>
    /// <param name="email">Email dell'utente</param>
    /// <returns>L'utente, se esiste</returns>
    Task<User> GetUserByEmail(string email);
    Task<IEnumerable<User>> GetUsers();
    Task AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(int id);
    Task<User> ValidateUser(string username, string password);
    
    /// <summary>
    /// Registra un nuovo utente
    /// </summary>
    /// <param name="registerModel">Dati di registrazione</param>
    /// <returns>L'utente creato</returns>
    Task<User> RegisterUser(RegisterModel registerModel);
}