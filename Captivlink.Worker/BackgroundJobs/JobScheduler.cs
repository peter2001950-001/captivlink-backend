using System.Runtime.InteropServices;
using NCrontab;

namespace Captivlink.Worker.BackgroundJobs
{
    public class JobScheduler<T> : BackgroundService where T : IScheduledJob
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private T _job;

        private string Schedule => "*/10 * * * * *"; //Runs every 10 seconds

        public JobScheduler(T job)
        {
            _job = job;
            _schedule = CrontabSchedule.Parse(_job.CronExpression,
                    new CrontabSchedule.ParseOptions {IncludingSeconds = true});
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            do
            {
                var now = DateTime.Now;
                var nextrun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task Process()
        {
            await _job.Execute();
        }
    }
}
