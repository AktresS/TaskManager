
namespace TaskManager.Models;

public class Board
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
}
