using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace GG_Downloader;

internal class GameInstaller {
    public static void ExtractFilesFromDirectory(string inputDirectoryPath) {
        var fileNames = Directory.GetFiles(inputDirectoryPath);
        foreach (var fileName in fileNames) {
            if (Regex.IsMatch(fileName, @"((part(0+)1\.rar))") || (fileName.Contains("rar")&& !fileName.Contains("part"))) {
                ExtractRar(fileName);
            }
        }
    }

    private static void ExtractRar(string filePath) {
        using var archive = RarArchive.Open(filePath);
        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory)) {
            entry.WriteToDirectory(Regex.Replace(Regex.Replace(filePath, @"[^\\]+$", ""), @"\\$", ""),
                new ExtractionOptions() {
                    ExtractFullPath = true,
                    Overwrite = true
                });
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
    public static int InstallFromSetupExecutable(string executablePath) {
        var installDir = Regex.Match(executablePath, @"([\s\S]).*(?=(setup).*(\(\d+\))\.exe)").ToString();
        var proc = new Process();
        var silence = false;
        int exitCode;
        string arguments =
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
            exitCode = proc.ExitCode;
            proc.Close();
            // Console.WriteLine(outPut);
            // Console.WriteLine(proc.ExitCode);
        }
        catch (InvalidOperationException) {
            exitCode = -1;
        }
        return exitCode;
    }

    public static bool CleanupRars(string inputDirectoryPath) {
        return true;
    }
    
}