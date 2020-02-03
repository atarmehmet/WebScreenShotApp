using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScreenShotService
{
    public class ShotterWorker : BackgroundService
    {
        private readonly ILogger<ShotterWorker> _logger;
        private string fileDirectory;
        private string processedDirectory;
        private string filePattern;
        private string windowSize;
        private int threadCount;
        private string dbConfig;
        public ShotterWorker(ILogger<ShotterWorker> logger)
        {
            _logger = logger; 
            AssignConfigVariables();
        }

        private void AssignConfigVariables()
        {
            var cnf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables().Build();

            fileDirectory = cnf.GetSection("InputFilesFolder").Value.ToString();
            filePattern = cnf.GetSection("InputFilePattern").Value.ToString();
            threadCount = int.Parse(cnf.GetSection("ShotterThreadCount").Value.ToString());
            processedDirectory = cnf.GetSection("ProcessedFilesFolder").Value.ToString();
            windowSize = cnf.GetSection("WindowSize").Value.ToString();  
            dbConfig = cnf.GetSection("MongoDB").Value.ToString();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Second == 0)
                    _logger.LogInformation("ShotterWorker running at: {time}", DateTimeOffset.Now);

                try
                {
                    ShotterQueue sq = new ShotterQueue(threadCount, dbConfig, windowSize, _logger);
                    IOUtil ioUtil = new IOUtil();

                    string[] files = ioUtil.GetFiles(fileDirectory, filePattern);

                    foreach (var file in files)
                    {
                        string[] lines = ioUtil.GetFileLines(file);

                        foreach (var line in lines)
                        {
                            sq.Enqueue(line.ToString());
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
