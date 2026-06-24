namespace SubmissionProcessor.Configuration;

public class ProcessingOptions
{
    public const string SectionName = "Processing";
    public int MaxAttempts { get; set; } = 3;
}
