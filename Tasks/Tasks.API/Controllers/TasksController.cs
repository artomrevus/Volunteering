using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks.API.Constants;
using Tasks.API.Helpers;
using Tasks.Application.Commands;
using Tasks.Application.Dtos.Pagination;
using Tasks.Application.Dtos.Tasks;
using Tasks.Application.Queries;

namespace Tasks.API.Controllers;

[Route("[controller]")]
public class TasksController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Military}")] 
    public async Task<IActionResult> CreateAsync([FromBody] CreateTaskDto dto)
    {
        var militaryId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new CreateTaskCommand 
        { 
            MilitaryId = militaryId,
            Title = dto.Title, 
            Description = dto.Description,
            Priority = dto.Priority,
        };
        
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    
    [HttpPut]
    [Authorize(Roles = $"{UserRoles.Military}")] 
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateTaskDto dto)
    {
        var militaryId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new UpdateTaskCommand 
        { 
            TaskId = dto.TaskId,
            MilitaryId = militaryId,
            Title = dto.Title, 
            Description = dto.Description,
            Priority = dto.Priority,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("confirm")]
    [Authorize(Roles = $"{UserRoles.Military}")] 
    public async Task<IActionResult> ConfirmAsync([FromBody] ConfirmTaskDto dto)
    {
        var militaryId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new ConfirmTaskCommand
        {
            TaskId = dto.TaskId,
            MilitaryId = militaryId,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("start")]
    [Authorize(Roles = $"{UserRoles.Volunteer}")] 
    public async Task<IActionResult> StartAsync([FromBody] StartTaskDto dto)
    {
        var volunteerId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new StartTaskCommand
        {
            TaskId = dto.TaskId,
            VolunteerId = volunteerId,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("status")]
    [Authorize(Roles = $"{UserRoles.Volunteer}")] 
    public async Task<IActionResult> UpdateStatusAsync([FromBody] UpdateTaskStatusDto dto)
    {
        var volunteerId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new UpdateTaskStatusCommand
        {
            TaskId = dto.TaskId,
            VolunteerId = volunteerId,
            Status = dto.Status,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAsync()
    {
        var query = new GetTasksQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
    {
        var query = new GetTaskByIdQuery { TaskId = id };
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("filter")]
    [Authorize]
    public async Task<IActionResult> GetFilteredTasks(
        [FromQuery] TaskFilterDto filterDto,
        [FromQuery] TaskSortingDto sortingDto,
        [FromQuery] PaginationDto paginationDto)
    {
        var query = new GetTasksWithFilterQuery
        {
            Status = filterDto.Status,
            Priority = filterDto.Priority,
            MilitaryId = filterDto.MilitaryId,
            VolunteerId = filterDto.VolunteerId,
            CreatedAtFrom = filterDto.CreatedAtFrom,
            CreatedAtTo = filterDto.CreatedAtTo,
            SortBy = sortingDto.SortBy,
            IsDescending = sortingDto.IsDescending,
            PageNumber = paginationDto.PageNumber,
            PageSize = paginationDto.PageSize,
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }
}