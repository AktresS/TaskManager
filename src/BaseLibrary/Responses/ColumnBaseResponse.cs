
using BaseLibrary.Enums;

namespace BaseLibrary.Responses;

public class ColumnBaseResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<WorkItemResponse> WorkItems { get; set; } = new();
    public TaskState? AutoStatus { get; set; }
}
