using API_BackEnd.Data;
using API_BackEnd.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API_BackEnd.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CartItemRepository(AppDbContext context, IMapper mapper) { 
            _context = context;
            _mapper = mapper;
        }
        public async Task<CartItemModel> AddItemToCartAsync(CartItemModel model)
        {
            // Kiểm tra xem giỏ hàng có tồn tại không
            var cartExists = await _context.Carts.AnyAsync(c => c.CartId == model.CartId);

            // Nếu giỏ hàng không tồn tại, tạo giỏ hàng mới
            if (!cartExists)
            {
                var cart = new Cart
                {
                    CartId = model.CartId
                    // Bạn có thể thêm các thuộc tính khác cho Cart nếu cần
                };

                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem sản phẩm trong giỏ hàng đã tồn tại chưa
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == model.CartId && ci.ProductId == model.ProductId);

            if (existingItem != null)
            {
                // Nếu sản phẩm đã có trong giỏ, cập nhật số lượng
                existingItem.Quantity += model.Quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ, tạo mới
                var cartItem = new CartItem
                {
                    CartId = model.CartId,
                    ProductId = model.ProductId,
                    Quantity = model.Quantity
                };

                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            // Sau khi thêm hoặc cập nhật sản phẩm, lấy lại thông tin sản phẩm từ CartItems
            var updatedItem = await _context.CartItems
                .Where(ci => ci.CartId == model.CartId && ci.ProductId == model.ProductId)
                .Include(ci => ci.Product)  // Bao gồm thông tin sản phẩm
                .FirstOrDefaultAsync();

            // Chuyển từ CartItem entity sang CartItemModel và trả về thông tin giỏ hàng cập nhật
            return new CartItemModel
            {
                CartItemId = updatedItem.CartItemId,
                CartId = updatedItem.CartId,
                ProductId = updatedItem.ProductId,
                Quantity = updatedItem.Quantity,
                ProductName = updatedItem.Product.Name, // Lấy tên sản phẩm từ Product
                ProductPrice = updatedItem.Product.UnitPrice, // Lấy giá sản phẩm từ Product
                ImagePath = updatedItem.Product.ImagePath // Lấy ảnh sản phẩm từ Product
            };
        }


        // Lấy mục giỏ hàng theo ID
        public async Task<CartItemModel> GetCartItemByIdAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)  // Nếu cần thông tin sản phẩm đi kèm
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            // Chuyển từ CartItem entity sang CartItemModel
            return _mapper.Map<CartItemModel>(cartItem);
        }


        public async Task<List<CartItemModel>> GetCartItemsByCartIdAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product) // Bao gồm thông tin sản phẩm
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            // Ánh xạ từ CartItem entity sang CartItemModel và lấy thông tin sản phẩm
            var cartItemModels = cartItems.Select(ci => new CartItemModel
            {
                CartItemId = ci.CartItemId,
                CartId = ci.CartId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,

                // Lấy thông tin từ Product
                ProductName = ci.Product.Name, // Tên sản phẩm
                ProductPrice = ci.Product.UnitPrice, // Giá sản phẩm
                ImagePath = ci.Product.ImagePath // Ảnh sản phẩm
            }).ToList();

            return cartItemModels;
        }


        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
            }
        }

    
    }
}
