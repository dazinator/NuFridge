using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public static void ExtractZipToFolder(string directory, ZipArchive archive)
        {
            //For each file in the zip archive
            foreach (var entry in archive.Entries)
            {
                //Get the parent folder and check it exists
                var directoryPath = Path.Combine(directory, Path.GetDirectoryName(entry.FullName));

                if (!Directory.Exists(directoryPath))
                {
                    try
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                //Construct the file path including the directory to store the feed in
                var fileName = Path.Combine(directoryPath, entry.Name);

                //Open the zip archive entry stream and copy it to the file output stream
                using (var entryStream = entry.Open())
                {
                    using (var outputStream = File.Create(fileName))
                    {
                        entryStream.CopyTo(outputStream);
                    }
                }
            }
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