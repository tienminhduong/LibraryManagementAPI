using LibraryManagementAPI.Models.Info;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Interfaces.IServices;

public interface IStaffService
{
    Task<StaffInfoDto> GetStaffInfoAsync(Guid id);
    Task<PagedResponse<StaffInfoDto>> SearchStaffsAsync(string? nameContains = null, string? emailContains = null, string? phoneContains = null,
        DateTime? hiredFrom = null, DateTime? hiredTo = null, DateTime? hiredDate = null, int pageNumber = 1,
        int pageSize = 20);
}