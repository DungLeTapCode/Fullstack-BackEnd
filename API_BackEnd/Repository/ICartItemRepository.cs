using API_BackEnd.Data;
using API_BackEnd.Model;

namespace API_BackEnd.Repository
{
    public interface ICartItemRepository
    {
        public Task<CartItemModel> AddItemToCartAsync(CartItemModel model); // Thêm mục vào giỏ hàng
        public Task<List<CartItemModel>> GetCartItemsByCartIdAsync(int cartId); // Lấy tất cả các mục trong giỏ hàng
        public Task UpdateCartItemAsync(int cartItemId, int quantity); // Cập nhật số lượng của mục giỏ hàng
        public Task RemoveCartItemAsync(int cartItemId); // Xóa mục khỏi giỏ hàng
    }
}
