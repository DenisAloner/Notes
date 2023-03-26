using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Extensions;

namespace Notes.CoreService.Repositories;

public interface IRepository<T>
{ 
    Task<Page<TResult>> GetAllAsync<TResult>(
        int skip,
        int take,
        ISpecification<T>? specification,
        CancellationToken cancellationToken
    );
}

public class NoteRepository : IRepository<Note>
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public NoteRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Page<T>> GetAllAsync<T>(int pageNumber, int pageSize, ISpecification<Note>? specification,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Notes.ToPageAsync<Note, T>(pageNumber, pageSize, specification, cancellationToken);
    }
}