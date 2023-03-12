using FluentValidation;
using System.ComponentModel;

namespace Notes.CoreService.DTO;

public class PageInput
{
    /// <summary>
    /// Номер страницы
    /// </summary>
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Размер страницы
    /// </summary>
    [DefaultValue(Constants.MaxPageSize)]
    public int PageSize { get; set; } = Constants.MaxPageSize;
}

public class PageInputValidator<T> : AbstractValidator<T>
    where T : PageInput
{
    public PageInputValidator() {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(0, Constants.MaxPageSize);
    }
}