using LibraryManagementAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Context;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AdminInfo> AdminInfos { get; set; }
    public DbSet<StaffInfo> StaffInfos { get; set; }
    public DbSet<MemberInfo> MemberInfos { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<BookImport> BookImports { get; set; }
    public DbSet<BookImportDetail> BookImportDetails { get; set; }
    public DbSet<BookTransaction> BookTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasMany(b => b.BookCategories)
            .WithMany(c => c.Books)
            .UsingEntity(j => j.ToTable("BookCategoryMap"));

        // Configure info and account relationship
        modelBuilder.Entity<BaseInfo>()
            .HasOne<Account>()
            .WithOne(a => a.info)
            .HasForeignKey<BaseInfo>(bi => bi.loginId)
            .IsRequired();

        modelBuilder.Entity<Account>()
            .HasOne(account => account.info)
            .WithOne(info => info.account)
            .IsRequired(false);

        modelBuilder.Entity<BaseInfo>().UseTpcMappingStrategy();
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed initial data if necessary
        // Seed Accounts
        // Khai báo GUID tĩnh để dễ quản lý và không thay đổi giữa các lần chạy
        var adminGuid = Guid.Parse("A0000000-0000-0000-0000-000000000001");
        var adminGuid1 = Guid.Parse("A0000000-0000-0000-0000-000000000011");
        var staffGuid = Guid.Parse("B0000000-0000-0000-0000-000000000002");
        var memberGuid = Guid.Parse("C0000000-0000-0000-0000-000000000003");

        // Create 3 GUID for 3 infos
        var adminGuidInfo = Guid.Parse("A0000000-0000-0000-0000-000000000011");
        var adminGuidInfo1 = Guid.Parse("A0000000-0000-0000-0000-000000000111");
        var staffGuidInfo = Guid.Parse("B0000000-0000-0000-0000-000000000022");
        var memberGuidInfo = Guid.Parse("C0000000-0000-0000-0000-000000000033");


        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                id = adminGuid, // <<< PHẢI CÓ
                userName = "admin",
                passwordHash = "hashed_password",
                role = Role.Admin,
                createdAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                lastLogin = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Account
            {
                id = adminGuid1, // <<< PHẢI CÓ
                userName = "admin",
                passwordHash = "hashed_password1",
                role = Role.Admin,
                createdAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                lastLogin = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Account
            {
                id = staffGuid, // <<< PHẢI CÓ
                userName = "staff",
                passwordHash = "hashed_password",
                role = Role.Staff,
                createdAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                lastLogin = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Account
            {
                id = memberGuid, // <<< PHẢI CÓ
                userName = "member",
                passwordHash = "hashed_password",
                role = Role.Member,
                createdAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                lastLogin = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        modelBuilder.Entity<AdminInfo>().HasData(
            // Seed AdminInfo

            new AdminInfo
            {
                id = adminGuidInfo,
                fullName = "Admin",
                email = "admin@gmail.com",
                phoneNumber = "0321547895",
                loginId = adminGuid
            },
            new AdminInfo
            {
                id = adminGuidInfo1,
                fullName = "Admin",
                email = "admin@gmail.com",
                phoneNumber = "0321547895",
                loginId = adminGuid1
            });
        modelBuilder.Entity<StaffInfo>().HasData(
            new StaffInfo
            {
                id = staffGuidInfo,
                fullName = "Staff",
                email = "staff@gmail.com",
                phoneNumber = "0321547896",
                loginId = staffGuid,
                hireDate = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            });
        modelBuilder.Entity<MemberInfo>().HasData(
            new MemberInfo
            {
                id = memberGuidInfo,
                fullName = "Member",
                email = "member@gmail.com",
                phoneNumber = "0321547897",
                loginId = memberGuid,
                address = "123 Main St",
                imageUrl = "http://example.com/image.jpg",
                joinDate = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            });

        // seed data for supplier, book import, book import detail, book copy

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier
            {
                id = Guid.Parse("D0000000-0000-0000-0000-000000000004"),
                name = "Default Supplier",
                email = "",
                phoneNumber = "",
                address = ""
            }
            );
        modelBuilder.Entity<BookImport>().HasData(
            new BookImport
            {
                id = Guid.Parse("E0000000-0000-0000-0000-000000000005"),
                supplierId = Guid.Parse("D0000000-0000-0000-0000-000000000004"),
                staffId = Guid.Parse("019aa216-657c-7b6d-abd5-6db1b06317ee"),
                importDate = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                totalAmount = 0,
                note = "First Import"
            },

            new BookImport
            {
                id = Guid.Parse("E0000000-0000-0000-0000-000000000006"),
                supplierId = Guid.Parse("D0000000-0000-0000-0000-000000000004"),
                staffId = Guid.Parse("019aa216-657c-7b6d-abd5-6db1b06317ee"),
                importDate = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                totalAmount = 0,
                note = "Second Import"
            }
            );
        modelBuilder.Entity<BookImportDetail>().HasData(
            new BookImportDetail
            {
                id = Guid.Parse("F0000000-0000-0000-0000-000000000007"),
                bookImportId = Guid.Parse("E0000000-0000-0000-0000-000000000005"),
                bookId = Guid.Parse("019a1a78-7b67-7f6c-9d63-f2d13554c669"),
                quantity = 10,
                unitPrice = 100m
            },
            new BookImportDetail
            {
                id = Guid.Parse("F0000000-0000-0000-0000-000000000008"),
                bookImportId = Guid.Parse("E0000000-0000-0000-0000-000000000006"),
                bookId = Guid.Parse("019a1a78-7b67-7f6c-9d63-f2d13554c669"),
                quantity = 5,
                unitPrice = 200m
            }
            );
        modelBuilder.Entity<BookCopy>().HasData(
            // 10 copies from first import
            Enumerable.Range(1, 10).Select(i => new BookCopy
            {
                id = Guid.Parse((10000000 + i).ToString("X8") + "-0000-0000-0000-000000000009"),
                bookId = Guid.Parse("019a1a78-7b67-7f6c-9d63-f2d13554c669"),
                bookImportDetailId = Guid.Parse("F0000000-0000-0000-0000-000000000007"),
                status = Status.Available
            })
        );
    }
}
