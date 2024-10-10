using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; }
    }
}
