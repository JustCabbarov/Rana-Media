using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using RanaMedia.Application.Profiles;
using RanaMedia.Contract.DTOs.Auth;
using RanaMedia.Contract.DTOs.Editorial;
using RanaMedia.Domain.Entities;
using RanaMedia.Domain.Enums;
using Xunit;

namespace RanaMedia.Tests;

public class AutoMapperConfigurationTests
{
    [Fact]
    public void CustomProfile_ShouldBeValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CustomProfile>(), NullLoggerFactory.Instance);
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void CustomProfile_ShouldMapArticleAndUserCorrectly()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<CustomProfile>(), NullLoggerFactory.Instance).CreateMapper();

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "test@ranamedia.co",
            FirstName = "Aysel",
            LastName = "Mammadova"
        };
        var userDto = mapper.Map<UserDTO>(user);
        Assert.Equal("Aysel Mammadova", userDto.FullName);

        var section = new Section { Id = Guid.NewGuid(), Slug = "philosophy", Title = "Philosophy", AccentColorHex = "#0c796e" };
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Slug = "test-essay",
            Title = "Test Essay",
            Type = ArticleType.Essay,
            Section = section,
            StandfirstShort = "Short",
            StandfirstLong = "Long",
            BodyHtml = "<p>Body</p>"
        };

        var detailDto = mapper.Map<ArticleDetailDTO>(article);
        Assert.Equal("test-essay", detailDto.Slug);
        Assert.Equal("philosophy", detailDto.Section?.Slug);
        Assert.Equal("center", detailDto.Settings.AlignX);
    }
}
