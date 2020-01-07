using Advanced_Combat_Tracker;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
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

        public ACTTabPageControl tabPageControl { get; private set; }
        public TimelineView TimelineView { get; private set; }
        public TimelineView TimelineView2 { get; private set; }
        public TimelineView TimelineView3 { get; private set; }
        public TimelineView TimelineView4 { get; private set; }
        public TimelineView TimelineView5 { get; private set; }
        public VisibilityControl VisibilityControl { get; private set; }
        public VisibilityControl VisibilityControl2 { get; private set; }
        public VisibilityControl VisibilityControl3 { get; private set; }
        public VisibilityControl VisibilityControl4 { get; private set; }
        public VisibilityControl VisibilityControl5 { get; private set; }

        public TimelineAutoLoader TimelineAutoLoader { get; private set; }
        public CheckBox checkBoxShowView { get; private set; }

        const double autoHideTimerInterval = 1000.0; // miliseconds

        private System.Timers.Timer timer;
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

        public string FontString2
        {
            get { return TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(TimelineView2.TimelineFont); }
            set { TimelineView2.TimelineFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(value) as Font; }
        }
        public int BarHeight2
        {
            get { return TimelineView2.BarHeight; }
            set { TimelineView2.BarHeight = value; }
        }
        public int BarWidth2
        {
            get { return TimelineView2.BarWidth; }
            set { TimelineView2.BarWidth = value; }
        }
        public int OpacityPercentage2
        {
            get { return (int)(TimelineView2.MyOpacity * 100.0); }
            set { TimelineView2.MyOpacity = (double)value / 100.0; }
        }
        public string FontString3
        {
            get { return TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(TimelineView3.TimelineFont); }
            set { TimelineView3.TimelineFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(value) as Font; }
        }
        public int BarHeight3
        {
            get { return TimelineView3.BarHeight; }
            set { TimelineView3.BarHeight = value; }
        }
        public int BarWidth3
        {
            get { return TimelineView3.BarWidth; }
            set { TimelineView3.BarWidth = value; }
        }
        public int OpacityPercentage3
        {
            get { return (int)(TimelineView3.MyOpacity * 100.0); }
            set { TimelineView3.MyOpacity = (double)value / 100.0; }
        }
        public string FontString4
        {
            get { return TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(TimelineView4.TimelineFont); }
            set { TimelineView4.TimelineFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(value) as Font; }
        }
        public int BarHeight4
        {
            get { return TimelineView4.BarHeight; }
            set { TimelineView4.BarHeight = value; }
        }
        public int BarWidth4
        {
            get { return TimelineView4.BarWidth; }
            set { TimelineView4.BarWidth = value; }
        }
        public int OpacityPercentage4
        {
            get { return (int)(TimelineView4.MyOpacity * 100.0); }
            set { TimelineView4.MyOpacity = (double)value / 100.0; }
        }
        public string FontString5
        {
            get { return TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(TimelineView5.TimelineFont); }
            set { TimelineView5.TimelineFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(value) as Font; }
        }
        public int BarHeight5
        {
            get { return TimelineView5.BarHeight; }
            set { TimelineView5.BarHeight = value; }
        }
        public int BarWidth5
        {
            get { return TimelineView5.BarWidth; }
            set { TimelineView5.BarWidth = value; }
        }
        public int OpacityPercentage5
        {
            get { return (int)(TimelineView5.MyOpacity * 100.0); }
            set { TimelineView5.MyOpacity = (double)value / 100.0; }
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

#if DEBUG
                StatusText.Text = "Loading Sprache.dll";
                // Sprache.dll is already injected by libZ in Release builds.
                Assembly.LoadFrom("Sprache.dll");
                StatusText.Text = "Sprache.dll Load Success!";
#endif

#if DEBUG
                // See Issue #1
                // Control.CheckForIllegalCrossThreadCalls = true;
#endif

                Controller = new TimelineController(this);
                Controller.TimelineTxtFilePath = String.Empty;

                TimelineView = new TimelineView(Controller);
                TimelineView.DoubleClick += TimelineView_DoubleClick;
                TimelineView2 = new TimelineView(Controller);
                TimelineView2.DoubleClick += TimelineView_DoubleClick;
                TimelineView3 = new TimelineView(Controller);
                TimelineView3.DoubleClick += TimelineView_DoubleClick;
                TimelineView4 = new TimelineView(Controller);
                TimelineView4.DoubleClick += TimelineView_DoubleClick;
                TimelineView5 = new TimelineView(Controller);
                TimelineView5.DoubleClick += TimelineView_DoubleClick;

                timer = new System.Timers.Timer(autoHideTimerInterval);

                VisibilityControl = new VisibilityControl(TimelineView, timer);
                VisibilityControl.Visible = false;
                VisibilityControl2 = new VisibilityControl(TimelineView2, timer);
                VisibilityControl2.Visible = false;
                VisibilityControl3 = new VisibilityControl(TimelineView3, timer);
                VisibilityControl3.Visible = false;
                VisibilityControl4 = new VisibilityControl(TimelineView4, timer);
                VisibilityControl4.Visible = false;
                VisibilityControl5 = new VisibilityControl(TimelineView5, timer);
                VisibilityControl5.Visible = false;

                timer.Start();

                TimelineAutoLoader = new TimelineAutoLoader(this);
                TimelineAutoLoader.Start();

                Settings = new PluginSettings(this);

                Settings.AddStringSetting("FontString");
                Settings.AddStringSetting("FontString2");
                Settings.AddStringSetting("FontString3");
                Settings.AddStringSetting("FontString4");
                Settings.AddStringSetting("FontString5");

                Settings.AddIntSetting("BarHeight");
                Settings.AddIntSetting("BarHeight2");
                Settings.AddIntSetting("BarHeight3");
                Settings.AddIntSetting("BarHeight4");
                Settings.AddIntSetting("BarHeight5");

                Settings.AddIntSetting("BarWidth");
                Settings.AddIntSetting("BarWidth2");
                Settings.AddIntSetting("BarWidth3");
                Settings.AddIntSetting("BarWidth4");
                Settings.AddIntSetting("BarWidth5");

                Settings.AddIntSetting("OpacityPercentage");
                Settings.AddIntSetting("OpacityPercentage2");
                Settings.AddIntSetting("OpacityPercentage3");
                Settings.AddIntSetting("OpacityPercentage4");
                Settings.AddIntSetting("OpacityPercentage5");

                SetupTab();
                InjectButton();

                Settings.Load();

                SetupUpdateChecker();

                StatusText.Text = "Plugin Started.";

                xivWindowTimer = new System.Threading.Timer(e => {
                    if (AutoHide)
                    {
                        bool visibleViaFocus = false;
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

                        VisibilityControl.VisibleViaFocus = visibleViaFocus;
                        VisibilityControl2.VisibleViaFocus = visibleViaFocus;
                        VisibilityControl3.VisibleViaFocus = visibleViaFocus;
                        VisibilityControl4.VisibleViaFocus = visibleViaFocus;
                        VisibilityControl5.VisibleViaFocus = visibleViaFocus;
                    }
                }, null, 0, (int) autoHideTimerInterval);
            }
            catch(Exception e)
            {
                if (StatusText != null)
                    StatusText.Text = "Plugin Init Failed: "+e.Message;
            }
        }

        public void updateAllVisibility(bool visible)
        {
            VisibilityControl.Visible = visible;
            VisibilityControl2.Visible = visible;
            VisibilityControl3.Visible = visible;
            VisibilityControl4.Visible = visible;
            VisibilityControl5.Visible = visible;
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
                ActGlobals.oFormActMain.CornerControlRemove(checkBoxShowView);

            if (TimelineAutoLoader != null)
                TimelineAutoLoader.Stop();

            if (Settings != null)
                Settings.Save();

            if (VisibilityControl != null)
                VisibilityControl.Close();

            if (VisibilityControl2 != null)
                VisibilityControl2.Close();
            if (VisibilityControl3 != null)
                VisibilityControl3.Close();
            if (VisibilityControl4 != null)
                VisibilityControl4.Close();
            if (VisibilityControl5 != null)
                VisibilityControl5.Close();

            if (TimelineView != null)
                TimelineView.Close();

            if (TimelineView2 != null)
                TimelineView2.Close();
            if (TimelineView3 != null)
                TimelineView3.Close();
            if (TimelineView4 != null)
                TimelineView4.Close();
            if (TimelineView5 != null)
                TimelineView5.Close();
            
            timer.Stop();

            if (Controller != null)
                Controller.Stop();

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
