using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JoelComponents.UI.Components.Menu;

public partial class MenuItem(MenuJsInterop menuJsInterop)
{
    [CascadingParameter(Name="Menu")]
    public Menu? ParentMenu { get; set; }

    [CascadingParameter(Name="MenuItems")]
    public MenuItems? ParentMenuItems { get; set; }
    
    [CascadingParameter(Name="MenuItem")]
    public IMenuItem? ParentMenuItem { get; set; }

    [CascadingParameter(Name="IsTopMenu")]
    public bool IsTopMenu { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter]
    public required string Text { get; set; }
    
    /// <summary>
    /// This is a tag that can used to listen in an onclick event to "assign" an action.
    /// That is for example if a MenuItem has the tag "FetchWeather"
    /// in the event handler for <c><Menu></c>-component one can filter for the tag "FetchWeather" to
    /// call the corresponding action.
    /// </summary>
    [Parameter]
    public string? Tag { get; set; }
    
    private ElementReference? MenuItemReference { get; set; }
    
    private bool IsFocused { get; set; }

    private bool HasSubmenu => ChildContent != null;

    private bool IsExpanded => ParentMenu?.IsItemExpanded(this) ?? false;
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.ShiftKey && e.Key == "Tab")
        {
            //Make sure to clear away menuitems 
            if(HasSubmenu)
                ParentMenu?.ClearExpandedItems(this);

            IsFocused = false;
        }

        if (e.Key == "Escape")
        {
            await Task.FromResult(menuJsInterop?.BlurElement(this.MenuItemReference));
            ParentMenu?.ClearAllExpandedItems();
        }
        
        if (e.Key == "Enter")
        {
            if (ParentMenu is { OnKeyEnter: { HasDelegate: true } } parentMenu)
            {
                _ = InvokeAsync(() =>  parentMenu.OnKeyEnter.InvokeAsync(this));

                await Task.FromResult(menuJsInterop?.BlurElement(this.MenuItemReference));
                ParentMenu?.ClearAllExpandedItems();
            }
        }
    }

    private async Task HandleOnClick()
    {
        if (ParentMenu is { OnClick: { HasDelegate: true } } parentMenu)
        {
            _ = InvokeAsync(() =>  parentMenu.OnClick.InvokeAsync(this));
            await Task.FromResult(menuJsInterop?.BlurElement(this.MenuItemReference));
            ParentMenu?.ClearAllExpandedItems();
        }
    }

    private void UpdateExpandedPath()
    {
        if(HasSubmenu)
            ParentMenu?.SetExpandedPath(this);
    }
    
    private void HandleMouseEnter()
    {
       UpdateExpandedPath();
    }

    private void HandleFocus()
    {
        UpdateExpandedPath();
    }
    
    private void HandleMouseLeave()
    {
        if(HasSubmenu)
            ParentMenu?.ClearExpandedItems(this);

        IsFocused = false;
    }

    public Guid ComponentId { get; set; } = Guid.NewGuid();
}