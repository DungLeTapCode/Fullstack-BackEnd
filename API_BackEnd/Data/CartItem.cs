namespace API_BackEnd.Data
{

    public class CartItem
    {
        public int CartItemId { get; set; } // ID của mục giỏ hàng
        public int CartId { get; set; } // Liên kết tới Cart

        public int ProductId { get; set; } // Liên kết tới Product (khóa ngoại)
        public Products Product { get; set; } // Liên kết thực thể với Product

        public int Quantity { get; set; } // Số lượng sản phẩm
        
        
    }
}
