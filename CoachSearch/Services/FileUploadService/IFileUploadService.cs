namespace CoachSearch.Services.FileUploadService;

public interface IFileUploadService
{
	Task<string> UploadFileAsync(IFormFile file);
	void DeleteFile(string fileName);
}