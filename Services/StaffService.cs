using AutoMapper;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Info;
using LibraryManagementAPI.Models.Pagination;

namespace LibraryManagementAPI.Services;

public class StaffService(IStaffRepository staffRepository, IMapper mapper) : IStaffService
{
    public async Task<StaffInfoDto> GetStaffInfoAsync(Guid id)
    {
        var staffInfo =  await staffRepository.GetStaffInfoAsync(id);
        return mapper.Map<StaffInfoDto>(staffInfo);
    }

    public async Task<PagedResponse<StaffInfoDto>> SearchStaffsAsync(string? nameContains = null, string? emailContains = null, string? phoneContains = null,
        DateTime? hiredFrom = null, DateTime? hiredTo = null, DateTime? hiredDate = null, int pageNumber = 1,
        int pageSize = 20)
    {
        var pagedStaffs = await staffRepository.SearchStaffsAsync(nameContains, emailContains, phoneContains,
            hiredFrom, hiredTo, hiredDate, pageNumber, pageSize);
        return PagedResponse<StaffInfoDto>.MapFrom(pagedStaffs, mapper);
    }
}