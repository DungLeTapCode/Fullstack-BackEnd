using API_BackEnd.Models;
using System.Threading.Tasks;

namespace API_BackEnd.Repository
{
    public interface IBlogRepository 
    {
        Task<int> AddBlogAsync(BlogModel model, IFormFile? imageFile);  // Thêm Blog mới
        Task<bool> DeleteBlogAsync(int id);  // Xóa Blog
        Task<List<BlogModel>> GetAllBlogsAsync();  // Lấy tất cả Blog
        Task<BlogModel> GetBlogByIdAsync(int id);  // Lấy Blog theo ID
        Task<bool> UpdateBlogAsync(int blogId, BlogModel model, IFormFile? imageFile);  // Cập nhật Blog

    }
}
