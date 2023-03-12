using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;

namespace Notes.CoreService.Domain.Notes;

public class GetNotesQuery : IRequest<IReadOnlyCollection<Note>>
{
}

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, IReadOnlyCollection<Note>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public GetNotesQueryHandler(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyCollection<Note>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Notes.ToListAsync(cancellationToken: cancellationToken);
    }
}