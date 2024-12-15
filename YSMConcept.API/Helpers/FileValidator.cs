using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YSMConcept.API.Helpers
{
    public class FileValidator
    {
        public readonly FileSettings _fileSettings;
        public readonly ILogger<FileValidator> _logger;

        public FileValidator(
            IOptions<FileSettings> fileSettings,
            ILogger<FileValidator> logger)
        {
            _fileSettings = fileSettings.Value;
            _logger = logger;
        }

        public string? ValidateFile(IFormFile? file)
        {
            if (file == null)
                return null; 

            if (IsValidSize(file.Length))
            {
                _logger.LogError("File size exceeds the maximum allowed size.");
                _logger.LogInformation("File length: " + file.Length);
                _logger.LogInformation("File max size: " + _fileSettings.MaxSize);
                return $"File size exceeds the maximum allowed size of {_fileSettings.MaxSize} bytes.";
            }
            if (IsValidFile(file.FileName))
            {
                _logger.LogError("Unsupported file type.");
                return $"Unsupported file type. Allowed types are: {string.Join(", ", _fileSettings.AllowedTypes)}.";
            }
            return null; 
        }

        public List<string> ValidateFiles(List<IFormFile>? files)
        {
            var errors = new List<string>();

            if (files == null)
                return errors; // No files, no errors

            foreach (var file in files)
            {
                var error = ValidateFile(file);
                if (error != null)
                {
                    errors.Add(error);
                }
            }
            return errors;
        }

        private bool IsValidSize(long fileSize) => fileSize > _fileSettings.MaxSize;
        private bool IsValidFile(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName)?.ToLower();
            return !_fileSettings.AllowedTypes.Contains(fileExtension);
        }
    }
}
