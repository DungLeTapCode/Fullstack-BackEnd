using API_BackEnd.Data;
using API_BackEnd.Model;

namespace API_BackEnd.Repository
{
    public interface IProductRepository
    {
        public Task<List<ProductsModel>> GetAllProductsAsyn();
        public Task<ProductsModel> GetProductsAsyn(int id);
        Task<int> AddProductsAsyn(ProductsModel model, IFormFile? imageFile);
        public Task<bool> UpdateProductAsync(int id, ProductsModel model, IFormFile? imageFile);
        public Task DeleteProductsAsyn(int id);
    }
}
