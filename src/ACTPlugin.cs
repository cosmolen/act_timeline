using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;


namespace ACTTimeline
{
    public class ACTPlugin : IActPluginV1
    {
        public TabPage ScreenSpace { get; private set; }
        public Label StatusText { get; private set; }

        public TimelineController Controller { get; private set; }

        public PluginSettings Settings { get; private set; }

        public ACTTabPageControl TabPageControl { get; private set; }

        public int NumberOfOverlays { get; private set; }

        public List<TimelineView> TimelineViewList { get; private set; }
        public List<VisibilityControl> VisibilityControlList { get; private set; }

        public TimelineAutoLoader TimelineAutoLoader { get; private set; }
        public CheckBox checkBoxShowView { get; private set; }

        private const double autoHideTimerInterval = 1000.0; // miliseconds

        private System.Timers.Timer timer;
        private System.Threading.Timer xivWindowTimer;

        public bool AutoHide { get; set; }

        public ACTPlugin() { }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            try
            {
                // DI log writer
                Globals.WriteLogImpl = (str) => { ActGlobals.oFormActMain.WriteInfoLog(String.Format("act_timeline: {0}", str)); };

                ScreenSpace = pluginScreenSpace;
                StatusText = pluginStatusText;

#if DEBUG
                StatusText.Text = "Loading Sprache.dll";
                // Sprache.dll is already injected by libZ in Release builds.
                Assembly.LoadFrom("Sprache.dll");
                StatusText.Text = "Sprache.dll Load Success!";

                // See Issue #1
                // Control.CheckForIllegalCrossThreadCalls = true;
#endif

                NumberOfOverlays = 5;

                Controller = new TimelineController(this);
                Controller.TimelineTxtFilePath = String.Empty;

                TimelineViewList = new List<TimelineView>();
                VisibilityControlList = new List<VisibilityControl>();

                timer = new System.Timers.Timer(autoHideTimerInterval);

                Settings = new PluginSettings(this);

                for (int i = 0; i < NumberOfOverlays; i++)
                {
                    TimelineView tvTemp = new TimelineView(Controller);
                    TimelineViewList.Add(tvTemp);
                    tvTemp.DoubleClick += TimelineView_DoubleClick;

                    VisibilityControl vcTemp = new VisibilityControl(tvTemp, timer);
                    VisibilityControlList.Add(vcTemp);
                    vcTemp.Visible = false;
                }

                timer.Start();

                TimelineAutoLoader = new TimelineAutoLoader(this);
                TimelineAutoLoader.Start();

                SetupTab();
                InjectButton();

                Settings.Load();

                SetupUpdateChecker();

                StatusText.Text = "Plugin Started.";

                xivWindowTimer = new System.Threading.Timer(e => {
                    bool visibleViaFocus = true;

                    if (AutoHide)
                    {
                        string processName = String.Empty;

                        // Attempt to grab the process name of the current active window, if there is one
                        // Attempt to exit gracefully if there's an issue
                        try
                        {
                            uint pid = 0;
                            var handle = NativeMethods.GetForegroundWindow();
                            NativeMethods.GetWindowThreadProcessId(handle, out pid);
                            Process p = Process.GetProcessById((int)pid);
                            processName = p.ProcessName;
                        }
                        catch
                        {
                            visibleViaFocus = false;
                        }

                        // Catches both DX9 and DX11 clients, as well as ACT (which happens to also be our parent process)
                        // Including ACT is important not only for debugging, but also because calling Show will usually kick
                        // ACT to the foreground
                        if (processName.StartsWith("ffxiv") || processName.StartsWith("Advanced Combat Tracker"))
                            visibleViaFocus = true;
                        else
                            visibleViaFocus = false;
                    }

                    foreach (VisibilityControl vc in VisibilityControlList)
                    {
                        vc.VisibleViaFocus = visibleViaFocus;
                    }
                }, null, 0, (int) autoHideTimerInterval);
            }
            catch (Exception e)
            {
                Globals.WriteLog(e.ToString());

                if (StatusText != null)
                    StatusText.Text = "Plugin Init Failed: " + e.Message;
            }
        }

        public void updateAllVisibility(bool visible)
        {
            foreach (VisibilityControl vc in VisibilityControlList)
            {
                vc.Visible = visible;
            }
        }

        void TimelineView_DoubleClick(object sender, EventArgs e)
        {
            updateAllVisibility(false);
            checkBoxShowView.Checked = false;
        }

        void InjectButton()
        {
            checkBoxShowView = new CheckBox();
            checkBoxShowView.Appearance = System.Windows.Forms.Appearance.Button;
            checkBoxShowView.Name = "checkBoxShowView";
            checkBoxShowView.Size = new System.Drawing.Size(70, 24);
            checkBoxShowView.Text = Translator.Get("_Show_Timeline");
            checkBoxShowView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            checkBoxShowView.UseVisualStyleBackColor = true;
            checkBoxShowView.Checked = true;
            checkBoxShowView.CheckedChanged += checkBoxShowView_CheckedChanged;
            Settings.AddControlSetting("TimelineShown", checkBoxShowView);

            ActGlobals.oFormActMain.CornerControlAdd(checkBoxShowView);
        }

        void checkBoxShowView_CheckedChanged(object sender, EventArgs e)
        {
            updateAllVisibility(checkBoxShowView.Checked);
        }

        void SetupTab()
        {
            ScreenSpace.Text = Translator.Get("_ACT_Timeline");

            TabPageControl = new ACTTabPageControl(this);
            ScreenSpace.Controls.Add(TabPageControl);
            ScreenSpace.Resize += ScreenSpace_Resize;
            ScreenSpace_Resize(this, null);

            TabPageControl.Show();
        }

        void ScreenSpace_Resize(object sender, EventArgs e)
        {
            TabPageControl.Location = new System.Drawing.Point(0, 0);
            TabPageControl.Size = ScreenSpace.Size;
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
                ActGlobals.oFormActMain.CornerControlRemove(checkBoxShowView);

            TimelineAutoLoader?.Stop();
            Settings?.Save();

            foreach (var vc in VisibilityControlList)
            {
                vc?.Close();
            }

            foreach (var tv in TimelineViewList)
            {
                tv?.Close();
            }
            
            timer?.Stop();
            Controller?.Stop();

            ActGlobals.oFormActMain.UpdateCheckClicked -= CheckForUpdate;

            if (StatusText != null)
                StatusText.Text = "Plugin Exited.";
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
