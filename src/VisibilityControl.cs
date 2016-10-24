using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACTTimeline
{
    class VisibilityControl
    {
        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(int hWnd, out uint ProcessId);

        private delegate void VisibilityChecker();

        private Control targetControl;
        private System.Timers.Timer timer;

        private bool visibleViaFocus;
        public bool Visible { get; set; }

        private string lastProcessName;

        const double timerInterval = 200.0; // miliseconds

        public VisibilityControl(Control control)
        {
            targetControl = control;

            visibleViaFocus = false;
            Visible = true;

            // Glue to avoid interop exceptions from calling Show/Hide directly
            VisibilityChecker myChecker = CheckVisibility;
            var currentDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            // Let the polling begin
            timer = new System.Timers.Timer(timerInterval);
            timer.Elapsed += (o, e) => { currentDispatcher.Invoke(myChecker); };
            timer.Start();
        }

        public void Close()
        {
            timer.Stop();
            targetControl = null;
        }

        private void CheckVisibility()
        {
            if (targetControl == null)
                return;

            bool shouldBeVisible = Visible && CheckVisibilityViaFocus();

            // We use a visibility flag instead and compare "expected" vs "actual" here in order to avoid calling
            // Show all the time, which will potentially kick ACT to the foreground.
            // TODO: Figure out why switching to FFXIV from something other than ACT doesn't secretly steal focus
            //       away from FFXIV and immediately give it to ACT. It's good that it works, but I'm not exactly
            //       sure why it works which is troubling
            if (targetControl.Visible == shouldBeVisible)
                return;

            if (shouldBeVisible)
                targetControl.Show();
            else
                targetControl.Hide();
        }

        private bool CheckVisibilityViaFocus()
        {
            string processName;

            // Attempt to grab the process name of the current active window, if there is one
            // Attempt to exit gracefully if there's an issue
            try
            {
                uint pid = 0;
                int handle = GetForegroundWindow();
                GetWindowThreadProcessId(handle, out pid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById((int)pid);
                processName = p.ProcessName;
            }
            catch
            {
                visibleViaFocus = false;
                return visibleViaFocus;
            }

            if (processName == lastProcessName)
                return visibleViaFocus;

            // Catches both DX9 and DX11 clients, as well as ACT (which happens to also be our parent process)
            // Including ACT is important not only for debugging, but also because calling Show will usually kick
            // ACT to the foreground
            if (processName.StartsWith("ffxiv") || processName.StartsWith("Advanced Combat Tracker"))
                visibleViaFocus = true;
            else
                visibleViaFocus = false;

            // Store the last active process name to save ourselves time next go-around
            lastProcessName = processName;

            return visibleViaFocus;
        }
    }
}
