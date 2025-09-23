namespace HmctsBackend.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "To do";
        public required DateTime DueDate { get; set; }
    }
}