using System.ComponentModel;

namespace AutoClicker.Bindings
{
    public class DownloadBindings : INotifyPropertyChanged
    {
        public DownloadBindings()
        {
            downloadData = new()
            {
                WindowTitle = "Downloading...",
                FileName = "File...",
                DownloadStatus = "0B / 0B",
                ProgressBarValue = 0
            };
        }
        private DownloadData downloadData;
        public string WindowTitle
        {
            get
            {
                if (downloadData != null)
                    return downloadData.WindowTitle;
                else
                    return "";
            }
            set
            {
                if (WindowTitle != value)
                    downloadData.WindowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }
        public string FileName
        {
            get
            {
                if (downloadData != null)
                    return downloadData.FileName;
                else
                    return "";
            }
            set
            {
                if (FileName != value)
                    downloadData.FileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }
        public double ProgressBarValue
        {
            get
            {
                return downloadData.ProgressBarValue;
            }
            set
            {
                if (ProgressBarValue != value)
                    downloadData.ProgressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
            }
        }
        public string DownloadStatus
        {
            get
            {
                if (downloadData != null)
                    return downloadData.DownloadStatus;
                else
                    return "";
            }
            set
            {
                if (DownloadStatus != value)
                    downloadData.DownloadStatus = value;
                OnPropertyChanged(nameof(DownloadStatus));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    sealed class DownloadData
    {
        public string WindowTitle { get; set; }
        public string FileName { get; set; }
        public double ProgressBarValue { get; set; }
        public string DownloadStatus { get; set; }
    }
}
