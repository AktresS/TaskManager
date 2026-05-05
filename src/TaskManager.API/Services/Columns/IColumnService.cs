
using BaseLibrary.DTOs.ColumnDtos;
using BaseLibrary.Responses;

namespace TaskManager.Services.Columns;

public interface IColumnService
{
    Task<List<ColumnBaseResponse>> GetByBoardAsync(int boardId);
    Task<ColumnBaseResponse> CreateAsync(int boardId, CreateColumnRequest request);
    Task<ColumnBaseResponse> UpdateAsync(int id, UpdateColumnRequest request);
    Task DeleteAsync(int id);
}
