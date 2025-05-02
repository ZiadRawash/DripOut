using AutoMapper;
using DripOut.Application.DTOs.Account;
using DripOut.Application.DTOs.Products;
using DripOut.Application.DTOs.Reviews;
using DripOut.Domain.Models;


namespace DripOut.Application.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile() // maps from Left into Right
        {
            CreateMap<Product, ProductDTO>(); 
            CreateMap<ProductInputDTO,Product>(); 
            CreateMap<VariantInputDTO,ProductVariant>(); 
            CreateMap<Review, ReviewDTO>().ForMember(rdto => rdto.User
            , opt => opt.MapFrom(u => u.User));
            CreateMap<AppUser, UserDTO>();
        }
    }
}
