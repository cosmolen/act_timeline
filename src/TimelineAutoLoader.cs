using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ACTTimeline
{
    public class TimelineAutoLoader
    {
        private string m_currentzone = string.Empty;

        public bool Autoload { get; set; }


        private Timer timer;
        private TimelineController controller;
        public TimelineAutoLoader(TimelineController _controller)
        {
            controller = _controller;
            timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += timer_CheckCurrentZone;
        }

        void timer_CheckCurrentZone(object sender, EventArgs e)
        {
            if (!Autoload)
                return;
            var zonename = ActGlobals.oFormActMain.CurrentZone;
            if (zonename.Length == 0)
                return;
            if (m_currentzone != zonename)
            {
                var file = String.Format("{0}/{1}.txt", Globals.TimelineTxtsRoot, zonename);
                if (System.IO.File.Exists(file))
                {
                    controller.Paused = true;
                    controller.TimelineTxtFilePath = file;
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
