using System.ComponentModel;
using System.Windows;

namespace AutoClicker.Bindings
{
    sealed class MainBindings : INotifyPropertyChanged
    {
        public MainBindings()
        {
            mainData = new()
            {
                LeftTitle = "Left Mouse Button",
                RightTitle = "Right Mouse Button",
                LeftTopCheckBox = false,
                RightTopCheckBox = false,
                LeftBottomCheckBox = false,
                RightBottomCheckBox = false,
                LeftUpDownText = "200",
                RightUpDownText = "200",
                LeftButtonContent = "START",
                RightButtonContent = "STOP!",
                LeftButtonEnabled = true,
                RightButtonEnabled = false,
                ApplicationEnabled = true,
                IndicatorLabelVisible = Visibility.Visible,
                IndicatorLabel = "Idle"
            };
        }
        private MainData mainData;
        public string LeftTitle
        {
            get
            {
                if (mainData != null)
                    return mainData.LeftTitle;
                else
                    return "";
            }
            set
            {
                if (LeftTitle != value)
                    mainData.LeftTitle = value;
                OnPropertyChanged(nameof(LeftTitle));
            }
        }
        public bool LeftTopCheckBox
        {
            get
            {
                if (mainData != null)
                    return mainData.LeftTopCheckBox;
                else
                    return false;
            }
            set
            {
                if (LeftTopCheckBox != value)
                    mainData.LeftTopCheckBox = value;
                OnPropertyChanged(nameof(LeftTopCheckBox));
            }
        }
        public bool LeftBottomCheckBox
        {
            get
            {
                if (mainData != null)
                    return mainData.LeftBottomCheckBox;
                else
                    return false;
            }
            set
            {
                if (LeftBottomCheckBox != value)
                    mainData.LeftBottomCheckBox = value;
                OnPropertyChanged(nameof(LeftBottomCheckBox));
            }
        }
        public string LeftUpDownText
        {
            get
            {
                if (mainData != null)
                    return mainData.LeftUpDownText;
                else
                    return "";
            }
            set
            {
                if (LeftUpDownText != value)
                    mainData.LeftUpDownText = value;
                OnPropertyChanged(nameof(LeftUpDownText));
            }
        }
        public string RightTitle
        {
            get
            {
                if (mainData != null)
                    return mainData.RightTitle;
                else
                    return "";
            }
            set
            {
                if (RightTitle != value)
                    mainData.RightTitle = value;
                OnPropertyChanged(nameof(RightTitle));
            }
        }
        public bool RightTopCheckBox
        {
            get
            {
                if (mainData != null)
                    return mainData.RightTopCheckBox;
                else
                    return false;
            }
            set
            {
                if (RightTopCheckBox != value)
                    mainData.RightTopCheckBox = value;
                OnPropertyChanged(nameof(RightTopCheckBox));
            }
        }
        public bool RightBottomCheckBox
        {
            get
            {
                if (mainData != null)
                    return mainData.RightBottomCheckBox;
                else
                    return false;
            }
            set
            {
                if (RightBottomCheckBox != value)
                    mainData.RightBottomCheckBox = value;
                OnPropertyChanged(nameof(RightBottomCheckBox));
            }
        }
        public string RightUpDownText
        {
            get
            {
                if (mainData != null)
                    return mainData.RightUpDownText;
                else
                    return "";
            }
            set
            {
                if (RightUpDownText != value)
                    mainData.RightUpDownText = value;
                OnPropertyChanged(nameof(RightUpDownText));
            }
        }
        public string LeftButtonContent
        {
            get
            {
                if (mainData != null)
                    return mainData.LeftButtonContent;
                else
                    return "";
            }
            set
            {
                if (LeftButtonContent != value)
                    mainData.LeftButtonContent = value;
                OnPropertyChanged(nameof(LeftButtonContent));
            }
        }
        public string RightButtonContent
        {
            get
            {
                if (mainData != null)
                    return mainData.RightButtonContent;
                else
                    return "";
            }
            set
            {
                if (RightButtonContent != value)
                    mainData.RightButtonContent = value;
                OnPropertyChanged(nameof(RightButtonContent));
            }
        }
        public string IndicatorLabel
        {
            get
            {
                if (mainData != null)
                    return mainData.IndicatorLabel;
                else
                    return "";
            }
            set
            {
                if (IndicatorLabel != value)
                    mainData.IndicatorLabel = value;
                OnPropertyChanged(nameof(IndicatorLabel));
            }
        }
        public bool LeftButtonEnabled
        {
            get
            {
                if (mainData != null)
                    return mainData.LeftButtonEnabled;
                else
                    return false;
            }
            set
            {
                if (LeftButtonEnabled != value)
                    mainData.LeftButtonEnabled = value;
                OnPropertyChanged(nameof(LeftButtonEnabled));
            }
        }
        public bool RightButtonEnabled
        {
            get
            {
                if (mainData != null)
                    return mainData.RightButtonEnabled;
                else
                    return false;
            }
            set
            {
                if (RightButtonEnabled != value)
                    mainData.RightButtonEnabled = value;
                OnPropertyChanged(nameof(RightButtonEnabled));
            }
        }
        public bool ApplicationEnabled
        {
            get
            {
                if (mainData != null)
                    return mainData.ApplicationEnabled;
                else
                    return false;
            }
            set
            {
                if (ApplicationEnabled != value)
                    mainData.ApplicationEnabled = value;
                OnPropertyChanged(nameof(ApplicationEnabled));
            }
        }
        public Visibility IndicatorLabelVisible
        {
            get
            {
                if (mainData != null)
                    return mainData.IndicatorLabelVisible;
                else
                    return Visibility.Hidden;
            }
            set
            {
                if (IndicatorLabelVisible != value)
                    mainData.IndicatorLabelVisible = value;
                OnPropertyChanged(nameof(IndicatorLabelVisible));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    sealed class MainData
    {
        public string LeftTitle { get; set; }
        public bool LeftTopCheckBox { get; set; }
        public bool LeftBottomCheckBox { get; set; }
        public string LeftUpDownText { get; set; }
        public string RightTitle { get; set; }
        public bool RightTopCheckBox { get; set; }
        public bool RightBottomCheckBox { get; set; }
        public string RightUpDownText { get; set; }
        public string LeftButtonContent { get; set; }
        public string RightButtonContent { get; set; }
        public string IndicatorLabel { get; set; }
        public bool LeftButtonEnabled { get; set; }
        public bool RightButtonEnabled { get; set; }
        public bool ApplicationEnabled { get; set; }
        public Visibility IndicatorLabelVisible { get; set; }
    }
}
