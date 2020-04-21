using Microsoft.AspNetCore.Components;

namespace BlazorTabs
{
    public interface ITab
    {
        RenderFragment ChildContent { get; }
    }
}