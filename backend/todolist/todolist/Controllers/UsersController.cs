using Microsoft.AspNetCore.Mvc;
using todolist.BL;
using todolist.Models;
using Microsoft.AspNetCore.Authorization;

namespace todolist.Controllers;

/// <summary>
/// Controller per la gestione degli utenti
/// </summary>
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserBL _userBL;

    /// <summary>
    /// Costruttore del controller degli utenti
    /// </summary>
    /// <param name="userBL">Business logic per gli utenti</param>
    public UsersController(IUserBL userBL)
    {
        _userBL = userBL;
    }

    /// <summary>
    /// Ottiene tutti gli utenti
    /// </summary>
    /// <returns>Lista di tutti gli utenti</returns>
    /// <response code="200">Ritorna la lista degli utenti</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userBL.GetUsers();
        return Ok(users);
    }

    /// <summary>
    /// Ottiene un utente specifico tramite il suo ID
    /// </summary>
    /// <param name="id">ID dell'utente da recuperare</param>
    /// <returns>L'utente richiesto</returns>
    /// <response code="200">Ritorna l'utente richiesto</response>
    /// <response code="404">Se l'utente non esiste</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userBL.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Crea un nuovo utente
    /// </summary>
    /// <param name="user">Dati dell'utente da creare</param>
    /// <returns>L'utente creato</returns>
    /// <response code="201">Ritorna l'utente creato</response>
    /// <response code="400">Se i dati dell'utente non sono validi o l'username è già in uso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        try
        {
            await _userBL.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Aggiorna un utente esistente
    /// </summary>
    /// <param name="id">ID dell'utente da aggiornare</param>
    /// <param name="user">Nuovi dati dell'utente</param>
    /// <returns>Nessun contenuto</returns>
    /// <response code="204">Se l'aggiornamento è avvenuto con successo</response>
    /// <response code="400">Se i dati dell'utente non sono validi</response>
    /// <response code="404">Se l'utente non esiste</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        try
        {
            await _userBL.UpdateUser(user);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un utente
    /// </summary>
    /// <param name="id">ID dell'utente da eliminare</param>
    /// <returns>Nessun contenuto</returns>
    /// <response code="204">Se l'eliminazione è avvenuta con successo</response>
    /// <response code="404">Se l'utente non esiste</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userBL.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        
        await _userBL.DeleteUser(id);
        return NoContent();
    }

    /// <summary>
    /// Effettua il login di un utente
    /// </summary>
    /// <param name="model">Credenziali di login (username e password)</param>
    /// <returns>Dati dell'utente autenticato</returns>
    /// <response code="200">Se il login è avvenuto con successo</response>
    /// <response code="401">Se le credenziali non sono valide</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<User>> Login([FromBody] LoginModel model)
    {
        var user = await _userBL.Login(model.Username, model.Password);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(user);
    }
    
    /// <summary>
    /// Registra un nuovo utente
    /// </summary>
    /// <param name="model">Dati di registrazione</param>
    /// <returns>L'utente registrato</returns>
    /// <response code="201">Se la registrazione è avvenuta con successo</response>
    /// <response code="400">Se i dati di registrazione non sono validi o l'utente esiste già</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> Register([FromBody] RegisterModel model)
    {
        try
        {
            var user = await _userBL.RegisterUser(model);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Modello per il login
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Nome utente
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
}