namespace Captivlink.Worker.BackgroundJobs
{
    public interface IScheduledJob
    {
        Task Execute();
        string CronExpression { get; }
    }
}
