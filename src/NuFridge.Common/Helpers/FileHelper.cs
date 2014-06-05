using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NuFridge.Common.Helpers
{
    public static class FileHelper
    {
        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }

        public static bool DeleteFilesInFolder(string folder)
        {
            var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

            var lockedFiles = new List<string>();

            Parallel.ForEach(files, file =>
            {
                var success = DeleteFile(file, 0 , 1);
                if (!success)
                {
                    lockedFiles.Add(file);
                }
            });

            foreach (var lockedFile in lockedFiles)
            {
                DeleteFile(lockedFile, 0, 5);
            }

            return true;
        }

        private static bool DeleteFile(string fileName, int currentRetryCount, int maxRetryCount)
        {
            if (IsFileLocked(fileName))
            {
                currentRetryCount++;

                if (currentRetryCount > maxRetryCount)
                {
                    return false;
                }

                Thread.Sleep(200);

                return DeleteFile(fileName, currentRetryCount, maxRetryCount);
            }

            File.Delete(fileName);

            return true;
        }
    }
}