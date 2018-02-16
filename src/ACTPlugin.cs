using Advanced_Combat_Tracker;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ACTTimeline
{
    public class ACTPlugin : IActPluginV1
    {
        public TabPage ScreenSpace { get; private set; }
        public Label StatusText { get; private set; }

        public TimelineController Controller { get; private set; }

        public PluginSettings Settings { get; private set; }

        private ACTTabPageControl tabPageControl;
        public TimelineView TimelineView { get; private set; }
        public TimelineAutoLoader TimelineAutoLoader { get; private set; }
        private CheckBox checkBoxShowView;
        private VisibilityControl visibilityControl;

        private System.Threading.Timer xivWindowTimer;

        public bool AutoHide { get; set; }

        #region delegates for PluginSettings

        public string TimelineTxtFilePath
        {
            get { return Controller.TimelineTxtFilePath; }
            set { Controller.TimelineTxtFilePath = value; }
        }

        public string FontString
        {
            get { return TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(TimelineView.TimelineFont); }
            set { TimelineView.TimelineFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(value) as Font; }
        }

        public int BarHeight
        {
            get { return TimelineView.BarHeight; }
            set { TimelineView.BarHeight = value; }
        }

        public int BarWidth
        {
            get { return TimelineView.BarWidth; }
            set { TimelineView.BarWidth = value; }
        }

        public int OpacityPercentage
        {
            get { return (int)(TimelineView.MyOpacity * 100.0); }
            set { TimelineView.MyOpacity = (double)value / 100.0; }
        }

        #endregion

        public ACTPlugin()
        {
            // See |InitPlugin()|
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            try
            {
                // DI log writer
                Globals.WriteLogImpl = (str) => { ActGlobals.oFormActMain.WriteInfoLog(String.Format("act_timeline: {0}", str)); };

                ScreenSpace = pluginScreenSpace;
                StatusText = pluginStatusText;

                StatusText.Text = "Loading Sprache.dll";
#if DEBUG
                // Sprache.dll is already injected by libZ in Release builds.
                Assembly.LoadFrom("Sprache.dll");
#endif
                StatusText.Text = "Sprache.dll Load Success!";

#if DEBUG
                // See Issue #1
                // Control.CheckForIllegalCrossThreadCalls = true;
#endif

                Controller = new TimelineController();

                TimelineView = new TimelineView(Controller);
                TimelineView.DoubleClick += TimelineView_DoubleClick;

                visibilityControl = new VisibilityControl(TimelineView);
                visibilityControl.Visible = true;

                TimelineAutoLoader = new TimelineAutoLoader(Controller);
                TimelineAutoLoader.Start();

                Settings = new PluginSettings(this);
                Settings.AddStringSetting("TimelineTxtFilePath");
                Settings.AddStringSetting("FontString");
                Settings.AddIntSetting("BarHeight");
                Settings.AddIntSetting("BarWidth");
                Settings.AddIntSetting("OpacityPercentage");

                SetupTab();
                InjectButton();

                Settings.Load();

                SetupUpdateChecker();

                StatusText.Text = "Plugin Started (^^)!";


                xivWindowTimer = new System.Threading.Timer(e => {
                    try
                    {
                        if (this.AutoHide)
                        {
                            uint pid;
                            var hWndFg = NativeMethods.GetForegroundWindow();
                            if (hWndFg == IntPtr.Zero)
                            {
                                return;
                            }
                            NativeMethods.GetWindowThreadProcessId(hWndFg, out pid);
                            var exePath = Process.GetProcessById((int)pid).MainModule.FileName;

                            if (Path.GetFileName(exePath) == "ffxiv.exe" ||
                                Path.GetFileName(exePath) == "ffxiv_dx11.exe")
                            {
                                this.TimelineView.Invoke(new Action(() => this.TimelineView.Visible = true));
                            }
                            else
                            {
                                this.TimelineView.Invoke(new Action(() => this.TimelineView.Visible = false));
                            }
                        }
                    }
                    catch
                    {
                    }
                });
            }
            catch(Exception e)
            {
                if (StatusText != null)
                    StatusText.Text = "Plugin Init Failed: "+e.Message;
            }
        }

        void TimelineView_DoubleClick(object sender, EventArgs e)
        {
            visibilityControl.Visible = false;
            checkBoxShowView.Checked = false;
        }

        void InjectButton()
        {
            checkBoxShowView = new CheckBox();
            checkBoxShowView.Appearance = System.Windows.Forms.Appearance.Button;
            checkBoxShowView.Name = "checkBoxShowView";
            checkBoxShowView.Size = new System.Drawing.Size(90, 24);
            checkBoxShowView.Text = "Show Timeline";
            checkBoxShowView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            checkBoxShowView.UseVisualStyleBackColor = true;
            checkBoxShowView.Checked = true;
            checkBoxShowView.CheckedChanged += checkBoxShowView_CheckedChanged;
            Settings.AddControlSetting("TimelineShown", checkBoxShowView);

            var formMain = ActGlobals.oFormActMain;
            formMain.Resize += formMain_Resize;
            formMain.Controls.Add(checkBoxShowView);
            formMain.Controls.SetChildIndex(checkBoxShowView, 0);

            formMain_Resize(this, null);
        }

        void checkBoxShowView_CheckedChanged(object sender, EventArgs e)
        {
            visibilityControl.Visible = checkBoxShowView.Checked;
        }

        void formMain_Resize(object sender, EventArgs e)
        {
            // update button location
            var mainFormSize = ActGlobals.oFormActMain.Size;
            checkBoxShowView.Location = new Point(mainFormSize.Width - 435, 0);
        }

        void SetupTab()
        {
            ScreenSpace.Text = "ACT Timeline";

            tabPageControl = new ACTTabPageControl(this);
            ScreenSpace.Controls.Add(tabPageControl);
            ScreenSpace.Resize += ScreenSpace_Resize;
            ScreenSpace_Resize(this, null);

            tabPageControl.Show();
        }

        void ScreenSpace_Resize(object sender, EventArgs e)
        {
            tabPageControl.Location = new System.Drawing.Point(0, 0);
            tabPageControl.Size = ScreenSpace.Size;
        }

        void SetupUpdateChecker()
        {
            ActGlobals.oFormActMain.UpdateCheckClicked += new FormActMain.NullDelegate(CheckForUpdate);
            if (ActGlobals.oFormActMain.GetAutomaticUpdatesAllowed())
                CheckForUpdate();
        }

        void CheckForUpdate()
        {
            var myVersion = typeof(ACTPlugin).Assembly.GetName().Version.ToString();
            var updateChecker = new UpdateChecker(myVersion);
            updateChecker.PerformCheckOnNewThread();
        }

        void IActPluginV1.DeInitPlugin()
        {
            if (checkBoxShowView != null)
                ActGlobals.oFormActMain.Controls.Remove(checkBoxShowView);

            if (TimelineAutoLoader != null)
                TimelineAutoLoader.Stop();

            if (Settings != null)
                Settings.Save();

            if (visibilityControl != null)
                visibilityControl.Close();

            if (TimelineView != null)
                TimelineView.Close();

            if (Controller != null)
                Controller.Stop();

            ActGlobals.oFormActMain.UpdateCheckClicked -= CheckForUpdate;

            if (StatusText != null)
                StatusText.Text = "Plugin Exited m(_ _)m";
        }

        static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
        }
    }
}
