using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScreenShotService
{     
    public class RetrieverWorker : BackgroundService
    {
        private readonly ILogger<RetrieverWorker> _logger;
        private string fileDirectory;
        private string processedDirectory;
        private string imagesDirectory;
        private string filePattern;
        private int threadCount;
        private string dbConfig;

        public RetrieverWorker(ILogger<RetrieverWorker> logger)
        {
            _logger = logger;
            AssignConfigVariables();
        }

        private void AssignConfigVariables()
        {
            var cnf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables().Build();

            fileDirectory = cnf.GetSection("OutputFilesFolder").Value.ToString();
            filePattern = cnf.GetSection("OutputFilePattern").Value.ToString();
            threadCount = int.Parse(cnf.GetSection("RetrieverThreadCount").Value.ToString());
            processedDirectory = cnf.GetSection("ProcessedFilesFolder").Value.ToString();
            imagesDirectory = cnf.GetSection("OutputImagesFolder").Value.ToString();            
            dbConfig = cnf.GetSection("MongoDB").Value.ToString();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Second == 0)
                    _logger.LogInformation("RetrieverWorker running at: {time}", DateTimeOffset.Now);
                try
                {
                    RetrieverQueue rq = new RetrieverQueue(threadCount, dbConfig, _logger, imagesDirectory);
                    IOUtil ioUtil = new IOUtil();

                    string[] files = ioUtil.GetFiles(fileDirectory, filePattern);

                    foreach (var file in files)
                    {
                        string[] lines = ioUtil.GetFileLines(file);

                        foreach (var line in lines)
                        {
                            if (line.StartsWith("https://") || line.StartsWith("http://"))
                                rq.Enqueue(new RetrieverJob { job = line.ToString(), type = 1 });
                            else
                                rq.Enqueue(new RetrieverJob { job = line.ToString(), type = 2 });
                        }

                        ioUtil.MoveFileToDirectory(file, processedDirectory);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

}
