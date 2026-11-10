using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace BlazorTabSet.Tests;

public class TabSetTests : BunitContext
{
    private static RenderFragment Tabs(Action<string>? onClick, params string[] titles) => builder =>
    {
        var seq = 0;
        foreach (var title in titles)
        {
            builder.OpenComponent<Tab>(seq++);
            builder.AddAttribute(seq++, nameof(Tab.Title), title);
            builder.AddAttribute(seq++, nameof(Tab.OnClick), onClick);
            builder.AddAttribute(seq++, nameof(Tab.ChildContent), (RenderFragment)(b => b.AddContent(0, $"Body of {title}")));
            builder.CloseComponent();
        }
    };

    private IRenderedComponent<TabSet> RenderTabSet(string? selectedTitle = null, Action<string>? onClick = null) =>
        Render<TabSet>(ps => ps
            .Add(p => p.SelectedTitle, selectedTitle)
            .Add(p => p.ChildContent, Tabs(onClick, "One", "Two", "Three")));

    private static string ActiveTitle(IRenderedComponent<TabSet> cut) =>
        cut.Find("ul.nav-tabs a.active").TextContent.Trim();

    private static string Body(IRenderedComponent<TabSet> cut) =>
        cut.Find("div.tab-pane").TextContent.Trim();

    [Fact]
    public void RendersAHeaderForEachTab()
    {
        var cut = RenderTabSet();

        var titles = cut.FindAll("ul.nav-tabs a.nav-link").Select(a => a.TextContent.Trim());
        Assert.Equal(new[] { "One", "Two", "Three" }, titles);
    }

    [Fact]
    public void FirstTabIsActiveByDefault()
    {
        var cut = RenderTabSet();

        Assert.Equal("One", ActiveTitle(cut));
        Assert.Equal("Body of One", Body(cut));
        Assert.Single(cut.FindAll("ul.nav-tabs a.active"));
    }

    [Fact]
    public void SelectedTitlePicksTheMatchingTab()
    {
        var cut = RenderTabSet(selectedTitle: "Two");

        Assert.Equal("Two", ActiveTitle(cut));
        Assert.Equal("Body of Two", Body(cut));
    }

    [Fact]
    public void SelectedTitleIgnoresCase()
    {
        var cut = RenderTabSet(selectedTitle: "three");

        Assert.Equal("Three", ActiveTitle(cut));
    }

    [Fact]
    public void UnknownSelectedTitleFallsBackToFirstTab()
    {
        var cut = RenderTabSet(selectedTitle: "Nope");

        Assert.Equal("One", ActiveTitle(cut));
    }

    [Fact]
    public void ClickingATabMakesItActive()
    {
        var cut = RenderTabSet();

        cut.FindAll("ul.nav-tabs a.nav-link").Single(a => a.TextContent.Trim() == "Three").Click();

        Assert.Equal("Three", ActiveTitle(cut));
        Assert.Equal("Body of Three", Body(cut));
        Assert.Single(cut.FindAll("ul.nav-tabs a.active"));
    }

    [Fact]
    public void ClickingATabRaisesOnClickWithTitle()
    {
        var clicked = new List<string>();
        var cut = RenderTabSet(onClick: clicked.Add);

        cut.FindAll("ul.nav-tabs a.nav-link").Single(a => a.TextContent.Trim() == "Two").Click();

        Assert.Equal(new[] { "Two" }, clicked);
    }

    [Fact]
    public void ActiveTabIsExposedOnTheComponent()
    {
        var cut = RenderTabSet(selectedTitle: "Two");

        Assert.Equal("Two", cut.Instance.ActiveTab?.Title);
    }

    [Fact]
    public void RemovingTheActiveTabClearsTheBody()
    {
        var cut = RenderTabSet();
        var active = cut.Instance.ActiveTab!;

        cut.InvokeAsync(() => cut.Instance.RemoveTab(active));

        Assert.Null(cut.Instance.ActiveTab);
        Assert.Equal("", Body(cut));
    }
}
