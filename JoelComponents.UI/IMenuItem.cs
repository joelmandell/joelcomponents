namespace JoelComponents.UI;

public interface IMenuItem
{
    public Guid ComponentId { get; set; }
    public IMenuItem? ParentMenuItem { get; set; }
}