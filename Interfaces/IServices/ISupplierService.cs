using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.Supplier;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface ISupplierService
    {
        Task<PagedResponse<SupplierDTO>> GetAllSuppliersAsync(int pageNumber, int pageSize);
        Task<SupplierDTO?> GetSupplierByIdAsync(Guid id);
        Task<SupplierDTO> AddSupplierAsync(CreateSupplierDTO supplierDto);
        Task UpdateSupplierAsync(Guid id, UpdateSupplierDTO supplierDto);
        Task DeleteSupplierAsync(Guid id);
        Task<PagedResponse<SupplierDTO>> SearchSuppliersAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
