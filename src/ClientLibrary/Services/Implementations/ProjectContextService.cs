
namespace ClientLibrary.Services.Implementations;

public class ProjectContextService
{
    public string? ProjectName { get; private set; }
    public event Action? OnChanged;

    public void SetProject(string? name)
    {
        ProjectName = name;
        OnChanged?.Invoke();
    }

    public void Clear()
    {
        ProjectName = null;
        OnChanged?.Invoke();
    }
}