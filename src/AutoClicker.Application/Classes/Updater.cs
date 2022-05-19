using AutoClicker.Windows;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoClicker.Classes;
internal class Updater
{
    private GitHubClient client;
    private IReadOnlyList<Release> releases;
    private readonly string OwnerName = "oollie34";
    private readonly string RepoName = "AutoClicker";
    public string DownloadLocation;
    private Bindings.DownloadBindings downloadBindings;
    private DownloadFile downloadFile;
    public UpdateState State { get; set; }
    private string[] VersionLabels = { "alpha", "beta" };
    public Updater()
    {
        DownloadLocation = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads\AutoClicker.msi");
    }
    /// <summary>
    /// Here we check for the latest release tag available on github.
    /// The tags should be formatted such as 1.0.0 with no other information.
    /// It is possible that an alpha / beta tag is present. This class will handle that and return a different state.
    /// </summary>
    /// <returns></returns>
    public async Task CheckVersionsAsync()
    {
        client = new(new ProductHeaderValue("AutoClickerUpdate"));
        releases = await client.Repository.Release.GetAll(OwnerName, RepoName);
        string tagName = releases[0].TagName;
        Version latestGitHubVersion = new(Regex.Match(tagName, @"([\d\.]+)").Groups[0].Value);
        Version localVersion = new(Assembly.GetEntryAssembly().GetName().Version.Major + "." +
            Assembly.GetEntryAssembly().GetName().Version.Minor + "." + Assembly.GetEntryAssembly().GetName().Version.Build);
        int versionComparison = localVersion.CompareTo(latestGitHubVersion);
        if (versionComparison < 0)
        {
            State = VersionLabels.FirstOrDefault(s => tagName.ToLower().Contains(s)) switch
            {
                "alpha" => UpdateState.AlphaUpdateAvailable,
                "beta" => UpdateState.BetaUpdateAvailable,
                _ => UpdateState.UpdateAvailable,
            };
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
    /// <summary>
    /// Download the update file, progress shown in a window. 
    /// </summary>
    /// <returns></returns>
    private async Task PerformUpdateAsync()
    {
        var assets = await client.Repository.Release.GetAllAssets(OwnerName, RepoName, releases[0].Id);
        HttpClientWithProgress httpClient = new(assets[0].BrowserDownloadUrl, DownloadLocation);
        downloadBindings = new();
        downloadBindings.WindowTitle = "Downloading Update " + releases[0].TagName;
        downloadBindings.FileName = DownloadLocation;
        downloadFile = new DownloadFile(downloadBindings);
        downloadFile.Show();
        httpClient.ProgressChanged += HttpClient_ProgressChanged;
        await httpClient.StartDownload();
    }
    private void HttpClient_ProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
    {
        downloadBindings.ProgressBarValue = (double)progressPercentage;
        downloadBindings.DownloadStatus = Math.Round(totalBytesDownloaded / 1e+6, 3).ToString("0.000") + " MB / " + Math.Round((long)totalFileSize / 1e+6, 3).ToString("0.000") + " MB";
        if(progressPercentage == 100)
        {
            RunUpdate();
        }
    }
    /// <summary>
    /// Run the downloaded update file.
    /// </summary>
    private void RunUpdate()
    {
        using Process installerProcess = new();
        ProcessStartInfo processInfo = new();
        processInfo.FileName = "cmd.exe";
        processInfo.Arguments = "/C start " + DownloadLocation;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        installerProcess.StartInfo = processInfo;
        installerProcess.Start();
        Environment.Exit(0);
    }
}
public enum UpdateState
{
    NotChecked,
    UpdateAvailable,
    NoUpdateAvailable,
    AlphaUpdateAvailable,
    BetaUpdateAvailable
}