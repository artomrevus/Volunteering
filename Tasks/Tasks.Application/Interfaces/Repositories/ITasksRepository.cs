using Tasks.Domain.Entities;
using Tasks.Domain.Models;

namespace Tasks.Application.Interfaces.Repositories;

public interface ITasksRepository
{
    Task<TaskEntity?> GetByIdAsync(string id);
    
    Task<IEnumerable<TaskEntity>> GetAllAsync();
    
    Task<TaskEntity> AddAsync(TaskEntity task);
    
    Task UpdateAsync(TaskEntity task);

    Task<(IEnumerable<TaskEntity> tasks, int totalCount)> GetFilteredTasksAsync(
        TaskFilter filter,
        TaskSorting sorting,
        int pageNumber,
        int pageSize);
}