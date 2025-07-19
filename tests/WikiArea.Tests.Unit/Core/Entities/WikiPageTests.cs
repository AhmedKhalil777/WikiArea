using FluentAssertions;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using Xunit;

namespace WikiArea.Tests.Unit.Core.Entities;

public class WikiPageTests
{
    [Fact]
    public void Constructor_ShouldCreateWikiPageWithValidProperties()
    {
        // Arrange
        var title = "Test Page";
        var content = "This is test content";
        var contentType = ContentType.Markdown;
        var folderId = "folder-123";

        // Act
        var wikiPage = new WikiPage(title, content, contentType, folderId);

        // Assert
        wikiPage.Title.Should().Be(title);
        wikiPage.Content.Should().Be(content);
        wikiPage.ContentType.Should().Be(contentType);
        wikiPage.FolderId.Should().Be(folderId);
        wikiPage.Status.Should().Be(PageStatus.Draft);
        wikiPage.Version.Should().Be(1);
        wikiPage.ViewCount.Should().Be(0);
        wikiPage.LikeCount.Should().Be(0);
        wikiPage.Slug.Should().Be("test-page");
        wikiPage.Id.Should().NotBeNullOrEmpty();
        wikiPage.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string invalidTitle)
    {
        // Arrange
        var content = "Valid content";
        var contentType = ContentType.Markdown;

        // Act & Assert
        var action = () => new WikiPage(invalidTitle, content, contentType);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateContent_ShouldUpdateTitleContentAndIncrementVersion()
    {
        // Arrange
        var wikiPage = new WikiPage("Original Title", "Original content", ContentType.Markdown);
        var originalVersion = wikiPage.Version;
        var newTitle = "Updated Title";
        var newContent = "Updated content";
        var updatedBy = "user123";

        // Act
        wikiPage.UpdateContent(newTitle, newContent, updatedBy);

        // Assert
        wikiPage.Title.Should().Be(newTitle);
        wikiPage.Content.Should().Be(newContent);
        wikiPage.Version.Should().Be(originalVersion + 1);
        wikiPage.UpdatedBy.Should().Be(updatedBy);
        wikiPage.Slug.Should().Be("updated-title");
    }

    [Fact]
    public void Publish_ShouldSetStatusToPublished()
    {
        // Arrange
        var wikiPage = new WikiPage("Test Page", "Test content", ContentType.Markdown);
        var publishedBy = "user123";

        // Act
        wikiPage.Publish(publishedBy);

        // Assert
        wikiPage.Status.Should().Be(PageStatus.Published);
        wikiPage.UpdatedBy.Should().Be(publishedBy);
    }

    [Fact]
    public void AddTag_ShouldAddTagToCollection()
    {
        // Arrange
        var wikiPage = new WikiPage("Test Page", "Test content", ContentType.Markdown);
        var tag = "Technology";

        // Act
        wikiPage.AddTag(tag);

        // Assert
        wikiPage.Tags.Should().Contain(tag.ToLowerInvariant());
    }

    [Fact]
    public void AddTag_WithDuplicateTag_ShouldNotAddDuplicate()
    {
        // Arrange
        var wikiPage = new WikiPage("Test Page", "Test content", ContentType.Markdown);
        var tag = "Technology";

        // Act
        wikiPage.AddTag(tag);
        wikiPage.AddTag(tag.ToUpperInvariant()); // Different case

        // Assert
        wikiPage.Tags.Should().HaveCount(1);
        wikiPage.Tags.Should().Contain(tag.ToLowerInvariant());
    }

    [Fact]
    public void IncrementViewCount_ShouldIncreaseViewCountByOne()
    {
        // Arrange
        var wikiPage = new WikiPage("Test Page", "Test content", ContentType.Markdown);
        var originalCount = wikiPage.ViewCount;

        // Act
        wikiPage.IncrementViewCount();

        // Assert
        wikiPage.ViewCount.Should().Be(originalCount + 1);
    }

    [Fact]
    public void GenerateSlug_ShouldCreateValidSlug()
    {
        // Arrange & Act
        var wikiPage1 = new WikiPage("Hello World!", "Content", ContentType.Markdown);
        var wikiPage2 = new WikiPage("Test & Development", "Content", ContentType.Markdown);
        var wikiPage3 = new WikiPage("User's Guide", "Content", ContentType.Markdown);

        // Assert
        wikiPage1.Slug.Should().Be("hello-world!");
        wikiPage2.Slug.Should().Be("test-and-development");
        wikiPage3.Slug.Should().Be("users-guide");
    }
} 