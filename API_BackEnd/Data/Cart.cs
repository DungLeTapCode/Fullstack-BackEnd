using Microsoft.AspNetCore.Identity;

namespace API_BackEnd.Data
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; } 
        public ApplicationUsers User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    }
}
