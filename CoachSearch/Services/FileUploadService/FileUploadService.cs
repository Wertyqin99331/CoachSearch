using Castle.DynamicProxy.Generators;

namespace CoachSearch.Services.FileUploadService;

public class FileUploadService: IFileUploadService
{
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public FileUploadService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
	{
		this._webHostEnvironment = webHostEnvironment;
		this._httpContextAccessor = httpContextAccessor;
	}

	public async Task<string> UploadFileAsync(IFormFile file)
	{
		var folderPath = Path.Combine(this._webHostEnvironment.WebRootPath, "Avatars");
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		var uniqueFileName = $"{file.Name}{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
		var filePath = Path.Combine(folderPath, uniqueFileName);
		if (File.Exists(filePath))
			File.Delete(filePath);

		await using var fileStream = File.Create(filePath);
		await file.CopyToAsync(fileStream);
		return uniqueFileName;
	}

	public void DeleteFile(string fileName)
	{
		var filePath = Path.Combine(this._webHostEnvironment.WebRootPath, "Avatars", fileName);
		
		if (File.Exists(filePath))
			File.Delete(filePath);
	}

	public string? GetAvatarUrl(string? fileName)
	{
		if (this._httpContextAccessor.HttpContext?.Request is null)
			return null;

		var request = this._httpContextAccessor.HttpContext.Request;
		
		return fileName == null
			? null
			: $"{request.Scheme}://{request.Host}{request.PathBase}/Avatars/{fileName}";
	}
}