namespace CoachSearch.Services.FileUploadService;

public class FileUploadService(IWebHostEnvironment webHostEnvironment): IFileUploadService
{
	public async Task<string> UploadFileAsync(IFormFile file)
	{
		var folderPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images");
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
		var filePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", fileName);
		
		if (File.Exists(filePath))
			File.Delete(filePath);
	}
}