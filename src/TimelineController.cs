using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACTTimeline
{
    public class TimelineController
    {
        public bool AutoStop { get; set; }

        private bool solidBar;
        public bool SolidBar
        {
            get { return solidBar; }
            set
            {
                solidBar = value;
                OnSolidBarUpdate();
            }
        }

        public event EventHandler SolidBarUpdate;
        public void OnSolidBarUpdate()
        {
            OnCurrentTimeUpdate();
            SolidBarUpdate?.Invoke(this, EventArgs.Empty);
        }

        public Color DefaultBarColor { get; private set; }
        private Color barColor;
        public Color BarColor
        {
            get { return barColor; }
            set
            {
                barColor = value;
                OnBarColorUpdate();
            }
        }

        public event EventHandler BarColorUpdate;
        public void OnBarColorUpdate()
        {
            OnCurrentTimeUpdate();
            BarColorUpdate?.Invoke(this, EventArgs.Empty);
        }

        public Color DefaultBarEmColor { get; private set; }
        private Color barEmColor;
        public Color BarEmColor
        {
            get { return barEmColor; }
            set
            {
                barEmColor = value;
                OnBarEmColorUpdate();
            }
        }

        public event EventHandler BarEmColorUpdate;
        public void OnBarEmColorUpdate()
        {
            OnCurrentTimeUpdate();
            BarEmColorUpdate?.Invoke(this, EventArgs.Empty);
        }

        public Color DefaultDurationBackColor { get; private set; }
        private Color durationBackColor;
        public Color DurationBackColor
        {
            get { return durationBackColor; }
            set
            {
                durationBackColor = value;
                OnDurationBackColorUpdate();
            }
        }

        public event EventHandler DurationBackColorUpdate;
        public void OnDurationBackColorUpdate()
        {
            OnCurrentTimeUpdate();
            DurationBackColorUpdate?.Invoke(this, EventArgs.Empty);
        }

        public Color DefaultDurationColor { get; private set; }
        private Color durationColor;
        public Color DurationColor
        {
            get { return durationColor; }
            set
            {
                durationColor = value;
                OnDurationColorUpdate();
            }
        }

        public event EventHandler DurationColorUpdate;
        public void OnDurationColorUpdate()
        {
            OnCurrentTimeUpdate();
            DurationColorUpdate?.Invoke(this, EventArgs.Empty);
        }

        public Color DefaultFontColor { get; private set; }
        private Color fontColor;
        public Color FontColor
        {
            get { return fontColor; }
            set
            {
                fontColor = value;
                OnFontColorUpdate();
            }
        }

        public event EventHandler FontColorUpdate;
        public void OnFontColorUpdate()
        {
            OnCurrentTimeUpdate();
            FontColorUpdate?.Invoke(this, EventArgs.Empty);
        }

        public Color DefaultFontStrokeColor { get; private set; }
        private Color fontStrokeColor;
        public Color FontStrokeColor
        {
            get { return fontStrokeColor; }
            set
            {
                fontStrokeColor = value;
                OnFontStrokeColorUpdate();
            }
        }

        public event EventHandler FontStrokeColorUpdate;
        public void OnFontStrokeColorUpdate()
        {
            OnCurrentTimeUpdate();
            FontStrokeColorUpdate?.Invoke(this, EventArgs.Empty);
        }

        private string timelineTxtFilePath;
        public string TimelineTxtFilePath
        {
            get { return timelineTxtFilePath; }
            set
            {
                if (value == null)
                    return;

                if (value == String.Empty)
                {
                    timelineTxtFilePath = value;
                    return;
                }

                try
                {
                    if (!System.IO.File.Exists(value))
                        throw new ResourceNotFoundException(value);

                    timelineTxtFilePath = value;
                    Timeline = TimelineLoader.LoadFromFile(timelineTxtFilePath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Failed to load timeline. Error: {0}", e.Message), "ACT Timeline Plugin");
                }
            }
        }

        private Timeline timeline;
        public Timeline Timeline
        {
            get { return timeline; }
            set
            {
                timeline = value;
                OnTimelineUpdate();
            }
        }

        public event EventHandler TimelineUpdate;
        public void OnTimelineUpdate()
        {
            Paused = true;
            CurrentTime = 0;

            TimelineUpdate?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CurrentTimeUpdate;
        public void OnCurrentTimeUpdate()
        {
            CurrentTimeUpdate?.Invoke(this, EventArgs.Empty);
        }

        private RelativeClock relativeClock;

        private double currentTime;
        public double CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                relativeClock.CurrentTime = value;
                if (value == 0.0)
                    timeline.ResetAllAlerts();
                OnCurrentTimeUpdate();
            }
        }
        
        private Timer timer;

        private bool paused;
        public bool Paused {
            get { return paused; }
            set {
                if (paused == value)
                    return;

                paused = value;
                OnPausedUpdate();
            }
        }

        public event EventHandler PausedUpdate;
        public void OnPausedUpdate()
        {
            if (!paused)
                relativeClock.CurrentTime = currentTime;

            PausedUpdate?.Invoke(this, EventArgs.Empty);
        }

        private List<string> keywordsEnd;

        private ACTPlugin plugin;

        public TimelineController(ACTPlugin plugin_)
        {
            plugin = plugin_;

            solidBar = false;

            DefaultBarColor = Color.FromArgb(64, 80, 176);
            DefaultBarEmColor = Color.Red;
            DefaultDurationBackColor = Color.FromArgb(247, 154, 0);
            DefaultDurationColor = Color.White;
            DefaultFontColor = Color.White;
            DefaultFontStrokeColor = Color.Black;

            barColor = DefaultBarColor;
            barEmColor = DefaultBarEmColor;
            durationBackColor = DefaultDurationBackColor;
            durationColor = DefaultDurationColor;
            fontColor = DefaultFontColor;
            FontStrokeColor = DefaultFontStrokeColor;

            timer = new Timer();
            timer.Tick += (object sender, EventArgs e) => { Synchronize(); };
            timer.Interval = 50;
            timer.Start();

            relativeClock = new RelativeClock();
            Paused = true;

            keywordsEnd = new List<string>();

            keywordsEnd.Add("00:0139:");
            keywordsEnd.Add("01:Changed Zone");

            keywordsEnd.Add("입찰을 진행하십시오.");
            keywordsEnd.Add("Cast your lot.");
            keywordsEnd.Add("ロットを行ってください。");

            keywordsEnd.Add("공략을 종료했습니다.");
            keywordsEnd.Add("has ended.");
            keywordsEnd.Add("の攻略を終了した。");

            keywordsEnd.Add("준비 확인을 시작했습니다.");
            keywordsEnd.Add("レディチェックを開始しました。");

            ActGlobals.oFormActMain.OnLogLineRead += act_OnLogLineRead;
        }

        private void act_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (isImport || timeline == null)
                return;

            string line = logInfo.logLine;

            TimelineAnchor anchor = timeline.FindAnchorMatchingLogline(CurrentTime, logInfo.logLine);
            if (anchor != null)
            {
                if (anchor.Jump == 0)
                {
                    CurrentTime = 0;
                    Paused = true;
                }
                else
                {
                    CurrentTime = anchor.Jump > 0 ? anchor.Jump : anchor.TimeFromStart;
                    Paused = false;
                }
            }

            // echo commands
            if (logInfo.logLine.Contains("/timeline show"))
            {
                plugin.checkBoxShowView.Checked = true;
            }

            if (logInfo.logLine.Contains("/timeline hide"))
            {
                plugin.checkBoxShowView.Checked = false;
            }

            if (logInfo.logLine.Contains("/timeline rewind"))
            {
                CurrentTime = 0;
            }

            if (logInfo.logLine.Contains("/timeline pause"))
            {
                Paused = true;
            }

            if (logInfo.logLine.Contains("/timeline play") || logInfo.logLine.Contains("/timeline start"))
            {
                if (timelineTxtFilePath != String.Empty && timeline != null)
                {
                    Paused = false;
                }
            }

            if (logInfo.logLine.Contains("/timeline stop"))
            {
                CurrentTime = 0;
                Paused = true;
            }

            if (logInfo.logLine.Contains("/timeline unload") || logInfo.logLine.Contains("/timeline close"))
            {
                plugin.tabPageControl?.buttonUnload?.PerformClick();
            }

            // auto stop
            if (AutoStop)
            {
                foreach (string k in keywordsEnd)
                {
                    if (logInfo.logLine.Contains(k))
                    {
                        CurrentTime = 0;
                        Paused = true;
                    }
                }
            }
        }

        public void Stop()
        {
            timer.Stop();
            timeline = null;

            ActGlobals.oFormActMain.OnLogLineRead -= act_OnLogLineRead;
        }

        private void Synchronize()
        {
            if (timeline == null)
                return;

            if (Paused)
                return;

            currentTime = relativeClock.CurrentTime;
            OnCurrentTimeUpdate();
        }
    }
}
