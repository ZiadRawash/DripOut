using AutoMapper;
using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Account;
using DripOut.Domain.Models;


namespace DripOut.Application.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductsDTO>(); // maps from Left into Right
            CreateMap<Review, ReviewDTO>().ForMember(rdto => rdto.User
            , opt => opt.MapFrom(u => u.User));
            CreateMap<AppUser, UserDTO>();
        }
    }
}
