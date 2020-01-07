using System;
using System.Windows.Forms;

namespace ACTTimeline
{
    public class VisibilityControl
    {
        private delegate void VisibilityChecker();

        private Control targetControl;
        public bool Visible { get; set; }
        public bool OverlayVisible { get; set; }
        public bool VisibleViaFocus { get; set; }

        public VisibilityControl(Control control, System.Timers.Timer timer)
        {
            targetControl = control;

            VisibleViaFocus = false;
            Visible = true;
            OverlayVisible = true;

            // Glue to avoid interop exceptions from calling Show/Hide directly
            VisibilityChecker myChecker = CheckVisibility;
            var currentDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            // Let the polling begin
            timer.Elapsed += (o, e) => { currentDispatcher.Invoke(myChecker); };
        }

        public void Close()
        {
            targetControl = null;
        }

        private void CheckVisibility()
        {
            if (targetControl == null)
                return;

            bool shouldBeVisible = Visible && OverlayVisible && VisibleViaFocus;

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
    }
}
