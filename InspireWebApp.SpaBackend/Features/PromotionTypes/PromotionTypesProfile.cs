using AutoMapper;
using JetBrains.Annotations;

namespace InspireWebApp.SpaBackend.Features.PromotionTypes;

[UsedImplicitly]
public class PromotionTypesProfile : Profile
{
    public PromotionTypesProfile()
    {
        CreateMap<PromotionType, PromotionTypeIdentifier>();

        CreateMap<PromotionTypesController.CreateModel, PromotionType>(MemberList.Source)
            ;

        CreateMap<PromotionTypesController.UpdateModel, PromotionType>(MemberList.Source)
            .ReverseMap()
            ;

        CreateMap<PromotionType, PromotionTypesController.ListModel>()
            ;

        CreateMap<PromotionType, PromotionTypeReferenceModel>()
            ;

        CreateMap<PromotionType, PromotionTypesController.DetailsModel>()
            ;
    }
}