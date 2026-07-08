using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Ardent.OrderApi.Application.UnitTest.Helpers;

public static class MapperFactory
{
    public static (MapperConfiguration Configuration, IMapper Mapper) Create(params Profile[] profiles)
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            foreach (var profile in profiles)
            {
                cfg.AddProfile(profile);
            }
        }, LoggerFactory.Create(_ => { }));

        return (configuration, configuration.CreateMapper());
    }
}