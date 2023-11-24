namespace BiyLineApi.Services;
public sealed class ImageService : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment ??
            throw new ArgumentNullException(nameof(webHostEnvironment));
    }

    public async Task<string> UploadImageAsync(IFormFile imageFile, string folderName)
    {
        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

        Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

        var imagePath = Path.Combine(uploadPath, fileName);

        using var stream = new FileStream(imagePath, FileMode.Create);

        await imageFile.CopyToAsync(stream);

        var imageUri = $"/{Path.Combine(folderName, fileName).Replace(Path.DirectorySeparatorChar, '/')}";

        return imageUri;
    }
}