using SquadTUI.Rendering;

namespace SquadTUI.Tests.Unit;

public class MarkdownRendererTests
{
    [Fact]
    public void ExtractHeadings_FindsH1()
    {
        var headings = MarkdownRenderer.ExtractHeadings("# Title").ToList();
        Assert.Single(headings);
        Assert.Equal(1, headings[0].Level);
        Assert.Equal("Title", headings[0].Text);
    }

    [Fact]
    public void ExtractHeadings_FindsH2()
    {
        var headings = MarkdownRenderer.ExtractHeadings("## Section").ToList();
        Assert.Single(headings);
        Assert.Equal(2, headings[0].Level);
        Assert.Equal("Section", headings[0].Text);
    }

    [Fact]
    public void ExtractHeadings_FindsH3()
    {
        var headings = MarkdownRenderer.ExtractHeadings("### Subsection").ToList();
        Assert.Single(headings);
        Assert.Equal(3, headings[0].Level);
        Assert.Equal("Subsection", headings[0].Text);
    }

    [Fact]
    public void ExtractHeadings_FindsMultipleHeadings()
    {
        var md = "# Title\nSome text\n## Section A\nMore text\n## Section B\n### Sub";
        var headings = MarkdownRenderer.ExtractHeadings(md).ToList();
        Assert.Equal(4, headings.Count);
    }

    [Fact]
    public void ExtractHeadings_ReturnsEmpty_ForNoHeadings()
    {
        var headings = MarkdownRenderer.ExtractHeadings("Just plain text\nNo headings here").ToList();
        Assert.Empty(headings);
    }

    [Fact]
    public void ExtractHeadings_HandlesEmptyString()
    {
        var headings = MarkdownRenderer.ExtractHeadings("").ToList();
        Assert.Empty(headings);
    }

    [Theory]
    [InlineData("---")]
    [InlineData("***")]
    [InlineData("___")]
    public void ExtractHeadings_IgnoresHorizontalRules(string hr)
    {
        var headings = MarkdownRenderer.ExtractHeadings(hr).ToList();
        Assert.Empty(headings);
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
        Assert.Equal(4, headings.Count);
        Assert.Equal((1, "Main Title"), headings[0]);
        Assert.Equal((2, "Features"), headings[1]);
        Assert.Equal((3, "Details"), headings[2]);
        Assert.Equal((2, "Another Section"), headings[3]);
    }
}
