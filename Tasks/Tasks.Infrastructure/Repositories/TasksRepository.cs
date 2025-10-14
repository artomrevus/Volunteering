using Microsoft.EntityFrameworkCore;
using Tasks.Application.Interfaces;
using Tasks.Application.Interfaces.Repositories;
using Tasks.Domain.Constants;
using Tasks.Domain.Entities;
using Tasks.Domain.Models;
using Tasks.Infrastructure.Persistence;

namespace Tasks.Infrastructure.Repositories;

public class TasksRepository(MongoDbContext context) : ITasksRepository
{
    public async Task<TaskEntity?> GetByIdAsync(string id)
    {
        return await context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<IEnumerable<TaskEntity>> GetAllAsync()
    {
        return await context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<TaskEntity> AddAsync(TaskEntity task)
    {
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return task;
    }
    
    public async Task UpdateAsync(TaskEntity task)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync();
    }
    
    public async Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetFilteredTasksAsync(
        TaskFilter filter, 
        TaskSorting sorting, 
        int pageNumber, 
        int pageSize)
    {
        IQueryable<TaskEntity> query = context.Tasks;
        query = ApplyFilters(query, filter);
        query = ApplySorting(query, sorting);
        
        var totalCount = await query.CountAsync();
        
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        var tasks = await query.ToListAsync();
        
        return (tasks, totalCount);
    }
    
    private static IQueryable<TaskEntity> ApplyFilters(IQueryable<TaskEntity> query, TaskFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(x => x.Status.Equals(filter.Status, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Priority))
        {
            query = query.Where(x => x.Priority.Equals(filter.Priority, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrWhiteSpace(filter.MilitaryId))
        {
            query = query.Where(x => x.MilitaryId.Equals(filter.MilitaryId, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrWhiteSpace(filter.VolunteerId))
        {
            query = query.Where(x => x.VolunteerId != null && x.VolunteerId.Equals(filter.VolunteerId, StringComparison.OrdinalIgnoreCase));
        }
        
        if (filter.CreatedAtFrom.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= filter.CreatedAtFrom.Value);
        }
        
        if (filter.CreatedAtTo.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= filter.CreatedAtTo.Value);
        }
        
        return query;
    }
    
    private static IQueryable<TaskEntity> ApplySorting(IQueryable<TaskEntity> query, TaskSorting sorting)
    {
        Dictionary<string, int> priorityOrder = new(StringComparer.OrdinalIgnoreCase)
        {
            [TaskPriorities.Low] = 0,
            [TaskPriorities.Average] = 1,
            [TaskPriorities.High] = 2,
        };
            
        return (sorting.SortBy?.ToLowerInvariant(), sorting.IsDescending) switch
        {
            ("priority", false) => query.OrderBy(x => priorityOrder.ContainsKey(x.Priority) ? priorityOrder[x.Priority] : -1),
            ("priority", true) => query.OrderByDescending(x => priorityOrder.ContainsKey(x.Priority) ? priorityOrder[x.Priority] : -1),
            ("createdat", false) => query.OrderBy(x => x.CreatedAt),
            ("createdat", true) => query.OrderByDescending(x => x.CreatedAt),
            _ => throw new ArgumentException("Sort by must be 'Priority' or 'CreatedAt'")
        };
    }
}