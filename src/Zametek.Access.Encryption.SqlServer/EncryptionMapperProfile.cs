using AutoMapper;
using Zametek.Utility;

namespace Zametek.Access.Encryption
{
    public class EncryptionMapperProfile
        : Profile
    {
        public EncryptionMapperProfile()
        {
            CreateMap<RegisterSymmetricKeyRequest, SymmetricKey>()
                .ForMember(dest => dest.WrappedSymmetricKey, opt => opt.MapFrom(src => src.WrappedSymmetricKey.ByteArrayToBase64String()))
                .ForMember(dest => dest.InitializationVector, opt => opt.MapFrom(src => src.InitializationVector.ByteArrayToBase64String()));

            CreateMap<UpdateSymmetricKeyRequest, SymmetricKey>()
                .ForMember(dest => dest.WrappedSymmetricKey, opt => opt.MapFrom(src => src.WrappedSymmetricKey.ByteArrayToBase64String()))
                .ForMember(dest => dest.InitializationVector, opt => opt.MapFrom(src => src.InitializationVector.ByteArrayToBase64String()));

            CreateMap<SymmetricKey, RegisterSymmetricKeyResponse>()
                .ForMember(dest => dest.WrappedSymmetricKey, opt => opt.MapFrom(src => src.WrappedSymmetricKey.Base64StringToByteArray()))
                .ForMember(dest => dest.InitializationVector, opt => opt.MapFrom(src => src.InitializationVector.Base64StringToByteArray()));

            CreateMap<SymmetricKey, UpdateSymmetricKeyResponse>()
                .ForMember(dest => dest.WrappedSymmetricKey, opt => opt.MapFrom(src => src.WrappedSymmetricKey.Base64StringToByteArray()))
                .ForMember(dest => dest.InitializationVector, opt => opt.MapFrom(src => src.InitializationVector.Base64StringToByteArray()));

            CreateMap<SymmetricKey, ViewSymmetricKeyResponse>()
                .ForMember(dest => dest.WrappedSymmetricKey, opt => opt.MapFrom(src => src.WrappedSymmetricKey.Base64StringToByteArray()))
                .ForMember(dest => dest.InitializationVector, opt => opt.MapFrom(src => src.InitializationVector.Base64StringToByteArray()));
        }
    }
}
