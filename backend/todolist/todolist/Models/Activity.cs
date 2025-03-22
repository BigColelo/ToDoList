namespace todolist.Models;

public class Activity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public PriorityLevel Priority { get; set; }
    public ActivityStatus Status { get; set; } = ActivityStatus.ToDo;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}