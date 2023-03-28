using Microsoft.AspNetCore.Mvc;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Enums;

namespace Notes.CoreService.DTO;

/// <summary>
/// Данные для получения списка заметок пользователя
/// </summary>
public class GetNotesInput : PageInput
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    [ModelBinder(BinderType = typeof(SortParametersConverterModelBinder<NoteSortField>))]
    public IReadOnlyList<SortParameter<NoteSortField>>? Sort { get; set; }
}

public class GetNotesInputValidator : PageInputValidator<GetNotesInput>
{
}