using Microsoft.AspNetCore.Components;

namespace BlazorTabSet
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}