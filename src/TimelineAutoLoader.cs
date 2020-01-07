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

        void resetTimeline()
        {
            try
            {
                controller.Paused = true;
                controller.CurrentTime = 0;
            }
            catch (Exception ex)
            {
                // ignore exception
            }
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

                foreach (string findName in findList)
                {
                    if (System.IO.File.Exists(findName))
                    {
                        controller.Paused = true;
                        controller.TimelineTxtFilePath = findName;
                        break;
                    }
                    else
                    {
                        resetTimeline();
                    }
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
