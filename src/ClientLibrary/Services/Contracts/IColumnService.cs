using BaseLibrary.DTOs.ColumnDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IColumnService
{
    Task<List<ColumnBaseResponse>> GetByBoardAsync(int boardId);
    Task<ColumnBaseResponse> CreateAsync(int boardId, CreateColumnRequest request);
    Task<ColumnBaseResponse> UpdateAsync(int columnId, int boardId, UpdateColumnRequest request);
    Task DeleteAsync(int columnId, int boardId);
}
