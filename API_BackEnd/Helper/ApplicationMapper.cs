using API_BackEnd.Data;
using API_BackEnd.Model;
using API_BackEnd.Models;
using AutoMapper;

namespace API_BackEnd.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() {
            CreateMap<Products, ProductsModel>();
            CreateMap<ProductsModel, Products>();
            CreateMap<Cart, CartModel>().ReverseMap();
            CreateMap<CartItem, CartItemModel>().ReverseMap();
            CreateMap<Blog, BlogModel>().ReverseMap();
        }
    }
}
