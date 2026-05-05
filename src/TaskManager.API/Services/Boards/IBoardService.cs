using BaseLibrary.DTOs.BoardDtos;
using BaseLibrary.Responses;

namespace TaskManager.Services.Boards;

public interface IBoardService
{
    Task<List<BoardBaseResponse>> GetByProjectAsync(int projectId);
    Task<BoardBaseResponse> CreateAsync(int projectId, CreateBoardRequest request);
    Task DeleteAsync(int id);
}
