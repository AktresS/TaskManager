
namespace ClientLibrary.Services.Implementations;

public class SearchService
{
    public string SearchQuery { get; private set; } = string.Empty;
    public event Action? OnChanged;

    public void SetQuery(string query)
    {
        SearchQuery = query;
        OnChanged?.Invoke();
    }
}