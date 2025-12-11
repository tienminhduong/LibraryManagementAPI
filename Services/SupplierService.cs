using AutoMapper;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Pagination;
using LibraryManagementAPI.Models.Supplier;

namespace LibraryManagementAPI.Services
{
    public class SupplierService(ISupplierRepository supplierRepository, IMapper mapper) : ISupplierService
    {
        public async Task<SupplierDTO> AddSupplierAsync(CreateSupplierDTO supplierDto)
        {
            if (supplierDto == null)
            {
                throw new ArgumentNullException(nameof(supplierDto), "Supplier data is null");
            }

            var supplier = mapper.Map<Supplier>(supplierDto);
            supplier.id = Guid.NewGuid();
            await supplierRepository.AddSupplierAsync(supplier);
            return mapper.Map<SupplierDTO>(supplier);
        }

        public async Task DeleteSupplierAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid supplier ID", nameof(id));
            }
            await supplierRepository.DeleteSupplierAsync(id);
        }

        public async Task<PagedResponse<SupplierDTO>> GetAllSuppliersAsync(int pageNumber, int pageSize)
        {
            var pagedSuppliers = await supplierRepository.GetAllSuppliersAsync(pageNumber, pageSize);
            var supplierDtos = PagedResponse<SupplierDTO>.MapFrom(pagedSuppliers, mapper);
            return supplierDtos;
        }

        public async Task<SupplierDTO?> GetSupplierByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid supplier ID", nameof(id));
            }

            var supplier = await supplierRepository.GetSupplierByIdAsync(id);
            return mapper.Map<SupplierDTO>(supplier);
        }

        public async Task<PagedResponse<SupplierDTO>> SearchSuppliersAsync(string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
            }

            var pagedSuppliers = await supplierRepository.SearchSuppliersAsync(searchTerm, pageNumber, pageSize);
            var supplierDtos = PagedResponse<SupplierDTO>.MapFrom(pagedSuppliers, mapper);
            return supplierDtos;
        }

        public async Task UpdateSupplierAsync(Guid id, UpdateSupplierDTO supplierDto)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid supplier ID", nameof(id));
            }

            if (supplierDto == null)
            {
                throw new ArgumentNullException(nameof(supplierDto), "Supplier data is null");
            }

            var supplier = mapper.Map<Supplier>(supplierDto);
            supplier.id = id;
            await supplierRepository.UpdateSupplierAsync(supplier);
        }
    }
}
