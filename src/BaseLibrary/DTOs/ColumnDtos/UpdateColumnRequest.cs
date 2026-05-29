
using BaseLibrary.Enums;

namespace BaseLibrary.DTOs.ColumnDtos;

public class UpdateColumnRequest
{
    public string? Name { get; set; }
    public int? Order { get; set; }
    public TaskState? AutoStatus { get; set; }
    public bool ClearAutoStatus { get; set; }
}
