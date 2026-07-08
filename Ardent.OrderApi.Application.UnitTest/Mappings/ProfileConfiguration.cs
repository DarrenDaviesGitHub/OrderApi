using Ardent.OrderApi.Application.MappingProfiles;
using Ardent.OrderApi.Application.UnitTest.Helpers;
using AutoMapper;

namespace Ardent.OrderApi.Application.UnitTest.Mappings;

public class ProfileConfiguration
{
    private readonly MapperConfiguration _configuration;

    public ProfileConfiguration()
    {
        var mapper = MapperFactory.Create(
            new ProductProfile(),
            new OrderProfile());

        _configuration = mapper.Configuration;
    }


    [Fact]
    public void MappingProfiles_ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }
}