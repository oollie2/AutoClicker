using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AutoClicker.Bindings
{
    public class MultipleInstanceBindings : INotifyPropertyChanged
    {
        public MultipleInstanceBindings()
        {
            mainData = new()
            {
                TitleText = "No Client Found",
                BodyText = "Multiple Minecraft instances found. Which ones would you like to run the auto-clicker on?",
                Processes = Process.GetProcesses().Where(b => b.ProcessName.StartsWith("java")).OrderBy(b => b.MainWindowTitle).Select(b => b.MainWindowTitle).ToList()
            };
        }
        private MultipleInstanceData mainData;
        public string TitleText
        {
            get
            {
                if (mainData != null)
                    return mainData.TitleText;
                else
                    return "";
            }
            set
            {
                if (TitleText != value)
                    mainData.TitleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }
        public string BodyText
        {
            get
            {
                if (mainData != null)
                    return mainData.BodyText;
                else
                    return "";
            }
            set
            {
                if (BodyText != value)
                    mainData.BodyText = value;
                OnPropertyChanged(nameof(BodyText));
            }
        }
        public List<string> Processes
        {
            get
            {
                if (mainData != null)
                    return mainData.Processes;
                else
                    return null;
            }
            set
            {
                if (Processes != value)
                    mainData.Processes = value;
                OnPropertyChanged(nameof(Processes));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    sealed class MultipleInstanceData
    {
        public string TitleText { get; set; }
        public string BodyText { get; set; }
        public List<string> Processes { get; set; }
    }
}
