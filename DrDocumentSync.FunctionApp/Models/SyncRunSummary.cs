namespace DrDocumentSync.FunctionApp.Models;

public sealed class SyncRunSummary
{
    public int TotalProcessed { get; set; }
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public List<string> Errors { get; } = [];
}
