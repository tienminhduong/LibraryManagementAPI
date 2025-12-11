using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories
{
    public class SupplierRepository(LibraryDbContext dbContext) : ISupplierRepository
    {
        public async Task AddSupplierAsync(Supplier supplier)
        {
            await dbContext.Set<Supplier>().AddAsync(supplier);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(Guid id)
        {
            var supplier = await dbContext.Set<Supplier>().FindAsync(id);
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), "Supplier not found");
            }
            dbContext.Set<Supplier>().Remove(supplier);
            await dbContext.SaveChangesAsync();
        }

        public async Task<PagedResponse<Supplier>> GetAllSuppliersAsync(int pageNumber, int pageSize)
        {
            var suppliers = dbContext.Set<Supplier>()
                .OrderBy(s => s.name)
                .AsQueryable()
                .AsNoTracking();
            
            return await PagedResponse<Supplier>.FromQueryable(suppliers, pageNumber, pageSize);
        }

        public async Task<Supplier?> GetSupplierByIdAsync(Guid id)
        {
            return await dbContext.Set<Supplier>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.id == id);
        }

        public async Task<PagedResponse<Supplier>> SearchSuppliersAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var normalizedSearch = searchTerm.ToLower().Trim();

            var suppliers = dbContext.Set<Supplier>()
                .Where(s =>
                    (s.name != null && s.name.ToLower().Contains(normalizedSearch)) ||
                    (s.email != null && s.email.ToLower().Contains(normalizedSearch)) ||
                    (s.phoneNumber != null && s.phoneNumber.ToLower().Contains(normalizedSearch)) ||
                    (s.address != null && s.address.ToLower().Contains(normalizedSearch)))
                .OrderBy(s => s.name)
                .AsQueryable()
                .AsNoTracking();

            return await PagedResponse<Supplier>.FromQueryable(suppliers, pageNumber, pageSize);
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await dbContext.Set<Supplier>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.id == supplier.id);

            if (existingSupplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), "Supplier not found");
            }

            dbContext.Set<Supplier>().Update(supplier);
            await dbContext.SaveChangesAsync();
        }
    }
}
