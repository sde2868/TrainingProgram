namespace TraineeManagement.Constants;
public static class CacheKeys
{
    public static string Trainee(int id)
        => $"trainee:{id}";

    public static string TaskAssignment(int id)
        => $"task-assignment:{id}";

    public static string TaskSubmission(int id)
        => $"task-submission:{id}";
}