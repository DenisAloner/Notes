using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.Domain.Notes;
using System.Net;
using System.Net.Mime;
using Swashbuckle.AspNetCore.Annotations;

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
    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK, null, typeof(IReadOnlyCollection<Note>), MediaTypeNames.Application.Json)]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, typeof(string), MediaTypeNames.Text.Plain)]
    public async Task<IActionResult> GetNotesAsync()
    {
        var response = await _mediator.Send(new GetNotesQuery());
        return Ok(response);
    }

    /// <summary>
    /// Создание заметки
    /// </summary>
    [HttpPost]
    [SwaggerResponse((int)HttpStatusCode.OK, null, typeof(Guid), MediaTypeNames.Text.Plain)]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, null, typeof(string), MediaTypeNames.Text.Plain)]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, typeof(string), MediaTypeNames.Text.Plain)]
    public async Task<IActionResult> CreateNoteAsync([FromBody] CreateNoteInput input)
    {
        var response = await _mediator.Send(new CreateNoteCommand { Input = input });
        return Ok(response);
    }
}