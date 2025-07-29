using JoelComponents.UI.Models;
using Microsoft.AspNetCore.Components;

namespace JoelComponents.UI.Components.Menu;

public partial class MenuItems
{
    [CascadingParameter(Name="Menu")]
    public Menu? Menu { get; set; }
    
    [CascadingParameter(Name="MenuItem")]
    public MenuItem? ParentMenuItem { get; set; }
    
    [CascadingParameter(Name="MenuItems")]
    public MenuItems? ParentMenuItems { get; set; }
    
    [CascadingParameter(Name="IsTopMenu")]
    public bool IsTopMenu { get; set; }

    [CascadingParameter(Name = "MenuStateTracker")]
    private MenuState? MenuState { get; set; }
    
    [CascadingParameter(Name = "ParentIsMenuItems")]
    private bool IsParentAlsoMenuItems { get; set; }
    
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        if (Menu == null)
        {
            throw new InvalidOperationException("The first MenuItems component needs to be placed inside an <Menu> component.");
        }

        if (IsParentAlsoMenuItems)
        {
            throw new InvalidOperationException("A MenuItems component cannot be a direct child of another MenuItems component. To create a sub-menu, nest it inside a MenuItem.");
        }
        
        if (MenuState != null)
        {
            // Check if another MenuItems has already been registered
            if (MenuState.HasRootMenuItems && this.IsTopMenu)
            {
                throw new InvalidOperationException("Only one MenuItems component is allowed as a direct child of a Menu component.");
            }
            
            MenuState.HasRootMenuItems = true;
        }
        
        base.OnInitialized();
    }
}