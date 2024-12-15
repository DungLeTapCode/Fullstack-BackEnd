using System.ComponentModel.DataAnnotations;

namespace API_BackEnd.Data
{
    public class Blog
    {
        [Key]

        public int Id { get; set; }
        public string blogName { get; set; }
        public string? blogDescription { get; set; }
        public string? ImagePath { get; set; }
        public DateTime DateTime { get; set; }
         public string author { get; set; }






    }
}
