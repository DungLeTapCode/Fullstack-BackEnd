using API_BackEnd.Helper;
using API_BackEnd.Model;
using API_BackEnd.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                // Lấy danh sách sản phẩm từ repository
                var products = await _productRepository.GetAllProductsAsyn();

                // Lấy thông tin base URL từ Request
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                // Cập nhật imagePath với base URL
                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.ImagePath))
                    {
                        product.ImagePath = $"{baseUrl}{product.ImagePath}";
                    }
                }

                // Trả về danh sách sản phẩm
                return Ok(products);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetProductsById(int id)
        {
            var products = await _productRepository.GetProductsAsyn(id);
            return products == null ? NotFound(): Ok(products);
        }

        [HttpPost("AddNewProduct")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] ProductsModel model, IFormFile? imageFile)
        {
            var productId = await _productRepository.AddProductsAsyn(model, imageFile);
            return Ok(new { ProductId = productId });
        }

        [HttpPut("UpdateProducts/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductsModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            try
            {
                var isUpdated = await _productRepository.UpdateProductAsync(id, model, imageFile);
                if (!isUpdated)
                {
                    return NotFound(new { message = "Product not found" }); // Không tìm thấy sản phẩm
                }

                return Ok(new { message = "Product updated successfully" }); // Cập nhật thành công
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần (VD: _logger.LogError(ex, "Error updating product"));
                return StatusCode(500, new { message = "An error occurred while updating the product", error = ex.Message });
            }
        }


        [HttpDelete("DeleteProducts/{id}")]
       
        public async Task<IActionResult> DeleteProductsAsync(int id)
        {
            await _productRepository.DeleteProductsAsyn(id);
            return Ok();
        }

    }
}
