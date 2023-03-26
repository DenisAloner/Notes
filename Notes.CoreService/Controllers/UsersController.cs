using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notes.CoreService.Domain.Notes;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Notes.CoreService.DTO;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Exceptions;
using Notes.CoreService.Extensions;

namespace Notes.CoreService.Controllers;

/// <summary>
/// Методы для работы с пользователями
/// </summary>
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получение списка заметок пользователя
    /// </summary>
    /// <response code="200">Возвращает список заметок пользователя</response>
    [HttpGet("{userId:guid}/notes")]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<Page<UserNote>>> GetUserNotesAsync(
        [FromRoute] Guid userId,
        [FromQuery] GetNotesInput input
    )
    {
        var currentUserId = HttpContext.GetUserId();
        if (currentUserId != userId) throw DomainException.Forbidden();
        var response = await _mediator.Send(new GetUserNotesQuery { UserId = currentUserId, Input = input });
        return Ok(response);
    }
}