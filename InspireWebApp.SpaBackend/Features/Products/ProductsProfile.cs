using AutoMapper;
using JetBrains.Annotations;

namespace InspireWebApp.SpaBackend.Features.Products;

[UsedImplicitly]
public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        CreateMap<Product, ProductIdentifier>();

        CreateMap<ProductsController.CreateModel, Product>(MemberList.Source)
            .ForMember(entity => entity.PromotionTypes, opts => opts.Ignore()) // AM can't conjure real entities
            ;

        CreateMap<ProductsController.UpdateModel, Product>(MemberList.Source)
            .ForMember(entity => entity.PromotionTypes, opts => opts.Ignore()) // AM can't conjure real entities
            .ReverseMap()
            ;

        CreateMap<Product, ProductsController.ListModel>()
            ;

        CreateMap<Product, ProductReferenceModel>()
            ;

        CreateMap<Product, ProductsController.DetailsModel>()
            ;
    }
}