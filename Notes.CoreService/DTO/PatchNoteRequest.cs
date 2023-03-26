using Microsoft.AspNetCore.Mvc;
using Notes.CoreService.DTO.Abstractions;

namespace Notes.CoreService.DTO;

/// <summary>
/// Данные для обновления заметки
/// </summary>
public class PatchNoteRequest
{
    public class BodyDto
    {
        /// <summary>
        /// Название заметки
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Описание заметки
        /// </summary>
        public Optional<string> Description { get; set; }
    }

    /// <summary>
    /// Идентификатор заметки
    /// </summary>
    [FromRoute]
    public Guid Id { get; set; }

    /// <summary>
    /// Данные
    /// </summary>
    [FromBody]
    public BodyDto Body { get; set; }
}