using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace AutoClicker.Classes
{
    internal class Updater
    {
        private GitHubClient client;
        private IReadOnlyList<Release> releases;
        private readonly string OwnerName = "oollie34";
        private readonly string RepoName = "AutoClicker";
        public string DownloadLocation;
        public UpdateState State { get; set; }
        public Updater()
        {
            DownloadLocation = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads\AutoClicker.msi");
        }
        public async Task CheckVersionsAsync()
        {
            client = new(new ProductHeaderValue("AutoClickerUpdate"));
            releases = await client.Repository.Release.GetAll(OwnerName, RepoName);
            Version latestGitHubVersion = new(releases[0].TagName);
            Version localVersion = new(Assembly.GetEntryAssembly().GetName().Version.Major + "." +
                Assembly.GetEntryAssembly().GetName().Version.Minor + "." + Assembly.GetEntryAssembly().GetName().Version.Build);
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            Debug.WriteLine(versionComparison);
            if (versionComparison < 0)
            {
                State = UpdateState.UpdateAvailable;
            }
            else
            {
                State = UpdateState.NoUpdateAvailable;
            }
        }
        public async void PerformUpdate()
        {
            await PerformUpdateAsync();
        }
        private async Task PerformUpdateAsync()
        {
            //todo we need a progress bar for the download
            var assets = await client.Repository.Release.GetAllAssets(OwnerName, RepoName, releases[0].Id);
            HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.GetAsync(assets[0].BrowserDownloadUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream stream = await response.Content.ReadAsStreamAsync();
                Debug.WriteLine(DownloadLocation);
                FileInfo fileInfo = new(DownloadLocation);
                using FileStream fileStream = fileInfo.OpenWrite();
                await stream.CopyToAsync(fileStream);
                RunUpdate();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Unable to download the update file.\r\nPlease raise an issue with the log file and any other details on GitHub.",
                    "Unable to Update",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                    Environment.Exit(0);
            }
        }
        private void RunUpdate()
        {
            using Process installerProcess = new();
            ProcessStartInfo processInfo = new();
            processInfo.FileName = "cmd.exe";
            Debug.WriteLine(DownloadLocation);
            processInfo.Arguments = "/C start " + DownloadLocation;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = false;
            installerProcess.StartInfo = processInfo;
            installerProcess.Start();
            Environment.Exit(0);
        }
    }
    public enum UpdateState
    {
        NotChecked,
        UpdateAvailable,
        NoUpdateAvailable
    }
}
