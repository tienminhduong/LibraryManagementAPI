using CloudinaryDotNet.Actions;

namespace LibraryManagementAPI.Interfaces.IServices
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
