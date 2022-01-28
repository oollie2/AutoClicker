using System;
using System.Windows.Threading;

namespace AutoClicker.Classes
{
    internal class Clicker : IDisposable
    {
        private Button button;
        private IntPtr handle;
        private readonly DispatcherTimer timer;
        private bool hold = false;
        private bool disposed;
        public Clicker(Button _button, IntPtr process)
        {
            handle = process;
            button = _button;

            timer = new();
            timer.Tick += Timer_Tick;
        }

        public void Start(TimeSpan delay)
        {
            Stop();
            hold = (delay.TotalMilliseconds == 0);

            if (hold)
                //Select the handle with Alt+Tab to not stop holding (when using the program)
                Win32Api.PostMessage(handle, button.DownCode, (IntPtr)0x0001, IntPtr.Zero);
            else
            {
                Click();
                timer.Interval = delay;
                timer.Start();
            }
        }

        public void Stop()
        {
            if (!hold)
                timer.Stop();

            Click();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Click();
        }
        private void Click()
        {
            Win32Api.PostMessage(handle, button.DownCode, IntPtr.Zero, IntPtr.Zero);
            Win32Api.PostMessage(handle, button.UpCode, IntPtr.Zero, IntPtr.Zero);
        }
        #region Dispose
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Stop();
            }

            disposed = true;
        }

        ~Clicker()
        {
            Dispose(false);
        }
        #endregion
    }
}
