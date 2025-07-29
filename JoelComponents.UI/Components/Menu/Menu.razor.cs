using JoelComponents.UI.Models;
using Microsoft.AspNetCore.Components;

namespace JoelComponents.UI.Components.Menu;

public partial class Menu
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    /// <summary>
    /// Pass a void method that takes <see cref="MenuItem"/> as a param.
    /// This triggers on a <c>Mouse click</c>. On MenuItem is a Tag property that can be filtered
    /// for different actions.
    /// </summary>
    /// 
    [Parameter]
    public EventCallback<MenuItem> OnClick { get; set; }
    
    /// <summary>
    /// Pass a void method that takes <see cref="MenuItem"/> as a param.
    /// This triggers on a <c>Enter keydown</c>. On MenuItem is a Tag property that can be filtered
    /// for different actions.
    /// </summary>
    [Parameter]
    public EventCallback<MenuItem> OnKeyEnter { get; set; }
    
    public ElementReference? ElementReference { get; set; }

    private List<IMenuItem> _expandedPath = new();
    
    // Each Menu instance has its own state tracker.
    private readonly MenuState _menuState = new();
    
    /// <summary>
    /// For tracking the path of hoveredItems.
    /// </summary>
    /// <param name="hoveredItem"></param>
    internal void SetExpandedPath(IMenuItem hoveredItem)
    {
        _expandedPath.Clear();
        var current = hoveredItem;
        
        while (current != null)
        {
            _expandedPath.Add(current);
            current = current.ParentMenuItem;
        }
    }

    internal bool IsItemExpanded(IMenuItem item)
    {
        return _expandedPath.Contains(item);
    }
    
    public void ClearExpandedItems(IMenuItem item)
    {
        //Clear all but the passed *item* that it was being onmouseouted
        _expandedPath = _expandedPath.Where(x => x != item).ToList();
    }

    public void ClearAllExpandedItems()
    {
        _expandedPath.Clear();
    }
}