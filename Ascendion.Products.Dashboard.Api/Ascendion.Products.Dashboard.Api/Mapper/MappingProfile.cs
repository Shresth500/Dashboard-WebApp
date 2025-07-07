using Ascendion.Products.Dashboard.DTO.Auth;
using Ascendion.Products.Dashboard.DTO.Product;
using Ascendion.Products.Dashboard.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Ascendion.Products.Dashboard.Mapper;

public class MappingProfile:Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequestDto, IdentityUser>();
        CreateMap<Product, ProductDto>();
    }
}
