using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace ScreenShotConsole
{
    class Program
    {
        private static ILog log;

        static void Main(string[] args)
        {
            var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepo, new FileInfo(Assembly.GetCallingAssembly().ManifestModule.Name + ".config"));
            log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            if (args.Length < 2)
            {
                log.ErrorFormat("Please enter 2 inputs!");
                return;
            }

            string inputFilesFolder = ConfigurationManager.AppSettings["InputFilesFolder"];
            string outputFilesFolder = ConfigurationManager.AppSettings["OutputFilesFolder"];

            switch (args[0])
            {
                case "1":
                    SaveByUrl(args[1], inputFilesFolder);
                    break;
                case "2":
                    SaveByFile(args[1], inputFilesFolder);
                    break;
                case "3":
                    RetrieveByUrl(args[1], outputFilesFolder);
                    break;
                case "4":
                    RetrieveByFile(args[1], outputFilesFolder);
                    break;
                default:
                    log.ErrorFormat("Input 1 is unrecognized! Please provide 1, 2, 3 or 4.");
                    break;
            }       
        }


        private static void SaveByFile(string pFilePath, string inputFilesFolder)
        {
            if (!Path.GetFileName(pFilePath).StartsWith("Input"))
            {
                log.ErrorFormat("File name must start with 'Input'");
            }
            else
            {
                File.Move(pFilePath, inputFilesFolder + "/" + Path.GetFileName(pFilePath));
            }
        }

        private static void SaveByUrl(string pUrl, string inputFilesFolder)
        {
            File.WriteAllText(inputFilesFolder + "/Input" + $@"{DateTime.Now.Ticks}.txt", pUrl);            
        }

        private static void RetrieveByFile(string pFilePath, string outputFilesFolder)
        {
            if (!Path.GetFileName(pFilePath).StartsWith("Ouput"))
            {
                log.ErrorFormat("File name must start with 'Output'");
            }
            else
            {
                File.Move(pFilePath, outputFilesFolder + "/" + Path.GetFileName(pFilePath));
            }
        }

        private static void RetrieveByUrl(string pUrl, string outputFilesFolder)
        {
            File.WriteAllText(outputFilesFolder + "/Output" + $@"{DateTime.Now.Ticks}.txt", pUrl);
        }
    }
}
