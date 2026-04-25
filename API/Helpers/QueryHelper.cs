using API.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Helpers
{
    public class QueryHelper
    {
        public async Task<PagedResult<TModel>> GetPagedResultAsync<TEntity, TModel>(
            IQueryable<TEntity> query,
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? searchExpression,
            Expression<Func<TEntity, TModel>> selector,
            Expression<Func<TEntity, object>> orderBy)
            where TEntity: class
        {
            if(searchExpression != null)
            {
                query = query.Where(searchExpression);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderBy(orderBy)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync();

            return new PagedResult<TModel>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                SearchTerm = searchExpression != null ? searchExpression.ToString() : null
            };
        }
    }
}
