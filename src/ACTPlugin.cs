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
        public TimelineView TimelineView2 { get; private set; }
        public TimelineView TimelineView3 { get; private set; }
        public TimelineView TimelineView4 { get; private set; }
        public TimelineView TimelineView5 { get; private set; }
        public TimelineAutoLoader TimelineAutoLoader { get; private set; }
        private CheckBox checkBoxShowView;
        public VisibilityControl VisibilityControl { get; private set; }
        public VisibilityControl VisibilityControl2 { get; private set; }
        public VisibilityControl VisibilityControl3 { get; private set; }
        public VisibilityControl VisibilityControl4 { get; private set; }
        public VisibilityControl VisibilityControl5 { get; private set; }

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

                TimelineView2 = new TimelineView(Controller);
                TimelineView2.DoubleClick += TimelineView_DoubleClick;
                TimelineView3 = new TimelineView(Controller);
                TimelineView3.DoubleClick += TimelineView_DoubleClick;
                TimelineView4 = new TimelineView(Controller);
                TimelineView4.DoubleClick += TimelineView_DoubleClick;
                TimelineView5 = new TimelineView(Controller);
                TimelineView5.DoubleClick += TimelineView_DoubleClick;

                VisibilityControl = new VisibilityControl(TimelineView);
                VisibilityControl.Visible = true;

                VisibilityControl2 = new VisibilityControl(TimelineView2);
                VisibilityControl2.Visible = true;
                VisibilityControl3 = new VisibilityControl(TimelineView3);
                VisibilityControl3.Visible = true;
                VisibilityControl4 = new VisibilityControl(TimelineView4);
                VisibilityControl4.Visible = true;
                VisibilityControl5 = new VisibilityControl(TimelineView5);
                VisibilityControl5.Visible = true;

                TimelineAutoLoader = new TimelineAutoLoader(Controller);
                TimelineAutoLoader.Start();

                Settings = new PluginSettings(this);
                Settings.AddStringSetting("TimelineTxtFilePath");
                Settings.AddStringSetting("FontString");
                Settings.AddIntSetting("BarHeight");
                Settings.AddIntSetting("BarWidth");
                Settings.AddIntSetting("OpacityPercentage");
                Settings.AddStringSetting("FontString2");
                Settings.AddIntSetting("BarHeight2");
                Settings.AddIntSetting("BarWidth2");
                Settings.AddIntSetting("OpacityPercentage2");
                Settings.AddStringSetting("FontString3");
                Settings.AddIntSetting("BarHeight3");
                Settings.AddIntSetting("BarWidth3");
                Settings.AddIntSetting("OpacityPercentage3");
                Settings.AddStringSetting("FontString4");
                Settings.AddIntSetting("BarHeight4");
                Settings.AddIntSetting("BarWidth4");
                Settings.AddIntSetting("OpacityPercentage4");
                Settings.AddStringSetting("FontString5");
                Settings.AddIntSetting("BarHeight5");
                Settings.AddIntSetting("BarWidth5");
                Settings.AddIntSetting("OpacityPercentage5");

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
                                this.TimelineView2.Invoke(new Action(() => this.TimelineView2.Visible = true));
                                this.TimelineView3.Invoke(new Action(() => this.TimelineView3.Visible = true));
                                this.TimelineView4.Invoke(new Action(() => this.TimelineView4.Visible = true));
                                this.TimelineView5.Invoke(new Action(() => this.TimelineView5.Visible = true));
                            }
                            else
                            {
                                this.TimelineView.Invoke(new Action(() => this.TimelineView.Visible = false));
                                this.TimelineView2.Invoke(new Action(() => this.TimelineView2.Visible = false));
                                this.TimelineView3.Invoke(new Action(() => this.TimelineView3.Visible = false));
                                this.TimelineView4.Invoke(new Action(() => this.TimelineView4.Visible = false));
                                this.TimelineView5.Invoke(new Action(() => this.TimelineView5.Visible = false));
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
            VisibilityControl.Visible = false;
            VisibilityControl2.Visible = false;
            VisibilityControl3.Visible = false;
            VisibilityControl4.Visible = false;
            VisibilityControl5.Visible = false;
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
            VisibilityControl.Visible = checkBoxShowView.Checked;
            VisibilityControl2.Visible = checkBoxShowView.Checked;
            VisibilityControl3.Visible = checkBoxShowView.Checked;
            VisibilityControl4.Visible = checkBoxShowView.Checked;
            VisibilityControl5.Visible = checkBoxShowView.Checked;
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
