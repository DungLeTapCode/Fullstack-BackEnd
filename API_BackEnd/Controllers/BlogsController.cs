using API_BackEnd.Models;
using API_BackEnd.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;

        public BlogsController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        [HttpGet("GetAllBlogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            try
            {
                var blogs = await _blogRepository.GetAllBlogsAsync();

                // Nếu không có blog nào, trả về mảng rỗng
                if (blogs == null || !blogs.Any())
                {
                    return Ok(new List<BlogModel>());
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                // Thêm base URL vào ImagePath cho mỗi blog
                foreach (var blog in blogs)
                {
                    if (!string.IsNullOrEmpty(blog.ImagePath))
                    {
                        blog.ImagePath = $"{baseUrl}{blog.ImagePath}";
                    }
                }

                return Ok(blogs);
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi chi tiết khi có lỗi
                return BadRequest(new { message = "An error occurred while retrieving blogs", error = ex.Message });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _blogRepository.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            if (!string.IsNullOrEmpty(blog.ImagePath))
            {
                blog.ImagePath = $"{baseUrl}{blog.ImagePath}";
            }

            return Ok(blog);
        }

        [HttpPost("AddNewBlog")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBlog([FromForm] BlogModel model, IFormFile? imageFile)
        {
            var blog = await _blogRepository.AddBlogAsync(model, imageFile);
            return Ok(blog);
        }

        [HttpPut("UpdateBlog/{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromForm] BlogModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedBlog = await _blogRepository.UpdateBlogAsync(id, model, imageFile);
                if (updatedBlog == null)
                {
                    return NotFound(new { message = "Blog not found" });
                }

                return Ok(new { message = "Blog updated successfully", updatedBlog });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the blog", error = ex.Message });
            }
        }

        [HttpDelete("DeleteBlog/{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var isDeleted = await _blogRepository.DeleteBlogAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = "Blog not found" });
            }

            return Ok(new { message = "Blog deleted successfully" });
        }
    }

}
