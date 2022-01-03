using System.Text.Json;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using DTO = SagaConfigService.DTO;
using Entities = SagaConfigService.Entities;

namespace SagaConfigService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Entities.SagaConfiguration, DTO.SagaConfiguration>();
        CreateMap<Entities.SagaConfiguration, DTO.SagaConfiguration>().ReverseMap();
        CreateMap<Entities.SagaStep, DTO.SagaStep>();
        CreateMap<Entities.SagaStep, DTO.SagaStep>().ReverseMap();
        
        // TODO: Fix conversion of Command from BsonDocument to Json
        CreateMap<Entities.SagaAction, DTO.SagaAction>();
            // .ForMember(dest => dest.Command,
            //     options => options.MapFrom<BsonDocument>(
            //         src => BsonSerializer.Deserialize<BsonDocument>(src.Command, 
            //             builder => builder.ToJson(new JsonWriterSettings(), null, null, default (BsonSerializationArgs)))));
            // .ForMember(dest => dest.Command,
            //     options => 
            //         options.MapFrom(src => src.Command.ToJson( new JsonWriterSettings(), null, null, default (BsonSerializationArgs))));
        CreateMap<Entities.SagaAction, DTO.SagaAction>()
            .ReverseMap()
            .ForMember(dest => dest.Command,
                options => 
                    options.MapFrom(src => BsonDocument.Parse(src.Command.ToString())));
    }
}
