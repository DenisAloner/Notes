using Mapster;
using Notes.CoreService.DTO;

namespace Notes.CoreService.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterTypeMapping(this IServiceCollection services)
    {
        TypeAdapterConfig<PatchNoteRequest, PatchNoteInput>.NewConfig().Map(d => d, s => s.Body);
        return services;
    }
}