using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class FileService
{
    public List<UploadedFile> UploadedFiles { get; set; } = new();
    public List<List<string>> CsvRows { get; set; } = new();
    public List<string> CsvHeaders { get; set; } = new();

    public async Task StoreFile(IBrowserFile browserFile)
    {
        var filePath = Path.GetTempFileName();
        using var stream = browserFile.OpenReadStream(5000 * 1024);
        using var destinationStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(destinationStream);

        UploadedFiles.Add(new UploadedFile
        {
            FileName = browserFile.Name,
            FileSize = browserFile.Size,
            FilePath = filePath
        });
    }

    public async Task ReadCsvFile(UploadedFile file)
    {
        CsvRows.Clear();
        CsvHeaders.Clear();

        if (File.Exists(file.FilePath))
        {
            var lines = await File.ReadAllLinesAsync(file.FilePath);
            if (lines.Length > 0)
            {
                CsvHeaders = lines[0].Split(',').ToList();
                CsvRows = lines.Skip(1).Select(line => line.Split(',').ToList()).ToList();
            }
        }
    }
}

public class UploadedFile
{
    public string FileName { get; set; } = "";
    public long FileSize { get; set; }
    public string FilePath { get; set; } = "";
}
