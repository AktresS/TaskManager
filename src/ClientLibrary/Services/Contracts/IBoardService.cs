using System;
using BaseLibrary.DTOs.BoardDtos;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IBoardService
{
    Task<List<BoardBaseResponse>> GetByProjectAsync(int projectId);
    Task<BoardBaseResponse> CreateAsync(int projectId, CreateBoardRequest request);
    Task DeleteAsync(int projectId, int id);
}
