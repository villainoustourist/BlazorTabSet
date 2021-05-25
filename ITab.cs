using Microsoft.AspNetCore.Components;

namespace BlazorTabSet
{
    public interface ITab
    {
        string Title { get; }
        RenderFragment ChildContent { get; }
    }
}