using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application.Commands;
using Notifications.Application.Dtos.Bindings;
using Notifications.Application.Dtos.Messages;
using Notifications.Application.Exceptions;
using Notifications.Application.Queries;

namespace Notifications.API.Controllers;

[Route("[controller]/tasks")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpPost("started")]
    public async Task<IActionResult> SendTaskStartedNotificationAsync([FromBody] TaskStartedMessageDto dto)
    {
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = dto.MilitaryToNotifyId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return Ok();
        }
        
        var sendEmailCommand = new SendTaskStartedEmailCommand
        {
            EmailTo = binding.Email,
            TaskTitle = dto.TaskTitle,
        };

        await mediator.Send(sendEmailCommand);
        
        return Ok();
    }
    
    [HttpPost("status")]
    public async Task<IActionResult> SendTaskStatusUpdatedNotificationAsync([FromBody] TaskStatusUpdatedMessageDto dto)
    {
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = dto.MilitaryToNotifyId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return Ok();
        }
        
        var sendEmailCommand = new SendTaskStatusUpdatedEmailCommand
        {
            EmailTo = binding.Email,
            TaskTitle = dto.TaskTitle,
            OldTaskStatus = dto.OldTaskStatus,
            NewTaskStatus = dto.NewTaskStatus,
        };

        await mediator.Send(sendEmailCommand);
        
        return Ok();
    }
    
    [HttpPost("confirmed")]
    public async Task<IActionResult> SendTaskConfirmedNotificationAsync([FromBody] TaskConfirmedMessageDto dto)
    {
        BindingDto binding;
        try
        {
            var getBindingQuery = new GetBindingByIdentityIdQuery { IdentityId = dto.VolunteerToNotifyId };
            binding = await mediator.Send(getBindingQuery);
        }
        catch (NotFoundException)
        {
            return Ok();
        }
        
        var sendEmailCommand = new SendTaskConfirmedEmailCommand
        {
            EmailTo = binding.Email,
            TaskTitle = dto.TaskTitle,
        };

        await mediator.Send(sendEmailCommand);
        
        return Ok();
    }
}