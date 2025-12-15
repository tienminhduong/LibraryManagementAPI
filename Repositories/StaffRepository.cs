using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Models.Info;
using LibraryManagementAPI.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories;

public class StaffRepository(LibraryDbContext dbContext) : IStaffRepository
{
    public async Task<StaffInfo> GetStaffInfoAsync(Guid id)
    {
        var staffInfo = await dbContext.StaffInfos.FindAsync(id);
        return staffInfo ?? throw new KeyNotFoundException($"Staff with ID {id} not found.");
    }

    public async Task<PagedResponse<StaffInfo>> SearchStaffsAsync(string? nameContains = null, string? emailContains = null, string? phoneContains = null,
        DateTime? hiredFrom = null, DateTime? hiredTo = null, DateTime? hiredDate = null, int pageNumber = 1,
        int pageSize = 20)
    {
        var query = dbContext.StaffInfos
            .AsNoTracking()
            .AsSplitQuery();
        
        if (nameContains != null)
            query = query.Where(s => EF.Functions.ILike(s.fullName!, $"%{nameContains}%"));
        
        if (emailContains != null)
            query = query.Where(s => EF.Functions.ILike(s.email!, $"%{emailContains}%"));
        
        if (phoneContains != null)
            query = query.Where(s => EF.Functions.ILike(s.phoneNumber!, $"%{phoneContains}%"));
        
        if (hiredFrom != null)
            query = query.Where(s => s.hireDate >= hiredFrom);
        
        if (hiredTo != null)
            query = query.Where(s => s.hireDate <= hiredTo);
        
        if (hiredDate != null)
            query = query.Where(s => s.hireDate.Date == hiredDate.Value.Date);
        
        query = query.OrderBy(s => s.hireDate);
        
        return await PagedResponse<StaffInfo>.FromQueryable(query, pageNumber, pageSize);
    }
}