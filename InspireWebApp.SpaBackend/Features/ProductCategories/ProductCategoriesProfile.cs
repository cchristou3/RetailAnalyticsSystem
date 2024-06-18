using AutoMapper;
using JetBrains.Annotations;

namespace InspireWebApp.SpaBackend.Features.ProductCategories;

[UsedImplicitly]
public class ProductCategoriesProfile : Profile
{
    public ProductCategoriesProfile()
    {
        CreateMap<ProductCategory, ProductCategoryIdentifier>();

        CreateMap<ProductCategoriesController.CreateModel, ProductCategory>(MemberList.Source)
            ;

        CreateMap<ProductCategoriesController.UpdateModel, ProductCategory>(MemberList.Source)
            .ReverseMap()
            ;

        CreateMap<ProductCategory, ProductCategoriesController.ListModel>()
            ;

        CreateMap<ProductCategory, ProductCategoryReferenceModel>()
            ;

        CreateMap<ProductCategory, ProductCategoriesController.DetailsModel>()
            ;
    }
}