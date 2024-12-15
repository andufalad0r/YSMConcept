using YSMConcept.Application.DTOs.ImageDTOs;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Mappers
{
    public static class ImageMapper
    {
        public static ImageDTO ToImageDTOFromImageEntity(this ImageEntity imageEntity)
        {
            return new ImageDTO
            {
                ImageId = imageEntity.ImageId,
                ImageURL = imageEntity.ImageURL
            };
        }
    }
}
