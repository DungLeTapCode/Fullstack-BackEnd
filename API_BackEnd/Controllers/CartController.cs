using API_BackEnd.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly AppDbContext _context;

        public CartController(UserManager<ApplicationUsers> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("cart-by-username/{userName}")]
        public async Task<IActionResult> GetCartIdByUserName(string userName)
        {
            // Tìm userId từ userName
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            var userId = user.Id;

            // Tìm cartId từ userId
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Không tìm thấy giỏ hàng cho người dùng này.");
            }

            return Ok(new { CartId = cart.CartId });
        }
    }
}
