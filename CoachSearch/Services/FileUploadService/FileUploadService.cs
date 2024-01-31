namespace CoachSearch.Services.FileUploadService;

public class FileUploadService: IFileUploadService
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	public FileUploadService(IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
	}

	public async Task<string> UploadFileAsync(IFormFile file)
	{
		var folderPath = Path.Combine(this._webHostEnvironment.ContentRootPath, "Images");
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		var uniqueFileName = $"{file.Name}{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
		var filePath = Path.Combine(folderPath, uniqueFileName);
		if (File.Exists(filePath))
			File.Delete(filePath);

		await using var fileStream = new FileStream(filePath, FileMode.Create);
		await file.CopyToAsync(fileStream);
		return uniqueFileName;
	}

	public void DeleteFile(string fileName)
	{
		var filePath = Path.Combine(this._webHostEnvironment.ContentRootPath, "Images", fileName);
		
		if (File.Exists(filePath))
			File.Delete(filePath);
	}

	public string? GetAvatarUrl(HttpRequest request, string? fileName)
	{
		return fileName == null
			? null
			: $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{fileName}";
	}
}