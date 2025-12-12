using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    /// <summary>
    /// Background service that automatically updates borrow request statuses
    /// - Updates Borrowed requests to Overdue when past due date
    /// - Tracks BookCopy status to determine if book has been returned
    /// - Each BorrowRequest represents ONE book copy
    /// </summary>
    public class BorrowRequestStatusUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BorrowRequestStatusUpdaterService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public BorrowRequestStatusUpdaterService(
            IServiceProvider serviceProvider,
            ILogger<BorrowRequestStatusUpdaterService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BorrowRequestStatusUpdaterService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateBorrowRequestStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating borrow request statuses.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("BorrowRequestStatusUpdaterService is stopping.");
        }

        private async Task UpdateBorrowRequestStatuses()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

            var currentTime = DateTime.UtcNow;
            var updatedCount = 0;

            // Get all active borrow requests (Borrowed or Overdue)
            // Include BookCopy to check its status
            var activeRequests = await dbContext.BorrowRequests
                .Include(br => br.BookCopy)
                .Where(br => 
                    br.Status == BorrowRequestStatus.Borrowed || 
                    br.Status == BorrowRequestStatus.Overdue)
                .ToListAsync();

            _logger.LogDebug("Checking {Count} active borrow requests for status updates.", activeRequests.Count);

            foreach (var request in activeRequests)
            {
                var originalStatus = request.Status;
                
                // Skip if no book copy assigned yet
                if (!request.BookCopyId.HasValue || request.BookCopy == null)
                {
                    _logger.LogWarning(
                        "BorrowRequest {RequestId} has no BookCopyId assigned. Skipping.",
                        request.Id);
                    continue;
                }

                // Check BookCopy status
                var bookCopyStatus = request.BookCopy.status;

                // If BookCopy is Available, it means the book has been returned
                if (bookCopyStatus == Status.Available && request.ReturnedAt.HasValue)
                {
                    // Book has been returned - check if it was on time or overdue
                    var wasOverdue = request.DueDate.HasValue && 
                                    request.ReturnedAt.Value > request.DueDate.Value;

                    request.Status = wasOverdue 
                        ? BorrowRequestStatus.OverdueReturned 
                        : BorrowRequestStatus.Returned;

                    _logger.LogInformation(
                        "BorrowRequest {RequestId} marked as {Status} (BookCopy is Available)",
                        request.Id, request.Status);
                }
                // If BookCopy is still Borrowed
                else if (bookCopyStatus == Status.Borrowed)
                {
                    // Check if past due date
                    if (request.DueDate.HasValue && request.DueDate.Value < currentTime)
                    {
                        request.Status = BorrowRequestStatus.Overdue;
                    }
                    else
                    {
                        // Ensure status is Borrowed (not Overdue if we somehow passed the due date)
                        request.Status = BorrowRequestStatus.Borrowed;
                    }
                }
                // Handle other BookCopy statuses
                else if (bookCopyStatus == Status.Lost || bookCopyStatus == Status.Damaged)
                {
                    _logger.LogWarning(
                        "BorrowRequest {RequestId} has BookCopy with status {CopyStatus}. Manual intervention may be needed.",
                        request.Id, bookCopyStatus);
                    // Don't auto-update status for Lost or Damaged books
                    continue;
                }

                // Log if status changed
                if (originalStatus != request.Status)
                {
                    updatedCount++;
                    _logger.LogInformation(
                        "Updated BorrowRequest {RequestId} status from {OldStatus} to {NewStatus} (BookCopy: {CopyId}, CopyStatus: {CopyStatus})",
                        request.Id, originalStatus, request.Status, request.BookCopyId, bookCopyStatus);
                }
            }

            // Save changes if any updates were made
            if (updatedCount > 0)
            {
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} borrow request statuses based on BookCopy status.", updatedCount);
            }
            else
            {
                _logger.LogDebug("No borrow request status updates needed.");
            }
        }
    }
}
