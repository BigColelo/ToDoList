using todolist.Models;

namespace todolist.BL;

public interface IUserBL
{
    Task<User> GetUserById(int id);
    Task<IEnumerable<User>> GetUsers();
    Task AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(int id);
    Task<User> Login(string username, string password);
    
    /// <summary>
    /// Registra un nuovo utente
    /// </summary>
    /// <param name="registerModel">Dati di registrazione</param>
    /// <returns>L'utente creato</returns>
    Task<User> RegisterUser(RegisterModel registerModel);
}