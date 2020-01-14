using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACTTimeline
{
    public class TimelineAutoLoader
    {
        private string m_currentzone = string.Empty;

        public bool Autoload { get; set; }


        private Timer timer;
        private ACTPlugin plugin;
        public TimelineAutoLoader(ACTPlugin plugin)
        {
            this.plugin = plugin;
            this.timer = new Timer();
            this.timer.Interval = 5000;
            this.timer.Tick += timer_CheckCurrentZone;
        }

        void timer_CheckCurrentZone(object sender, EventArgs e)
        {
            if (!ActGlobals.oFormActMain.InitActDone)
                return;

            if (!Autoload)
                return;

            var zonename = ActGlobals.oFormActMain.CurrentZone;

            if (zonename.Length == 0)
                return;

            if (m_currentzone != zonename)
            {
                List<string> findList = new List<string>();

                List<char> replacementList = new List<char>();
                replacementList.Add('_');
                replacementList.Add('-');

                foreach (char replacement in replacementList)
                {
                    var safeFileName = zonename;
                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    {
                        safeFileName = safeFileName.Replace(c, replacement);
                    }

                    findList.Add(String.Format("{0}/{1}.txt", Globals.TimelineTxtsRoot, safeFileName));
                    findList.Add(String.Format("{0}/{1}.txt", Globals.TimelineTxtsRoot, safeFileName.Replace($"{replacement} ", $" {replacement} ")));
                }

                bool timelineLoaded = false;

                foreach (string findName in findList)
                {
                    if (System.IO.File.Exists(findName))
                    {
                        plugin.Controller.Paused = true;
                        plugin.Controller.TimelineTxtFilePath = findName;
                        plugin.updateAllVisibility(plugin.checkBoxShowView.Checked);
                        timelineLoaded = true;
                        break;
                    }
                }

                if (!timelineLoaded)
                {
                    plugin.TabPageControl?.buttonUnload?.PerformClick();
                }

                m_currentzone = zonename;
            }
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
