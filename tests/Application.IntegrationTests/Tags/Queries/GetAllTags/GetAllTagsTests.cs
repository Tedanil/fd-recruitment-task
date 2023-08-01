using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Tags.Queries.GetAllTags;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.Tags.Queries.GetAllTags;
using static Testing;

public class GetAllTagsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnAllTags()
    {
        // Arrange
        await AddAsync(new Tag { Name = "Tag1" });
        await AddAsync(new Tag { Name = "Tag2" });
        await AddAsync(new Tag { Name = "Tag3" });

        var query = new GetAllTagsQuery();

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Tags.Should().HaveCount(3);
        result.Tags.ElementAt(0).Name.Should().Be("Tag1");
        result.Tags.ElementAt(1).Name.Should().Be("Tag2");
        result.Tags.ElementAt(2).Name.Should().Be("Tag3");
    }
}
