using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models
{
    public class TaskItem
    {
        public ObjectId Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; }
    }
}
