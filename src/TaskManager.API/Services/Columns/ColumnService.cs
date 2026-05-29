
using System.Data;
using BaseLibrary.DTOs;
using BaseLibrary.DTOs.ColumnDtos;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services.ProjectAuthorization;

namespace TaskManager.Services.Columns;

public class ColumnService(AppDbContext context, IProjectAuthorizationService projectAuthService) : IColumnService
{
    public async Task<List<ColumnBaseResponse>> GetByBoardAsync(int boardId)
    {
        var board = await context.Boards.FindAsync(boardId);
        if (board == null)
            throw new Exception("Board not found");

        await projectAuthService.EnsureMember(board.ProjectId);

        var columns = await context.BoardColumns
            .Include(c => c.WorkItems)
                .ThenInclude(w => w.Assignees)
                    .ThenInclude(a => a.User)
            .Where(x => x.BoardId == boardId)
            .OrderBy(x => x.Order)
            .ToListAsync();

        return columns.Select(x => new ColumnBaseResponse
        {
            Id = x.Id,
            Name = x.Name,
            Order = x.Order,
            AutoStatus = x.AutoStatus,
            WorkItems = x.WorkItems.Select(w => new WorkItemResponse
            {
                Id = w.WorkItemId,
                Title = w.Title,
                Description = w.Description,
                Priority = w.Priority,
                State = w.State,
                CompletedAt = w.CompletedAt,
                CreatedById = w.CreatedById ?? 0,
                DeadLine = w.DeadLine,
                Assignees   = w.Assignees.Select(a => new UserShortDto
                {
                    Id        = a.UserId,
                    Name      = a.User.Name,
                    AvatarUrl = a.User.AvatarUrl
                }).ToList()
            }).ToList()
        }).ToList();
    }

    public async Task<ColumnBaseResponse> CreateAsync(int boardId, CreateColumnRequest request)
    {
        var board = await context.Boards.FindAsync(boardId);
        if (board == null)
            throw new Exception("Board not found");

        await projectAuthService.EnsureMember(board.ProjectId);

        var maxOrder = await context.BoardColumns
            .Where(x => x.BoardId == boardId)
            .Select(x => (int?)x.Order)
            .MaxAsync() ?? 0;

        var column = new BoardColumn
        {
            Name = request.Name,
            BoardId = boardId,
            Order = maxOrder + 1
        };

        context.BoardColumns.Add(column);
        await context.SaveChangesAsync();

        return new ColumnBaseResponse
        {
            Id = column.Id,
            Name = column.Name,
            Order = column.Order
        };
    }

    public async Task<ColumnBaseResponse> UpdateAsync(int id, UpdateColumnRequest request)
    {
        var column = await context.BoardColumns
            .Include(x => x.Board)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (column == null)
            throw new Exception("Column not found");
        
        await projectAuthService.EnsureMember(column.Board.ProjectId);

        if (request.Name != null)
            column.Name = request.Name;
        
        if (request.Order.HasValue)
            column.Order = request.Order.Value;

        if (request.ClearAutoStatus)
            column.AutoStatus = null;
        else if (request.AutoStatus.HasValue)
            column.AutoStatus = request.AutoStatus.Value;

        await context.SaveChangesAsync();

        return new ColumnBaseResponse
        {
            Id = column.Id,
            Name = column.Name,
            Order = column.Order
        };
    }

    public async Task DeleteAsync(int id)
    {
        var column = await context.BoardColumns
            .Include(x => x.Board)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (column == null)
            throw new Exception("Column not found");

        await projectAuthService.EnsureAdmin(column.Board.ProjectId);

        context.BoardColumns.Remove(column);
        await context.SaveChangesAsync();
    }
}
