using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.CoreService.DTO.Abstractions;
using System.Linq.Expressions;
using Mapster;
using Notes.CoreService.Enums;
using Microsoft.EntityFrameworkCore.Query;

namespace Notes.CoreService.Extensions;

public static class QueryableExtensions
{
    public static async Task<Page<T>> ToPageAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        ISpecification<T>? specification,
        CancellationToken cancellationToken
    )
        where T : class
    {
        var queryResult = specification == null
            ? source
            : SpecificationEvaluator.Default.GetQuery(source, specification);

        var count = await queryResult.CountAsync(cancellationToken);

        var items = await queryResult
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new Page<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<Page<TResult>> ToPageAsync<TValue, TResult>(
        this IQueryable<TValue> source,
        int pageNumber,
        int pageSize,
        ISpecification<TValue>? specification,
        CancellationToken cancellationToken
    )
        where TValue : class
    {
        var queryResult = specification == null
            ? source
            : SpecificationEvaluator.Default.GetQuery(source, specification);

        var count = await queryResult.CountAsync(cancellationToken);

        var items = await queryResult
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<TResult>()
            .ToListAsync(cancellationToken);

        return new Page<TResult>(items, count, pageNumber, pageSize);
    }

    public static void OrderBy<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression<Func<T, object?>> orderExpression,
        SortOrder orderBy)
    {
        var orderExpressions = (List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions;
        orderExpressions
            .Add(
                new OrderExpressionInfo<T>(orderExpression,
                    orderExpressions.Any()
                        ? orderBy == SortOrder.Desc
                            ? OrderTypeEnum.ThenByDescending
                            : OrderTypeEnum.ThenBy
                        : orderBy == SortOrder.Desc
                            ? OrderTypeEnum.OrderByDescending
                            : OrderTypeEnum.OrderBy));
    }

    public static SetPropertyCalls<TEntity> SetIf<TEntity, TProperty>(
        this SetPropertyCalls<TEntity> self,
        Func<TEntity, TProperty> propertyExpression,
        TProperty value,
        bool execute
    ) =>
        execute ? self.SetProperty(propertyExpression, value) : self;
}