namespace Flower.Backend.Models.DTOs;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<string> SkippedSkus { get; set; } = new();
}

public class ImportError
{
    public int RowIndex { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ImportViewModel
{
    public ImportResult? Result { get; set; }
}
