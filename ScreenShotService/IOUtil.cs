using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ScreenShotService
{
    public class IOUtil
    {
        public string[] GetFiles(string pDirectory, string pPattern)
        {
            return Directory.GetFiles(pDirectory, pPattern);
        }

        public string[] GetFileLines(string pFilePath)
        {
            string urlText = File.ReadAllText(pFilePath);
            string[] urlList = urlText.Split(';');
            List<string> lstLines = new List<string>();

            foreach (string url in urlList)
            {
                if (!lstLines.Contains(url))
                {
                    lstLines.Add(url);
                }
            }

            return lstLines.ToArray();
        }
        public void MoveFileToDirectory(string pSourceFile, string pDestinationDir)
        {
            string strFile = Path.GetFileName(pSourceFile);

            pDestinationDir = EnsureDirectory(pDestinationDir);

            string strDestinationFile = Path.Combine(pDestinationDir, strFile);

            MoveFile(pSourceFile, strDestinationFile);
        }

        public void MoveFile(string pSourceFile, string pDestinationFileName)
        {
                if (File.Exists(pSourceFile))
                {
                    if (File.Exists(pDestinationFileName))
                        File.Delete(pDestinationFileName);

                    File.Move(pSourceFile, pDestinationFileName);
                }
                else
                    throw new Exception(String.Format("File does not exist ({0})", pSourceFile));
  
        }

        public string EnsureDirectory(string pDirectoryPath)
        {
            if (!pDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                pDirectoryPath += Path.DirectorySeparatorChar;

            if (!Directory.Exists(pDirectoryPath))
                Directory.CreateDirectory(pDirectoryPath);

            return pDirectoryPath;
        }

        public string CleanInputAndCreteDirectory(string pDirectoryPath, string pInput)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            pInput = rgx.Replace(pInput, "");
            pInput = EnsureDirectory(pDirectoryPath + Path.DirectorySeparatorChar + pInput);
            return pInput;
        }
 

    }
}
