
using BaseLibrary.DTOs.BoardDtos;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.Boards;

public class BoardService(AppDbContext context, IProjectAuthorizationService projectAuthService) : IBoardService
{
    public async Task<BoardBaseResponse> CreateAsync(int projectId, CreateBoardRequest request)
    {
        await projectAuthService.EnsureAdmin(projectId);

        var board = new Board
        {
            Name = request.Name,
            ProjectId = projectId
        };

        context.Boards.Add(board);
        await context.SaveChangesAsync();

        return new BoardBaseResponse
        {
            Id = board.Id,
            Name = board.Name
        };
    }

    public async Task<List<BoardBaseResponse>> GetByProjectAsync(int projectId)
    {
        await projectAuthService.EnsureMember(projectId);

        var board = await context.Boards
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();

        return board.Select(x => new BoardBaseResponse
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
    }
    
    public async Task DeleteAsync(int id)
    {
        var board = await context.Boards.FindAsync(id);
        if (board is null)
            throw new Exception("Board not found");

        await projectAuthService.EnsureAdmin(board.ProjectId);

        context.Boards.Remove(board);
        await context.SaveChangesAsync();
    }
}
