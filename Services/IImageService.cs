namespace BiyLineApi.Services;
public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile imageFile, string folderName);
}
