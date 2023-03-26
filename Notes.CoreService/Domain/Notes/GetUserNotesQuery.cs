using Ardalis.Specification;
using FluentValidation;
using MediatR;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.DTO;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Enums;
using Notes.CoreService.Repositories;
using Notes.CoreService.Extensions;

namespace Notes.CoreService.Domain.Notes;

public class GetUserNotesQuery : IRequest<Page<UserNote>>
{
    public required Guid UserId { get; set; }
    public required GetNotesInput Input { get; set; }
}

public class GetNotesQueryValidator : AbstractValidator<GetUserNotesQuery>
{
    public GetNotesQueryValidator(GetNotesInputValidator getNotesInputValidator)
    {
        RuleFor(x => x.Input)
            .SetValidator(getNotesInputValidator);
    }
}

public class GetNotesQueryHandler : IRequestHandler<GetUserNotesQuery, Page<UserNote>>
{
    private readonly IRepository<Note> _repository;

    public GetNotesQueryHandler(IRepository<Note> repository)
    {
        _repository = repository;
    }

    public Task<Page<UserNote>> Handle(GetUserNotesQuery query, CancellationToken cancellationToken)
    {
        ISpecification<Note> specification = new NoteFilterSpec(query);
        return _repository.GetAllAsync<UserNote>(query.Input.PageNumber, query.Input.PageSize, specification, cancellationToken);
    }
}

public sealed class NoteFilterSpec : Specification<Note>
{
    public NoteFilterSpec(GetUserNotesQuery query)
    { 
        Query.Where(x => x.UserId == query.UserId);

        var input = query.Input;
        if (input.Title != null) Query.Where(x => x.Title.Contains(input.Title));
        if (input.Description != null)
            Query.Where(x => x.Description != null && x.Description.Contains(input.Description));
        if (input.Sort == null) return;
        foreach (var value in input.Sort.Values)
            switch (value.Field)
            {
                case NoteSortField.Title:
                {
                    Query.OrderBy(x => x.Title, value.Order);
                }
                    break;
                case NoteSortField.Description:
                {
                    Query.OrderBy(x => x.Description, value.Order);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}