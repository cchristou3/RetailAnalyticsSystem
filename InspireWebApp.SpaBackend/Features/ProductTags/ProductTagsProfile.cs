using AutoMapper;
using JetBrains.Annotations;

namespace InspireWebApp.SpaBackend.Features.ProductTags;

[UsedImplicitly]
public class ProductTagsProfile : Profile
{
    public ProductTagsProfile()
    {
        CreateMap<ProductTag, ProductTagIdentifier>();

        CreateMap<ProductTagsController.CreateModel, ProductTag>(MemberList.Source)
            ;

        CreateMap<ProductTagsController.UpdateModel, ProductTag>(MemberList.Source)
            .ReverseMap()
            ;

        CreateMap<ProductTag, ProductTagsController.ListModel>()
            ;

        CreateMap<ProductTag, ProductTagReferenceModel>()
            ;

        CreateMap<ProductTag, ProductTagsController.DetailsModel>()
            ;
    }
}