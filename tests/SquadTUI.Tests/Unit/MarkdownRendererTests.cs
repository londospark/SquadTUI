using FluentAssertions;
using SquadTUI.Rendering;

namespace SquadTUI.Tests.Unit;

public class MarkdownRendererTests
{
    [Fact]
    public void ExtractHeadings_FindsH1()
    {
        var headings = MarkdownRenderer.ExtractHeadings("# Title").ToList();
        headings.Should().ContainSingle();
        headings[0].Level.Should().Be(1);
        headings[0].Text.Should().Be("Title");
    }

    [Fact]
    public void ExtractHeadings_FindsH2()
    {
        var headings = MarkdownRenderer.ExtractHeadings("## Section").ToList();
        headings.Should().ContainSingle();
        headings[0].Level.Should().Be(2);
        headings[0].Text.Should().Be("Section");
    }

    [Fact]
    public void ExtractHeadings_FindsH3()
    {
        var headings = MarkdownRenderer.ExtractHeadings("### Subsection").ToList();
        headings.Should().ContainSingle();
        headings[0].Level.Should().Be(3);
        headings[0].Text.Should().Be("Subsection");
    }

    [Fact]
    public void ExtractHeadings_FindsMultipleHeadings()
    {
        var md = "# Title\nSome text\n## Section A\nMore text\n## Section B\n### Sub";
        var headings = MarkdownRenderer.ExtractHeadings(md).ToList();
        headings.Should().HaveCount(4);
    }

    [Fact]
    public void ExtractHeadings_ReturnsEmpty_ForNoHeadings()
    {
        var headings = MarkdownRenderer.ExtractHeadings("Just plain text\nNo headings here").ToList();
        headings.Should().BeEmpty();
    }

    [Fact]
    public void ExtractHeadings_HandlesEmptyString()
    {
        var headings = MarkdownRenderer.ExtractHeadings("").ToList();
        headings.Should().BeEmpty();
    }

    [Theory]
    [InlineData("---")]
    [InlineData("***")]
    [InlineData("___")]
    public void ExtractHeadings_IgnoresHorizontalRules(string hr)
    {
        var headings = MarkdownRenderer.ExtractHeadings(hr).ToList();
        headings.Should().BeEmpty();
    }

    [Fact]
    public void ExtractHeadings_HandlesMixedContent()
    {
        var md = @"# Main Title

Some paragraph text here.

## Features
- Item 1
- Item 2

### Details
> A blockquote

## Another Section";

        var headings = MarkdownRenderer.ExtractHeadings(md).ToList();
        headings.Should().HaveCount(4);
        headings[0].Should().Be((1, "Main Title"));
        headings[1].Should().Be((2, "Features"));
        headings[2].Should().Be((3, "Details"));
        headings[3].Should().Be((2, "Another Section"));
    }
}
