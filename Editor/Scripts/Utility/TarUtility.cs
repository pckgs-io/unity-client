using System.IO;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;
using UnityEngine;

namespace Pckgs
{
    public class TarUtility
    {
        public static void CreateTarGz(string sourceDirectory, string outputFilePath)
        {
            using (var outStream = File.Create(outputFilePath))
            using (var gzipStream = new GZipOutputStream(outStream))
            using (var tarArchive = TarArchive.CreateOutputTarArchive(gzipStream))
            {
                tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
                if (tarArchive.RootPath.EndsWith("/"))
                    tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

                AddDirectoryFilesToTar(tarArchive, sourceDirectory, "package");
            }
        }

        private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDir, string entryPrefix)
        {
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string relativePath = Path.Combine(entryPrefix, Path.GetFileName(filePath)).Replace('\\', '/');
                var entry = TarEntry.CreateEntryFromFile(filePath);
                entry.Name = relativePath;

                tarArchive.WriteEntry(entry, true);
            }

            foreach (string dirPath in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dirPath);
                string newEntryPrefix = Path.Combine(entryPrefix, dirName).Replace('\\', '/');

                var dirEntry = TarEntry.CreateTarEntry(newEntryPrefix + "/");
                tarArchive.WriteEntry(dirEntry, false);

                AddDirectoryFilesToTar(tarArchive, dirPath, newEntryPrefix);
            }
        }
    }
}