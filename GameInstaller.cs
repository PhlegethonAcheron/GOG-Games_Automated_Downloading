using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace GG_Downloader {
    internal static class GameInstaller {
        public static void ExtractFilesFromDirectory(string inputDirectoryPath) {
            Console.WriteLine($"Extracting .rar files from {inputDirectoryPath}");
            var fileNames = Directory.GetFiles(inputDirectoryPath);
            foreach (var fileName in fileNames) {
                if (Regex.IsMatch(fileName, @"((part(0+)1\.rar))") ||
                    (fileName.Contains("rar") && !fileName.Contains("part"))) {
                    ExtractRar(fileName);
                }
            }
        }

        private static void ExtractRar(string filePath) {
            using (var archive = RarArchive.Open(filePath)) {
                Console.WriteLine($"\tExtracting {Regex.Match(filePath, @"[^\\]+$")}");
                foreach (var entry in archive.Entries) {
                    if (!entry.IsDirectory) {
                        entry.WriteToDirectory(Regex.Replace(Regex.Replace(filePath, @"[^\\\¥]+$", ""), @"[\\\¥]$", ""), new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }
        }

        /// <summary>
        /// Iterates through all files in a directory, if the filename matches the pattern <c>"(setup).*(\(\d+\))\.exe"</c>,
        /// calls <c>InstallFromSetupExecutable</c> on the found setup files.
        /// </summary>
        /// <param name="gamePath"></param>
        /// <returns>Boolean foundSetup</returns>
        public static bool InstallGivenDirectory(string gamePath) {
            var fileNames = Directory.GetFiles(gamePath);
            var foundSetup = false;
            foreach (var fileName in fileNames) {
                if (!Regex.IsMatch(fileName, @"(setup).*(\(\d+\))\.exe")) continue;
                InstallFromSetupExecutable(fileName);
                foundSetup = true;
            }

            return foundSetup;
        }

        /// <summary>
        /// Installs from setup executable, given absolute path of setup.exe. Must have admin permissions for now.
        /// </summary>
        /// <param name="executablePath"></param>
        /// <returns>Exit code: 3 For success, -1 for failure due to not having admin permissions.</returns>
        private static void InstallFromSetupExecutable(string executablePath) {
            var installDir = Regex.Match(executablePath, @"([\s\S]).*(?=(setup).*(\(\d+\))\.exe)").ToString();
            Console.WriteLine($"\tInstalling {Regex.Match(executablePath, @"(setup).*(\(\d+\))\.exe")}");
            var proc = new Process();
            var silence = false;
#pragma warning disable CS0219
            int exitCode;
#pragma warning restore CS0219
            var arguments =
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                $"/portable=1 /dir=\"{installDir}\" /{(silence ? "verysilent" : "silent")} /suppressmsgboxes /noicons /norestart /nocloseapplications /lang=english";
            try {
                proc.StartInfo.FileName = executablePath;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                // ReSharper disable once UnusedVariable
                string outPut = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                // exitCode = proc.ExitCode;
                proc.Close();
            }
            catch (InvalidOperationException) {
                // ReSharper disable once RedundantAssignment
                exitCode = -1;
            }
        }

        /// <summary>
        /// Moves all rar files and "Uploaded by gog-games.txt" files to the recycle bin.
        /// </summary>
        /// <param name="inputDirectoryPath"></param>
        public static void CleanupFiles(string inputDirectoryPath) {
            var fileNames = Directory.GetFiles(inputDirectoryPath);
            foreach (var fileName in fileNames) {
                if (Regex.IsMatch(fileName, @"[\s\S]+\.rar$") ||
                    fileName.Contains("Uploaded by www.gog-games.com.txt")) {
                    FileSystem.DeleteFile(fileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
            }
        }

        public static void CleanupV2(string inputDirectoryPath) {
            var cleanupRegexes = new List<string>() {
                @"[\s\S]+\.rar$",
                @"Uploaded by www.*.com\.txt$",
                @"unins000\.ini$",
                @"support\.ico$",
                @"gog\.ico$",
                @"goglog\.ini$",
                @"webcache\.zip$",
                @"(setup).*(\(\d+\))\.exe",
                "redist"
            };
            var fileNames = (Directory.GetFiles(inputDirectoryPath).ToList());
            fileNames.AddRange((Directory.GetDirectories(inputDirectoryPath).ToList()));
            foreach (var fileName in from fileName in fileNames
                     from regex in cleanupRegexes
                     where Regex.IsMatch(fileName, regex)
                     select fileName) {
                if (File.GetAttributes(fileName).HasFlag(FileAttributes.Directory)) {
                    Console.WriteLine($"\tDeleting Directory {fileName}");
                    FileSystem.DeleteDirectory(fileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else {
                    Console.WriteLine($"\tDeleting file {fileName}");
                    FileSystem.DeleteFile(fileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
            }
        }
    }
}