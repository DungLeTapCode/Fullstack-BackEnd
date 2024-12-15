using API_BackEnd.Model;
using API_BackEnd.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemController(ICartItemRepository cartItemRepository) {
            _cartItemRepository = cartItemRepository;
        }

        [HttpGet("GetCarts/{CartId}")]
        public async Task<IActionResult> GetAllCartItem(int CartId)
        {
            var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(CartId);


            // Lấy thông tin base URL từ Request
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Cập nhật imagePath với base URL cho từng sản phẩm trong cartItem
            foreach (var cartItem in cartItems)
            {
                if (!string.IsNullOrEmpty(cartItem.ImagePath))
                {
                    // Cập nhật imagePath đầy đủ cho mỗi sản phẩm
                    cartItem.ImagePath = $"{baseUrl}{cartItem.ImagePath}";
                }
            }

            // Trả về danh sách mục giỏ hàng
            return Ok(cartItems);
        }

        [HttpPost("AddCarts")]

        public async Task<IActionResult> AddNewItem (CartItemModel model)
        {
            var cartItem = await _cartItemRepository.AddItemToCartAsync(model);
            return Ok(cartItem);
        }

        [HttpDelete("RemoveCarts/{id}")]

        public async Task<IActionResult> DeleteCartItem (int id)
        {
           await _cartItemRepository.RemoveCartItemAsync(id);
            return Ok();
        }

        [HttpPut("UpdateCarts/{id}")]
        public async Task<IActionResult> UpdateItem (int id,int quantity)
        {
            await _cartItemRepository.UpdateCartItemAsync(id,quantity);
            return Ok();
        }
    }
}
