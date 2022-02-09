using System;
using System.Windows.Threading;

namespace AutoClicker.Classes
{
    internal class Clicker : IDisposable
    {
        public uint UpCode { get; set; }
        public uint DownCode { get; set; }
        private IntPtr handle;
        private readonly DispatcherTimer timer;
        private bool hold = false;
        private bool disposed;
        public Clicker(uint _downCode, uint _upCode, IntPtr process)
        {
            handle = process;
            DownCode = _downCode;
            UpCode = _upCode;

            timer = new();
            timer.Tick += Timer_Tick;
        }

        public void Start(TimeSpan delay)
        {
            Stop();
            hold = (delay.TotalMilliseconds == 0);

            if (hold)
                //Select the handle with Alt+Tab to not stop holding (when using the program)
                Win32Api.PostMessage(handle, DownCode, (IntPtr)0x0001, IntPtr.Zero);
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
            Win32Api.PostMessage(handle, DownCode, IntPtr.Zero, IntPtr.Zero);
            Win32Api.PostMessage(handle, UpCode, IntPtr.Zero, IntPtr.Zero);
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
