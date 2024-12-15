using API_BackEnd.Data;

namespace API_BackEnd.Model
{
    public class CartModel
    {
        public int CartId { get; set; } // ID của giỏ hàng
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>(); // Các mục giỏ hàng
       
    }
}
