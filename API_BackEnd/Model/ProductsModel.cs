using API_BackEnd.Data;
using System.ComponentModel.DataAnnotations;

namespace API_BackEnd.Model
{
    public class ProductsModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DetailDesc { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string? ImagePath { get; set; }
        public string CategoryName { get; set; }
    }
}
