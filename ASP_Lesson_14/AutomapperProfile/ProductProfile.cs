using ASP_Lesson_14.Models;
using ASP_Lesson_14.Models.DTO.Product;
using AutoMapper;

namespace ASP_Lesson_14.AutomapperProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
