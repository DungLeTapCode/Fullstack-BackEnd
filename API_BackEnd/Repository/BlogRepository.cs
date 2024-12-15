using API_BackEnd.Data;
using API_BackEnd.Model;
using API_BackEnd.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API_BackEnd.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BlogRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Thêm Blog mới
        public async Task<int> AddBlogAsync(BlogModel model, IFormFile? imageFile)
        {
            string? imagePath = null;

            // Xử lý upload ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagesBlogs");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                imagePath = $"/imagesBlogs/{fileName}";
            }

            // Tạo mới Blog
            var newBlog = new Blog
            {
                blogName = model.BlogName,
                blogDescription = model.BlogDescription,
                ImagePath = imagePath,
                DateTime = DateTime.UtcNow,
                author = model.author
            };

            _context.Blogs.Add(newBlog);
            await _context.SaveChangesAsync();

            return newBlog.Id;
        }

        // Xóa Blog
        public async Task<bool> DeleteBlogAsync(int id)
        {
            // Tìm Blog theo ID
            var deleteBlog = await _context.Blogs.SingleOrDefaultAsync(b => b.Id == id);
            if (deleteBlog == null)
            {
                return false; // Trả về thông báo nếu không tìm thấy blog
            }

            // Lấy đường dẫn ảnh từ Blog
            var imagePath = deleteBlog.ImagePath;

            // Xóa Blog khỏi cơ sở dữ liệu
            _context.Blogs.Remove(deleteBlog);
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

            return true; // Trả về thông báo thành công
        }


        // Lấy tất cả Blog
        public async Task<List<BlogModel>> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs.ToListAsync();

            // Ánh xạ Blog và trả về danh sách BlogModel
            var blogModels = _mapper.Map<List<BlogModel>>(blogs);

            return blogModels;
        }

        // Lấy thông tin Blog theo ID
        public async Task<BlogModel> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            return _mapper.Map<BlogModel>(blog);
        }

        // Cập nhật Blog
        public async Task<bool> UpdateBlogAsync(int blogId, BlogModel model, IFormFile? imageFile)
        {
            // Tìm Blog theo ID
            var existingBlog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blogId);
            if (existingBlog == null)
            {
                return false; // Không tìm thấy Blog
            }

            string? imagePath = existingBlog.ImagePath;

            // Xử lý cập nhật ảnh nếu có
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagesBlogs");

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
                if (!string.IsNullOrEmpty(existingBlog.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingBlog.ImagePath.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                imagePath = $"/imagesBlogs/{fileName}";
            }

            // Cập nhật thông tin Blog
            existingBlog.blogName = model.BlogName;
            existingBlog.blogDescription = model.BlogDescription;
            existingBlog.ImagePath = imagePath;
            existingBlog.DateTime = DateTime.UtcNow;
            existingBlog.author = model.author;

            // Lưu thay đổi
            _context.Blogs.Update(existingBlog);
            await _context.SaveChangesAsync();

            return true; // Cập nhật thành công
        }
    }
}
