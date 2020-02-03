using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using ScreenShotDAL;
using ScreenShotDAL.Entity;
using ScreenShotter;

namespace ScreenShotService
{
    public class RetrieverJob
    {
        public string job { get; set; }
        public int type { get; set; }
    }
   public class RetrieverQueue
    {
        private readonly ILogger<RetrieverWorker> _logger;
        BlockingCollection<RetrieverJob> _jobs = new BlockingCollection<RetrieverJob>();
        private string[] _dbConfig;
        private string _imagesDirectory;

        public RetrieverQueue(int numThreads, string dbConfig, ILogger<RetrieverWorker> logger, string imagesDirectory)
        {
            _dbConfig = dbConfig.Split(';');
            _logger = logger;
            _imagesDirectory = imagesDirectory;

            for (int i = 0; i < numThreads; i++)
            {
                var thread = new Thread(OnHandlerStart)
                { IsBackground = true };
                thread.Start();
            }
        }

        public void Enqueue(RetrieverJob rj)
        {
            if (!_jobs.IsAddingCompleted)
            {
                _jobs.Add(rj);
            }
        }

        public void Stop()
        {
            _jobs.CompleteAdding();
        }

        private void OnHandlerStart()
        {
            foreach (var rJob in _jobs.GetConsumingEnumerable(CancellationToken.None))
            {
                _logger.LogInformation("Processing job: " + rJob.job);

                ScreenShotSaver ssSaver = new ScreenShotSaver(_dbConfig[0], _dbConfig[1], _dbConfig[2]);
                IOUtil ioUtil = new IOUtil();

                try
                {
                    List<ScreenShotData> result;

                    if (rJob.type == 1)
                    {
                       result = ssSaver.GetByUrl(rJob.job);                       
                    }
                    else
                    {
                        string minDateStr = rJob.job.Substring(0, rJob.job.IndexOf('-', 10));
                        string maxDateStr = rJob.job.Substring(minDateStr.Length + 1);
                        DateTime minDate = DateTime.ParseExact(minDateStr, "yyyy-MM-dd HH:mm", null);
                        DateTime maxDate = DateTime.ParseExact(maxDateStr, "yyyy-MM-dd HH:mm", null);

                        result = ssSaver.GetByDateInterval(minDate, maxDate);
                    }

                    if (result.Count > 0)
                    {
                        string outputFolder = ioUtil.CleanInputAndCreteDirectory(_imagesDirectory, rJob.job);
                        Shotter shotter = new Shotter();
                        foreach (var item in result)
                        {
                            shotter.SaveAsFile(outputFolder + "/" + item.Id.ToString() + ".png", item.ContentImage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                 
            }
        }
    }
}

