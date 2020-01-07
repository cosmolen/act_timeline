using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace ACTTimeline
{
    public partial class ACTTabPageControl : UserControl
    {
        private ACTPlugin plugin;
        private bool updateFromOverlayMove;

        public void languagePatch(ControlCollection ctrl)
        {
            foreach (Control c in ctrl)
            {
                if (CultureInfo.InstalledUICulture.Name == "ko-KR")
                {
                    c.Font = new Font("맑은 고딕", 9F, FontStyle.Regular);
                }
                c.Text = Translator.Get(c.Name) == c.Name ? c.Text : Translator.Get(c.Name);
                if (c.HasChildren)
                {
                    if (c.Controls != null)
                    {
                        languagePatch(c.Controls);
                    }
                }
            }
        }

        public ACTTabPageControl(ACTPlugin plugin_)
        {
            InitializeComponent();

            languagePatch(Controls);

            plugin = plugin_;
            updateFromOverlayMove = false;

            var settings = plugin.Settings;
            settings.AddControlSetting("ResourcesDir", textBoxResourceDir);
            settings.AddControlSetting("OverlayX", udOverlayX);
            settings.AddControlSetting("OverlayY", udOverlayY);
            settings.AddControlSetting("NumberOfRowsToDisplay", udNumRows);
            settings.AddControlSetting("MoveOverlayByDrag", checkBoxMoveOverlayByDrag);
            settings.AddControlSetting("ShowOverlayButtons", checkBoxShowOverlayButtons);
            settings.AddControlSetting("PlaySoundByACT", checkBoxPlaySoundByACT);
            settings.AddControlSetting("Autoload", checkBoxAutoloadAfterChangeZone);
            settings.AddControlSetting("Autohide", checkBoxAutohide);

            plugin.TimelineView.Move += TimelineView_Move;
            plugin.TimelineView.TimelineFontChanged += TimelineView_TimelineFontChanged;
            plugin.TimelineView.ColumnWidthChanged += TimelineView_ColumnWidthChanged;
            plugin.TimelineView.OpacityChanged += TimelineView_OpacityChanged;
            plugin.Controller.CurrentTimeUpdate += Controller_CurrentTimeUpdate;
            plugin.Controller.TimelineUpdate += Controller_TimelineUpdate;
            plugin.Controller.PausedUpdate += Controller_PausedUpdate;
            TimelineView_TimelineFontChanged(this, null);
            TimelineView_ColumnWidthChanged(this, null);
            TimelineView_OpacityChanged(this, null);
            Controller_TimelineUpdate(this, null);
            Controller_PausedUpdate(this, null);
        }

        private void Controller_PausedUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_PausedUpdate(sender, e)));
                return;
            }

            buttonPause.Enabled = !plugin.Controller.Paused;
            buttonPlay.Enabled = plugin.Controller.Paused;
        }

        public static string FormatMMSS(double time)
        {
            var mm = Math.Floor(time / 60.0);
            var ss = time - mm * 60.0;
            return String.Format("{0:00}:{1:00}", mm, ss);
        }

        private void Controller_TimelineUpdate(object sender, EventArgs e)
        {
            Timeline timeline = plugin.Controller.Timeline;
            if (timeline == null)
                return;

            double endtime = timeline.EndTime;
            labelEndPos.Text = FormatMMSS(endtime);
            trackBar.Maximum = (int)Math.Ceiling(endtime);

            labelLoadedTimeline.Text = timeline.Name;
        }

        private void Controller_CurrentTimeUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { Controller_CurrentTimeUpdate(sender, e); }));
                return;
            }

            double currtime = plugin.Controller.CurrentTime;
            labelCurrPos.Text = FormatMMSS(currtime);

            int currTimeInt = (int)Math.Floor(currtime);
            if (currTimeInt < trackBar.Maximum)
                trackBar.Value = currTimeInt;
        }

        private void TimelineView_Move(object sender, EventArgs e)
        {
            updateFromOverlayMove = true;
            udOverlayX.Value = plugin.TimelineView.Left;
            udOverlayY.Value = plugin.TimelineView.Top;
            updateFromOverlayMove = false;
        }

        private void buttonResourceDirSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = Translator.Get("_LN_PleaseSelectResDir");

            // if canceled does not setting empty directory
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxResourceDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void textBoxResourceDir_TextChanged(object sender, EventArgs e)
        {
            Globals.ResourceRoot = textBoxResourceDir.Text;
            Synchronize();
        }

        private string GenerateDirStatusString()
        {
            if (!Directory.Exists(Globals.ResourceRoot))
                return Translator.Get("_LN_ResNotFound");

            string statusText = Translator.Get("_LN_ResFound");

            //"Sound files dir not found!";
            if (!Directory.Exists(Globals.SoundFilesRoot))
                statusText += Translator.Get("_LN_WavNotFound"); 
            // "Found {0} sound files. "
            else
                statusText += string.Format(Translator.Get("_LN_WavFound"), Globals.NumberOfSoundFilesInResourcesDir());

            // "Timeline txt files dir not found!"
            if (!Directory.Exists(Globals.TimelineTxtsRoot))
                statusText += Translator.Get("_LN_TimelineNotFound");
            // "Found {0} timeline txt files."
            else
                statusText += string.Format(Translator.Get("_LN_TimelineFound"), Globals.TimelineTxtsInResourcesDir.Length);

            return statusText;
        }

        private void Synchronize()
        {
            labelResourceDirStatus.Text = GenerateDirStatusString();

            // update timeline list
            ListItems.Controls.Clear();
            foreach (string fullpath in Globals.TimelineTxtsInResourcesDir)
            {
                CheckBox btn = new CheckBox();
                btn.Text = Path.GetFileName(fullpath);
                btn.Appearance = Appearance.Button;
                btn.Click += Btn_CheckedChanged;
                btn.Dock = DockStyle.Top;
                btn.BackColor = Color.Transparent;
                ListItems.Controls.Add(btn);
            }
        }

        private void Btn_CheckedChanged(object sender, EventArgs e)
        {
            foreach(Control c in ListItems.Controls)
            {
                ((CheckBox)c).Checked = false;
            }

            ((CheckBox)sender).Checked = true;
            string timelineTxtFilePath = ((CheckBox)sender).Text;
            plugin.Controller.TimelineTxtFilePath = string.Format("{0}/{1}", Globals.TimelineTxtsRoot, timelineTxtFilePath);
        }

        private void buttonResourceDirOpen_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Globals.ResourceRoot))
                Process.Start(Globals.ResourceRoot);
        }

        private void buttonRefreshList_Click(object sender, EventArgs e)
        {
            Synchronize();
        }

        private string getSelectedItem()
        {
            foreach(Control c in ListItems.Controls)
            {
                if (((CheckBox)c).Checked)
                    return c.Text;
            }
            return "";
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            string timelineTxtFilePath = getSelectedItem();

            try
            {
                plugin.Controller.TimelineTxtFilePath = String.Format("{0}/{1}", Globals.TimelineTxtsRoot, timelineTxtFilePath);

                buttonRewind.Enabled = true;
                buttonPlay.Enabled = true;
            }
            catch (Exception ex)
            {
                // pass
            }
        }

        private void udOverlayX_ValueChanged(object sender, EventArgs e)
        {
            if (!updateFromOverlayMove)
                plugin.TimelineView.Left = (int)udOverlayX.Value;
        }

        private void udOverlayY_ValueChanged(object sender, EventArgs e)
        {
            if (!updateFromOverlayMove)
                plugin.TimelineView.Top = (int)udOverlayY.Value;
        }

        private void udNumRows_ValueChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.NumberOfRowsToDisplay = (int)udNumRows.Value;
        }

        private void checkBoxMoveOverlayByDrag_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.MoveByDrag = checkBoxMoveOverlayByDrag.Checked;
        }

        private void trackbar_Scroll(object sender, EventArgs e)
        {
            plugin.Controller.CurrentTime = (int)trackBar.Value;
        }

        private void buttonRewind_Click(object sender, EventArgs e)
        {
            plugin.Controller.CurrentTime = 0;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            plugin.Controller.Paused = true;
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            plugin.Controller.Paused = false;
        }

        private void checkBoxShowOverlayButtons_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.ShowOverlayButtons = checkBoxShowOverlayButtons.Checked;
        }

        private void TimelineView_TimelineFontChanged(object sender, EventArgs e)
        {
            buttonFontSelect.Text = plugin.FontString;
        }

        private void buttonFontSelect_Click(object sender, EventArgs e)
        {
            FontDialog fontdialog = new FontDialog();
            fontdialog.Font = plugin.TimelineView.TimelineFont;

            if (fontdialog.ShowDialog() != DialogResult.Cancel)
            {
                plugin.TimelineView.TimelineFont = fontdialog.Font;
            }
        }

        private void TimelineView_ColumnWidthChanged(object sender, EventArgs e)
        {
            udTextWidth.Value = plugin.TimelineView.TextWidth;
            udBarWidth.Value = plugin.TimelineView.BarWidth;
        }

        private void udTextWidth_ValueChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.TextWidth = (int)udTextWidth.Value;
        }

        private void udBarWidth_ValueChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.BarWidth = (int)udBarWidth.Value;
        }

        private void TimelineView_OpacityChanged(object sender, EventArgs e)
        {
            int percentage = (int)(plugin.TimelineView.MyOpacity * 100);

            labelCurrOpacity.Text = String.Format("{0}%", percentage);

            percentage = Math.Min(trackBarOpacity.Maximum, percentage);
            percentage = Math.Max(trackBarOpacity.Minimum, percentage);
            trackBarOpacity.Value = percentage;
        }

        private void trackBarOpacity_Scroll(object sender, EventArgs e)
        {
            plugin.TimelineView.MyOpacity = ((double)trackBarOpacity.Value) / 100;
        }

        private void checkBoxPlaySoundByACT_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.PlaySoundByACT = checkBoxPlaySoundByACT.Checked;
        }

        private void checkBoxAutoloadAfterChangeZone_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineAutoLoader.Autoload = checkBoxAutoloadAfterChangeZone.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            plugin.AutoHide = this.checkBoxAutohide.Checked;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
