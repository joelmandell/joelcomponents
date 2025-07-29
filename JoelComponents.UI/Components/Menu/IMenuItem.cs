namespace JoelComponents.UI.Components.Menu;

public interface IMenuItem
{
    public Guid ComponentId { get; set; }
    public IMenuItem? ParentMenuItem { get; set; }
}