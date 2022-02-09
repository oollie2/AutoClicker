using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoClicker.Classes
{
    /// <summary>
    /// Class to retreive any Java application instances
    /// </summary>
    internal class GetInstances
    {
        private static readonly List<string> WindowTitles = new()
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
            if(matchingProcesses != null && matchingProcesses.Any())
            {
                if (matchingProcesses.Count > 1)
                    return Multiple();
                else
                    return true;
            }
            else
            {
                //TODO no matching processes found, prompt user to select a process
                return false;
            }
        }
        internal bool Multiple()
        {
            //TODO add user selection for a process
            return false;
        }
    }
}
