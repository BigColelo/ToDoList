using todolist.Models;

namespace todolist.DTO;

public class ActivityCreateDTO
{
    public string Title { get; set; } // Titolo dell'attività
    public string? Description { get; set; } // Descrizione opzionale
    public PriorityLevel Priority { get; set; } // Priorità dell'attività (definita da un enum)
}