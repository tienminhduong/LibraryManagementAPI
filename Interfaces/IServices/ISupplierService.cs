using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.Supplier;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface ISupplierService
    {
        Task<PagedResponse<SupplierDto>> GetAllSuppliersAsync(int pageNumber, int pageSize);
        Task<SupplierDto?> GetSupplierByIdAsync(Guid id);
        Task<SupplierDto> AddSupplierAsync(CreateSupplierDTO supplierDto);
        Task UpdateSupplierAsync(Guid id, UpdateSupplierDTO supplierDto);
        Task DeleteSupplierAsync(Guid id);
        Task<PagedResponse<SupplierDto>> SearchSuppliersAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
