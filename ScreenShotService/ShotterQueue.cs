using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using ScreenShotDAL;
using ScreenShotDAL.Entity;
using ScreenShotter;

namespace ScreenShotService
{
    public class ShotterQueue
    {
        private readonly ILogger<ShotterWorker> _logger;
        BlockingCollection<string> _jobs = new BlockingCollection<string>();
        private string _windowSize;
        private string[] _dbConfig;

        public ShotterQueue(int numThreads, string dbConfig, string windowSize, ILogger<ShotterWorker> logger)
        {
            _windowSize = windowSize;
            _dbConfig = dbConfig.Split(';');
            _logger = logger;

            for (int i = 0; i < numThreads; i++)
            {
                var thread = new Thread(OnHandlerStart)
                { IsBackground = true };
                thread.Start();
            }
        }

        public void Enqueue(string job)
        {
            if (!_jobs.IsAddingCompleted)
            {
                _jobs.Add(job);
            }
        }

        public void Stop()
        {
            _jobs.CompleteAdding();
        }

        private void OnHandlerStart()
        {
            foreach (var job in _jobs.GetConsumingEnumerable(CancellationToken.None))
            {
                if (job.StartsWith("http://www") || job.StartsWith("https://www"))
                {
                    _logger.LogInformation("Processing job: " + job);
                    Shotter shotter = new Shotter(_windowSize);

                    ScreenShotSaver ssSaver = new ScreenShotSaver(_dbConfig[0], _dbConfig[1], _dbConfig[2]);

                    try
                    {
                        ScreenShotData ssData = new ScreenShotData
                        {
                            Url = job,
                            Date = DateTime.UtcNow,
                            ContentImage = shotter.TakeScreenshot(job)
                        };
                        ssSaver.Create(ssData);
                        _logger.LogInformation("Successfully imported job: " + job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                    finally
                    {
                        shotter.Dispose();
                    }
                }
            }
        }
    }
}
