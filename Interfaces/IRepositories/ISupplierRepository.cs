using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface ISupplierRepository
    {
        Task<PagedResponse<Supplier>> GetAllSuppliersAsync(int pageNumber, int pageSize);
        Task<Supplier?> GetSupplierByIdAsync(Guid id);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(Guid id);
        Task AddSupplierAsync(Supplier supplier);
        Task<PagedResponse<Supplier>> SearchSuppliersAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
