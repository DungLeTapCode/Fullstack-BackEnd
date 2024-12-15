using API_BackEnd.Data;
using API_BackEnd.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API_BackEnd.Repository
{
    public class ProductsRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsRepository(AppDbContext context,IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> AddProductsAsyn(ProductsModel model, IFormFile? imageFile)
        {
            string? imagePath = null;

            // Xử lý upload ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                imagePath = $"/images/{fileName}";
            }

            // Tìm danh mục theo tên hoặc tạo mới nếu chưa tồn tại
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.CategoryName);
            if (category == null)
            {
                category = new Category { Name = model.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync(); // Lưu để lấy ID
            }

            // Ánh xạ sản phẩm
            var newProduct = new Products
            {
                Name = model.Name,
                Description = model.Description,
                DetailDesc = model.DetailDesc,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                ImagePath = imagePath,
                CategoryId = category.Id // Liên kết với danh mục
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return newProduct.Id;
        }




        public async Task DeleteProductsAsyn(int id)
        {
            // Tìm sản phẩm theo ID
            var deleteProducts = await _context.Products!.SingleOrDefaultAsync(prod => prod.Id == id);
            if (deleteProducts != null)
            {
                // Lấy đường dẫn ảnh từ sản phẩm
                var imagePath = deleteProducts.ImagePath;

                // Xóa sản phẩm khỏi cơ sở dữ liệu
                _context.Products!.Remove(deleteProducts);
                await _context.SaveChangesAsync();

                // Kiểm tra và xóa tệp ảnh nếu tồn tại
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
        }


        public async Task<List<ProductsModel>> GetAllProductsAsyn()
        {
            var products = await _context.Products
                .Include(p => p.Category) // Eager load Category
                .ToListAsync();

            // Ánh xạ sản phẩm và danh mục, thêm CategoryName
            var productModels = _mapper.Map<List<ProductsModel>>(products);


            return productModels;
        }



        public async Task<ProductsModel> GetProductsAsyn(int id)
        {
            var products = await _context.Products!.FindAsync(id);
            return  _mapper.Map<ProductsModel>(products);
        }

        public async Task<bool> UpdateProductAsync(int productId, ProductsModel model, IFormFile? imageFile)
        {
            // Tìm sản phẩm theo ID
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (existingProduct == null)
            {
                return false; // Không tìm thấy sản phẩm
            }

            string? imagePath = existingProduct.ImagePath;

            // Xử lý cập nhật ảnh nếu có
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.ImagePath.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                imagePath = $"/images/{fileName}";
            }

            // Tìm danh mục hoặc tạo mới nếu chưa tồn tại
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.CategoryName);
            if (category == null)
            {
                category = new Category { Name = model.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync(); // Lưu để lấy ID
            }

            // Cập nhật thông tin sản phẩm
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.DetailDesc = model.DetailDesc;
            existingProduct.Quantity = model.Quantity;
            existingProduct.UnitPrice = model.UnitPrice;
            existingProduct.ImagePath = imagePath;
            existingProduct.CategoryId = category.Id;

            // Lưu thay đổi
            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return true; // Cập nhật thành công
        }

    }
}
