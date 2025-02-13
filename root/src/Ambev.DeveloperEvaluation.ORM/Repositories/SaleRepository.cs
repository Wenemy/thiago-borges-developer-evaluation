using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(o=> o.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .FirstOrDefaultAsync(u => u.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        sale.Cancel();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    private IQueryable<Sale> ApplyFilter(int page, int pageSize, Dictionary<string, string> filters)
    {
        var query = _context.Sales.Include(q => q.Items).AsQueryable();

        foreach (var filter in filters)
        {
            var field = filter.Key.ToLower();
            var value = filter.Value.ToLower();

            if (field == "productid")
            {
                if (Guid.TryParse(value, out var productId))
                    query = query.Where(sale => sale.Items.Any(item => item.ProductId == productId));
            }
            else if (field == "salenumber")
            {
                query = query.Where(sale => sale.SaleNumber.Contains(value));
            }
            else if (field == "date")
            {
                if (DateTime.TryParse(value, out var date))
                    query = query.Where(sale => sale.SaleDate == date);
            }

            else if (field.EndsWith("_min"))
            {
                var fieldName = field.Substring(0, field.Length - 4);
                if (decimal.TryParse(value, out var minValue))
                    query = query.Where(sale => EF.Property<decimal>(sale, fieldName) >= minValue);
            }
            else if (field.EndsWith("_max"))
            {
                var fieldName = field.Substring(0, field.Length - 4);
                if (decimal.TryParse(value, out var maxValue))
                    query = query.Where(sale => EF.Property<decimal>(sale, fieldName) <= maxValue);
            }
        }

        return query;
    }

    public async Task<PagedResult<Sale>> GetPagedSalesAsync(int page, int pageSize, Dictionary<string, string> filters, CancellationToken cancellationToken)
    {
        var query = ApplyFilter(page, pageSize, filters);

        var totalCount = await query.CountAsync(cancellationToken);
        var sales = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>(sales, page, pageSize, totalCount);
    }
}
