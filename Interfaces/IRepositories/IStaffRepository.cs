using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IRepositories;

public interface IStaffRepository
{
    Task<StaffInfo> GetStaffInfoAsync(Guid id);
    Task<PagedResponse<StaffInfo>> SearchStaffsAsync(
        string? nameContains = null,
        string? emailContains = null,
        string? phoneContains = null,
        DateTime? hiredFrom = null,
        DateTime? hiredTo = null,
        DateTime? hiredDate = null,
        int pageNumber = 1,
        int pageSize = 20
    );
}