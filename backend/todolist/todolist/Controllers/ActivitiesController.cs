using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using todolist.BL;
using todolist.DAO;
using todolist.DTO;
using todolist.Models;

namespace todolist.Controllers;

/// <summary>
/// Controller per la gestione delle attività
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize] // Aggiungi questo attributo
public class ActivitiesController : ControllerBase
{
    private readonly IActivitiesBL _activitiesBL;
    private readonly IActivitiesDAO _activitiesDAO;

    /// <summary>
    /// Costruttore del controller delle attività
    /// </summary>
    /// <param name="activitiesBL">Business logic per le attività</param>
    /// <param name="activitiesDAO">Data access object per le attività</param>
    public ActivitiesController(IActivitiesBL activitiesBL, IActivitiesDAO activitiesDAO)
    {
        _activitiesBL = activitiesBL;
        _activitiesDAO = activitiesDAO;
    }
    
    /// <summary>
    /// Ottiene tutte le attività
    /// </summary>
    /// <returns>Lista di tutte le attività</returns>
    /// <response code="200">Ritorna la lista delle attività</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivities()
    {
        var activities = await _activitiesBL.GetActivities();
        return Ok(activities); // Risposta OK con la lista delle attività
    }

    /// <summary>
    /// Ottiene un'attività specifica tramite il suo ID
    /// </summary>
    /// <param name="id">ID dell'attività da recuperare</param>
    /// <returns>L'attività richiesta</returns>
    /// <response code="200">Ritorna l'attività richiesta</response>
    /// <response code="404">Se l'attività non esiste</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActivityById(int id)
    {
        var activity = await _activitiesBL.GetActivityById(id);
        if (activity == null)
            return NotFound(); // Se l'attività non esiste, ritorna 404
        return Ok(activity); // Risposta OK con l'attività
    }

    /// <summary>
    /// Ottiene tutte le attività di un utente specifico
    /// </summary>
    /// <param name="userId">ID dell'utente</param>
    /// <returns>Lista delle attività dell'utente</returns>
    /// <response code="200">Ritorna la lista delle attività dell'utente</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Activity>>> GetActivitiesByUser(int userId)
    {
        // Opzionalmente, puoi verificare che l'utente corrente abbia il permesso di vedere queste attività
        var activities = await _activitiesBL.GetActivitiesByUserId(userId);
        return Ok(activities);
    }

    /// <summary>
    /// Crea una nuova attività
    /// </summary>
    /// <param name="activity">Dati dell'attività da creare</param>
    /// <returns>L'attività creata</returns>
    /// <response code="201">Ritorna l'attività creata</response>
    /// <response code="400">Se i dati dell'attività non sono validi</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
    {
        if (activity == null)
            return BadRequest("Invalid data."); // Verifica se i dati sono validi
    
        await _activitiesBL.AddActivity(activity);
        return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
    }

    /// <summary>
    /// Aggiorna un'attività esistente
    /// </summary>
    /// <param name="id">ID dell'attività da aggiornare</param>
    /// <param name="activity">Nuovi dati dell'attività</param>
    /// <returns>Nessun contenuto</returns>
    /// <response code="204">Se l'aggiornamento è avvenuto con successo</response>
    /// <response code="400">Se i dati dell'attività non sono validi</response>
    /// <response code="404">Se l'attività non esiste</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateActivity(int id, [FromBody] Activity activity)
    {
        if (activity == null)
            return BadRequest("Invalid data.");
                
        var existingActivity = await _activitiesBL.GetActivityById(id);
        if (existingActivity == null)
            return NotFound(); // Se l'attività non esiste, ritorna 404
        
        // Aggiorna le proprietà dell'attività esistente
        activity.Id = id;
        await _activitiesBL.UpdateActivity(activity);
        return NoContent(); // Risposta 204 NoContent quando l'aggiornamento è avvenuto con successo
    }

    /// <summary>
    /// Marca un'attività come completata
    /// </summary>
    /// <param name="id">ID dell'attività da marcare come completata</param>
    /// <returns>Nessun contenuto</returns>
    /// <response code="204">Se l'operazione è avvenuta con successo</response>
    /// <response code="404">Se l'attività non esiste</response>
    [HttpPut("{id}/mark-as-done")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkActivityAsDone(int id)
    {
        var activity = await _activitiesBL.GetActivityById(id);
        if (activity == null)
            return NotFound(); // Se l'attività non esiste, ritorna 404

        await _activitiesBL.MarkActivityAsDoneAsync(id);
        return NoContent(); // Risposta 204 NoContent quando l'operazione è completata con successo
    }

    /// <summary>
    /// Elimina un'attività
    /// </summary>
    /// <param name="id">ID dell'attività da eliminare</param>
    /// <returns>Nessun contenuto</returns>
    /// <response code="204">Se l'eliminazione è avvenuta con successo</response>
    /// <response code="404">Se l'attività non esiste</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        var activity = await _activitiesBL.GetActivityById(id);
        if (activity == null)
            return NotFound(); // Se l'attività non esiste, ritorna 404
    
        await _activitiesBL.DeleteActivity(id);
        return NoContent(); // Risposta 204 NoContent quando l'eliminazione è avvenuta con successo
    }
}