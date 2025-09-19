namespace HmctsBackend.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "To do";
        public DateTime? DueDate { get; set; }
    }
}