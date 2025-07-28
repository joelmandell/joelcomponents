using Microsoft.AspNetCore.Components;

namespace JoelComponents.UI;

public partial class MenuItems
{
    [CascadingParameter(Name="Menu")]
    public Menu? Menu { get; set; }
    
    [CascadingParameter(Name="MenuItem")]
    public MenuItem? ParentMenuItem { get; set; }
    
    [CascadingParameter(Name="IsTopMenu")]
    public bool IsTopMenu { get; set; }
        
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnInitialized()
    {
        if (Menu == null)
        {
            throw new InvalidOperationException("The first MenuItems component needs to be placed inside an <Menu> component.");
        }
        base.OnInitialized();
    }
}