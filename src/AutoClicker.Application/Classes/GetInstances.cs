using AutoClicker.Bindings;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoClicker.Windows;

namespace AutoClicker.Classes;
/// <summary>
/// Class to retreive any Java application instances
/// </summary>
internal class GetInstances
{
    public static readonly List<string> WindowTitles = new()
    {
        "Minecraft",
        "RLCraft"
    };
    public List<Process> matchingProcesses { get; set; }
    public GetInstances()
    {
        matchingProcesses = Process.GetProcesses().Where(b => b.ProcessName.StartsWith("java") && WindowTitles.Any(title => b.MainWindowTitle.Contains(title))).ToList();
    }
    internal bool Check()
    {
        if (matchingProcesses != null && matchingProcesses.Any())
        {
            if (matchingProcesses.Count > 1)
            {
                return Multiple();
            }
            else
            {
                return true;
            }
        }
        else
        {
            return NoInstances();
        }
    }
    internal bool Multiple()
    {
        MultipleInstanceBindings binding = new();
        MultipleInstanceWindow multipleInstanceWindow = new(binding);
        bool check = (bool)multipleInstanceWindow.ShowDialog();
        if (check)
        {
            matchingProcesses = multipleInstanceWindow.ProcessSelected;
        }
        return check;
    }
    internal bool NoInstances()
    {
        NoInstanceBindings binding = new();
        NoInstanceWindow noInstanceWindow = new(binding);
        bool check = (bool)noInstanceWindow.ShowDialog();
        if (check)
        {
            matchingProcesses = noInstanceWindow.ProcessSelected;
        }
        return check;
    }
}