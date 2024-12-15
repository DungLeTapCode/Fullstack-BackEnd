using API_BackEnd.Data;

namespace API_BackEnd.Model
{
    public class CartItemModel
    {
        public int CartItemId { get; set; } 
        public int CartId { get; set; }
        public int ProductId { get; set; } 
        public int Quantity { get; set; }
        public string ProductName { get; set; } // Tên sản phẩm
        public double ProductPrice { get; set; } // Giá sản phẩm
        public string? ImagePath { get; set; }

    }
}
