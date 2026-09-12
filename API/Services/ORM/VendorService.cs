using API.Data;
using API.Helpers;
using API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace API.Services.ORM
{
    public class VendorService
    {
        private readonly DataContext context;
        private readonly QueryHelper _queryHelper;
        public VendorService(DataContext context, QueryHelper queryHelper)
        {
            this.context = context;
            this._queryHelper = queryHelper;
        }

        public async Task<PagedResult<VendorModel>>GetVendorsPages(int pageNumber, int pageSize, string? search)
        {
            var query = context.VendorNonCommercials.Where(v => v.IsActive).Select(v => new VendorNonCommercialsModel { Name = v.Name, Code = v.Code }).Distinct();
            return await _queryHelper.GetPagedResultAsync(
                query,
                pageNumber,
                pageSize,
                searchExpression: !string.IsNullOrEmpty(search) ? v => EF.Functions.ILike(v.Name, $"%{search}%") : null,
                selector: v => new VendorModel
                {
                    Name = v.Name,
                    Code = v.Code,
                },
                orderBy: v => v.Code
            );
        }


        public async Task<PagedResult<VendorModel>> GetVendorsPages(int pageNumber, int pageSize, string? searchBy, string? keyword)
        {
            var query = context.VendorNonCommercials
                .Where(v => v.IsActive)
                .Select(v => new VendorNonCommercialsModel { Name = v.Name, Code = v.Code })
                .Distinct();

            Expression<Func<VendorNonCommercialsModel, bool>>? searchExpression = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                string searchColumn = searchBy ?? "";
                searchExpression = searchColumn.ToLower() switch
                {
                    "name" => v => EF.Functions.ILike(v.Name, $"%{keyword}%"),
                    "code" => v => EF.Functions.ILike(v.Code, $"%{keyword}%"),
                    _ => v => EF.Functions.ILike(v.Name, $"%{keyword}%")
                };
            }

            return await _queryHelper.GetPagedResultAsync(
                query,
                pageNumber,
                pageSize,
                searchExpression,
                selector: v => new VendorModel
                {
                    Name = v.Name,
                    Code = v.Code,
                },
                orderBy: v => v.Code

            );
        }


    }
}
