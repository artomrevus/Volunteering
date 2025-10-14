using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.API.Helpers;
using Notifications.Application.Commands;
using Notifications.Application.Dtos.Bindings;
using Notifications.Application.Queries;

namespace Notifications.API.Controllers;

[Route("[controller]")]
public class BindingsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBindingDto dto)
    {
        var identityId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new CreateBindingCommand
        {
            IdentityId = identityId,
            Email = dto.Email,
        };
        
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAsync()
    {
        var identityId = ClaimsHelper.GetNameIdentifier(User);
        
        var query = new GetBindingByIdentityIdQuery
        {
            IdentityId = identityId
        };
        
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateBindingDto dto)
    {
        var identityId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new UpdateBindingCommand 
        { 
            IdentityId = identityId,
            Email = dto.Email,
        };
        
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAsync()
    {
        var identityId = ClaimsHelper.GetNameIdentifier(User);
        
        var command = new DeleteBindingCommand
        {
            IdentityId = identityId
        };
        
        await mediator.Send(command);
        return NoContent();
    }
}