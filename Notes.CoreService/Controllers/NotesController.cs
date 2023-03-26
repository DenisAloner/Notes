using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notes.CoreService.Domain.Notes;
using System.Net.Mime;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Notes.CoreService.Exceptions;
using Notes.CoreService.Extensions;
using Notes.CoreService.DTO;

namespace Notes.CoreService.Controllers;

/// <summary>
/// Методы для работы с заметками
/// </summary>
[Route("[controller]")]
[Authorize]
[ApiController]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Создание заметки
    /// </summary>
    /// <response code="200">Возвращает данные о созданной заметки</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<CreateNoteResponse>> CreateUserNoteAsync([FromBody] CreateNoteRequest request)
    {
        var input = request.Adapt<CreateNoteInput>();
        input.UserId = HttpContext.GetUserId();
        var response = await _mediator.Send(new CreateNoteCommand { Input = input });
        return Ok(new CreateNoteResponse { Id = response.Id });
    }

    /// <summary>
    /// Обновление заметки
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> PatchNoteAsync(PatchNoteRequest request)
    {
        var currentUserId = HttpContext.GetUserId();
        await _mediator.Send(new PatchNoteCommand
            { ModifiedBy = currentUserId, Input = request.Adapt<PatchNoteInput>() });
        return Ok();
    }
}