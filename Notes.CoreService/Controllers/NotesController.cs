using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.Domain.Notes;
using System.Net.Mime;
using Notes.CoreService.Abstractions;

namespace Notes.CoreService.Controllers;

/// <summary>
/// Методы для работы с заметками
/// </summary>
[ApiController]
[Route("[controller]")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получение списка заметок
    /// </summary>
    /// <response code="200">Возвращает список заметок</response>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<Page<Note>>> GetNotesAsync([FromQuery] GetNotesInput input)
    {
        var response = await _mediator.Send(new GetNotesQuery { Input = input });
        return Ok(response);
    }

    /// <summary>
    /// Создание заметки
    /// </summary>
    /// <response code="200">Возвращает идентификатор созданной заметки</response>
    [HttpPost]
    [Produces(MediaTypeNames.Text.Plain)]
    public async Task<ActionResult<Guid>> CreateNoteAsync([FromBody] CreateNoteInput input)
    {
        var response = await _mediator.Send(new CreateNoteCommand { Input = input });
        return Ok(response);
    }
}