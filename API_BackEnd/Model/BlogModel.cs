using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_BackEnd.Models
{
    public class BlogModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [BindNever]
        public int Id { get; set; }  // Id sẽ được gán tự động khi tạo mới blog.
        public string BlogName { get; set; }
        public string BlogDescription { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? DateTime { get; set; }
        public string author { get; set; }
    }
}
