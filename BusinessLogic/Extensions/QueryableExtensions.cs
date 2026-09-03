using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<TDestination>> ToPagedResultAsync<TEntity, TDestination>(
        this IQueryable<TEntity> query,
        int page,
        int pageSize,
        IMapper mapper,
        CancellationToken cancellationToken = default)
        where TDestination : class
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PagedResult<TDestination>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
            };
        }
    }
}
