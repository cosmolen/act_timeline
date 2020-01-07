using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

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

            settings.AddControlSetting("PlaySoundByACT", checkBoxPlaySoundByACT);
            settings.AddControlSetting("Autostop", checkBoxAutoStop);
            settings.AddControlSetting("Autoload", checkBoxAutoloadAfterChangeZone);
            settings.AddControlSetting("Autohide", checkBoxAutohide);

            settings.AddControlSetting("OverlayX", udOverlayX);
            settings.AddControlSetting("OverlayY", udOverlayY);
            settings.AddControlSetting("NumberOfRowsToDisplay", udNumRows);
            settings.AddControlSetting("OverlayVisible", CheckBoxOverlayVisible);
            settings.AddControlSetting("ReverseOrder", checkBoxReverseOrder);
            settings.AddControlSetting("MoveOverlayByDrag", checkBoxMoveOverlayByDrag);
            settings.AddControlSetting("ShowOverlayButtons", checkBoxShowOverlayButtons);
            settings.AddControlSetting("Over10", checkBoxOver10);
            settings.AddControlSetting("Under10", checkBoxUnder10);
            settings.AddControlSetting("ShowCasting", checkBoxShowCasting);
            settings.AddControlSetting("PopupMode", checkBoxPopup);

            settings.AddControlSetting("OverlayX2", udOverlayX2);
            settings.AddControlSetting("OverlayY2", udOverlayY2);
            settings.AddControlSetting("NumberOfRowsToDisplay2", udNumRows2);
            settings.AddControlSetting("OverlayVisible2", CheckBoxOverlayVisible2);
            settings.AddControlSetting("ReverseOrder2", checkBoxReverseOrder2);
            settings.AddControlSetting("MoveOverlayByDrag2", checkBoxMoveOverlayByDrag2);
            settings.AddControlSetting("ShowOverlayButtons2", checkBoxShowOverlayButtons2);
            settings.AddControlSetting("Over102", checkBoxOver102);
            settings.AddControlSetting("Under102", checkBoxUnder102);
            settings.AddControlSetting("ShowCasting2", checkBoxShowCasting2);
            settings.AddControlSetting("PopupMode2", checkBoxPopup2);

            settings.AddControlSetting("OverlayX3", udOverlayX3);
            settings.AddControlSetting("OverlayY3", udOverlayY3);
            settings.AddControlSetting("NumberOfRowsToDisplay3", udNumRows3);
            settings.AddControlSetting("OverlayVisible3", CheckBoxOverlayVisible3);
            settings.AddControlSetting("ReverseOrder3", checkBoxReverseOrder3);
            settings.AddControlSetting("MoveOverlayByDrag3", checkBoxMoveOverlayByDrag3);
            settings.AddControlSetting("ShowOverlayButtons3", checkBoxShowOverlayButtons3);
            settings.AddControlSetting("Over103", checkBoxOver103);
            settings.AddControlSetting("Under103", checkBoxUnder103);
            settings.AddControlSetting("ShowCasting3", checkBoxShowCasting3);
            settings.AddControlSetting("PopupMode3", checkBoxPopup3);

            settings.AddControlSetting("OverlayX4", udOverlayX4);
            settings.AddControlSetting("OverlayY4", udOverlayY4);
            settings.AddControlSetting("NumberOfRowsToDisplay4", udNumRows4);
            settings.AddControlSetting("OverlayVisible4", CheckBoxOverlayVisible4);
            settings.AddControlSetting("ReverseOrder4", checkBoxReverseOrder4);
            settings.AddControlSetting("MoveOverlayByDrag4", checkBoxMoveOverlayByDrag4);
            settings.AddControlSetting("ShowOverlayButtons4", checkBoxShowOverlayButtons4);
            settings.AddControlSetting("Over104", checkBoxOver104);
            settings.AddControlSetting("Under104", checkBoxUnder104);
            settings.AddControlSetting("ShowCasting4", checkBoxShowCasting4);
            settings.AddControlSetting("PopupMode4", checkBoxPopup4);

            settings.AddControlSetting("OverlayX5", udOverlayX5);
            settings.AddControlSetting("OverlayY5", udOverlayY5);
            settings.AddControlSetting("NumberOfRowsToDisplay5", udNumRows5);
            settings.AddControlSetting("OverlayVisible5", CheckBoxOverlayVisible5);
            settings.AddControlSetting("ReverseOrder5", checkBoxReverseOrder5);
            settings.AddControlSetting("MoveOverlayByDrag5", checkBoxMoveOverlayByDrag5);
            settings.AddControlSetting("ShowOverlayButtons5", checkBoxShowOverlayButtons5);
            settings.AddControlSetting("Over105", checkBoxOver105);
            settings.AddControlSetting("Under105", checkBoxUnder105);
            settings.AddControlSetting("ShowCasting5", checkBoxShowCasting5);
            settings.AddControlSetting("PopupMode5", checkBoxPopup5);

            plugin.Controller.CurrentTimeUpdate += Controller_CurrentTimeUpdate;
            plugin.Controller.TimelineUpdate += Controller_TimelineUpdate;
            plugin.Controller.PausedUpdate += Controller_PausedUpdate;

            plugin.TimelineView.Move += TimelineView_Move;
            plugin.TimelineView.TimelineFontChanged += TimelineView_TimelineFontChanged;
            plugin.TimelineView.ColumnWidthChanged += TimelineView_ColumnWidthChanged;
            plugin.TimelineView.OpacityChanged += TimelineView_OpacityChanged;

            plugin.TimelineView2.Move += TimelineView_Move;
            plugin.TimelineView2.TimelineFontChanged += TimelineView_TimelineFontChanged;
            plugin.TimelineView2.ColumnWidthChanged += TimelineView_ColumnWidthChanged;
            plugin.TimelineView2.OpacityChanged += TimelineView_OpacityChanged;

            plugin.TimelineView3.Move += TimelineView_Move;
            plugin.TimelineView3.TimelineFontChanged += TimelineView_TimelineFontChanged;
            plugin.TimelineView3.ColumnWidthChanged += TimelineView_ColumnWidthChanged;
            plugin.TimelineView3.OpacityChanged += TimelineView_OpacityChanged;

            plugin.TimelineView4.Move += TimelineView_Move;
            plugin.TimelineView4.TimelineFontChanged += TimelineView_TimelineFontChanged;
            plugin.TimelineView4.ColumnWidthChanged += TimelineView_ColumnWidthChanged;
            plugin.TimelineView4.OpacityChanged += TimelineView_OpacityChanged;

            plugin.TimelineView5.Move += TimelineView_Move;
            plugin.TimelineView5.TimelineFontChanged += TimelineView_TimelineFontChanged;
            plugin.TimelineView5.ColumnWidthChanged += TimelineView_ColumnWidthChanged;
            plugin.TimelineView5.OpacityChanged += TimelineView_OpacityChanged;

            TimelineView_TimelineFontChanged(this, null);
            TimelineView_ColumnWidthChanged(this, null);
            TimelineView_OpacityChanged(this, null);
            Controller_TimelineUpdate(this, null);
            Controller_PausedUpdate(this, null);

            checkBoxPlaySoundByACT_CheckedChanged(this, null);
            checkBoxAutoStop_CheckedChanged(this, null);
            checkBoxAutoloadAfterChangeZone_CheckedChanged(this, null);
            checkBoxAutohide_CheckedChanged(this, null);

            if (plugin.Controller.TimelineTxtFilePath == null || plugin.Controller.TimelineTxtFilePath == "")
            {
                trackBar.Enabled = false;
                buttonRewind.Enabled = false;
                buttonPause.Enabled = false;
                buttonPlay.Enabled = false;
            }
            else
            {
                trackBar.Enabled = true;
                buttonRewind.Enabled = true;
                buttonPause.Enabled = false;
                buttonPlay.Enabled = true;
            }
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

            labelLoadedTimeline.Text = Path.GetFileNameWithoutExtension(timeline.Name);

            buttonUnload.Enabled = true;
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
            NumericUpDown udx = udOverlayX;
            NumericUpDown udy = udOverlayY;
            TimelineView tv = (TimelineView)sender;

            if(sender == plugin.TimelineView2)
            {
                udx = udOverlayX2;
                udy = udOverlayY2;
            }
            else if (sender == plugin.TimelineView3)
            {
                udx = udOverlayX3;
                udy = udOverlayY3;
            }
            else if (sender == plugin.TimelineView4)
            {
                udx = udOverlayX4;
                udy = udOverlayY4;
            }
            else if (sender == plugin.TimelineView5)
            {
                udx = udOverlayX5;
                udy = udOverlayY5;
            }

            updateFromOverlayMove = true;
            udx.Value = tv.Left;
            udy.Value = tv.Top;
            updateFromOverlayMove = false;
        }

        private void buttonResourceDirSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

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
            {
                return Translator.Get("_Resource_dir_not_found");
            }

            string statusText = Translator.Get("_Resource_dir_exists");

            if (!Directory.Exists(Globals.SoundFilesRoot))
            {
                statusText += Translator.Get("_Sound_files_dir_not_found");
                return statusText;
            }
            statusText += String.Format(Translator.Get("_Found_N_sound_files"), Globals.NumberOfSoundFilesInResourcesDir());
            
            if (!Directory.Exists(Globals.TimelineTxtsRoot))
            {
                statusText += Translator.Get("_Timeline_txt_files_dir_not_found");
                return statusText;
            }
            statusText += String.Format(Translator.Get("_Found_N_timeline_txt_files"), Globals.TimelineTxtsInResourcesDir.Length);

            return statusText;
        }

        private void Synchronize()
        {
            labelResourceDirStatus.Text = GenerateDirStatusString();
            
            // update timeline list
            listTimelines.Items.Clear();
            foreach (string fullpath in Globals.TimelineTxtsInResourcesDir)
            {
                listTimelines.Items.Add(Path.GetFileNameWithoutExtension(fullpath));
            }
        }

        private void listTimelines_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonLoad.Enabled = true;
        }

        private void listTimelines_DrawItem(object sender, DrawItemEventArgs e)
        {
            int fontHeight = 8;

            if (e.Index < 0)
                return;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          SystemColors.GradientActiveCaption);

            e.DrawBackground();

            int x = e.Bounds.X + fontHeight;
            int y = e.Bounds.Y + fontHeight;

            e.Graphics.DrawString(this.listTimelines.Items[e.Index].ToString(),
                new Font(FontFamily.GenericSansSerif, fontHeight), Brushes.Black, x, y, StringFormat.GenericDefault);

            e.DrawFocusRectangle();
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

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            string timelineTxtFilePath = (string)listTimelines.SelectedItem;

            try
            {
                plugin.Controller.TimelineTxtFilePath = String.Format("{0}/{1}.txt", Globals.TimelineTxtsRoot, timelineTxtFilePath);

                trackBar.Enabled = true;
                buttonRewind.Enabled = true;
                buttonPlay.Enabled = true;

                plugin.updateAllVisibility(true);
            }
            catch
            {
                // pass
            }
        }

        public void unloadTimeline()
        {
            if (plugin.Controller != null)
            {
                try
                {
                    plugin.Controller.Paused = true;
                    plugin.Controller.CurrentTime = 0;

                    plugin.Controller.TimelineTxtFilePath = String.Empty;

                    plugin.updateAllVisibility(false);
                }
                catch
                {
                    // pass
                }
            }

            labelLoadedTimeline.Text = Translator.Get("labelLoadedTimeline");

            buttonRewind.Enabled = false;
            buttonPause.Enabled = false;
            buttonPlay.Enabled = false;

            buttonUnload.Enabled = false;

            trackBar.Enabled = false;
            trackBar.Maximum = 10;

            labelEndPos.Text = "00:00";
        }

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            unloadTimeline();
        }

        private TimelineView getTimelineView(string name)
        {
            TimelineView result = plugin.TimelineView;

            char overlayNumber = name[name.Length - 1];

            if (overlayNumber == '2')
                result = plugin.TimelineView2;
            else if (overlayNumber == '3')
                result = plugin.TimelineView3;
            else if (overlayNumber == '4')
                result = plugin.TimelineView4;
            else if (overlayNumber == '5')
                result = plugin.TimelineView5;

            return result;
        }

        private void udOverlayX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ud = (NumericUpDown)sender;
            TimelineView tv = getTimelineView(ud.Name);

            if (!updateFromOverlayMove)
                tv.Left = (int)ud.Value;
        }

        private void udOverlayY_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ud = (NumericUpDown)sender;
            TimelineView tv = getTimelineView(ud.Name);

            if (!updateFromOverlayMove)
                tv.Top = (int)ud.Value;
        }

        private void udNumRows_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ud = (NumericUpDown)sender;
            TimelineView tv = getTimelineView(ud.Name);

            tv.NumberOfRowsToDisplay = (int)ud.Value;
        }

        private void checkBoxMoveOverlayByDrag_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.MoveByDrag = cb.Checked;
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
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.ShowOverlayButtons = cb.Checked;
        }

        private void TimelineView_TimelineFontChanged(object sender, EventArgs e)
        {
            buttonFontSelect.Text = plugin.FontString;
            buttonFontSelect2.Text = plugin.FontString2;
            buttonFontSelect3.Text = plugin.FontString3;
            buttonFontSelect4.Text = plugin.FontString4;
            buttonFontSelect5.Text = plugin.FontString5;
        }

        private void buttonFontSelect_Click(object sender, EventArgs e)
        {
            FontDialog fontdialog = new FontDialog();

            Button btn = (Button)sender;
            TimelineView tv = getTimelineView(btn.Name);

            fontdialog.Font = tv.TimelineFont;

            if (fontdialog.ShowDialog() != DialogResult.Cancel)
            {
                tv.TimelineFont = fontdialog.Font;
            }
        }

        private void TimelineView_ColumnWidthChanged(object sender, EventArgs e)
        {
            udBarHeight.Value = plugin.TimelineView.BarHeight;
            udBarWidth.Value = plugin.TimelineView.BarWidth;
            udBarHeight2.Value = plugin.TimelineView2.BarHeight;
            udBarWidth2.Value = plugin.TimelineView2.BarWidth;
            udBarHeight3.Value = plugin.TimelineView3.BarHeight;
            udBarWidth3.Value = plugin.TimelineView3.BarWidth;
            udBarHeight4.Value = plugin.TimelineView4.BarHeight;
            udBarWidth4.Value = plugin.TimelineView4.BarWidth;
            udBarHeight5.Value = plugin.TimelineView5.BarHeight;
            udBarWidth5.Value = plugin.TimelineView5.BarWidth;
        }

        private void udTextWidth_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ud = (NumericUpDown)sender;
            TimelineView tv = getTimelineView(ud.Name);

            tv.BarHeight = (int)ud.Value;
        }

        private void udBarWidth_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown ud = (NumericUpDown)sender;
            TimelineView tv = getTimelineView(ud.Name);

            tv.BarWidth = (int)ud.Value;
        }

        private void TimelineView_OpacityChanged(object sender, EventArgs e)
        {
            int percentage = (int)(plugin.TimelineView.MyOpacity * 100);

            labelCurrOpacity.Text = String.Format("{0}%", percentage);

            percentage = Math.Min(trackBarOpacity.Maximum, percentage);
            percentage = Math.Max(trackBarOpacity.Minimum, percentage);
            trackBarOpacity.Value = percentage;

            percentage = (int)(plugin.TimelineView2.MyOpacity * 100);

            labelCurrOpacity2.Text = String.Format("{0}%", percentage);

            percentage = Math.Min(trackBarOpacity2.Maximum, percentage);
            percentage = Math.Max(trackBarOpacity2.Minimum, percentage);
            trackBarOpacity2.Value = percentage;

            percentage = (int)(plugin.TimelineView3.MyOpacity * 100);

            labelCurrOpacity3.Text = String.Format("{0}%", percentage);

            percentage = Math.Min(trackBarOpacity3.Maximum, percentage);
            percentage = Math.Max(trackBarOpacity3.Minimum, percentage);
            trackBarOpacity3.Value = percentage;

            percentage = (int)(plugin.TimelineView4.MyOpacity * 100);

            labelCurrOpacity4.Text = String.Format("{0}%", percentage);

            percentage = Math.Min(trackBarOpacity4.Maximum, percentage);
            percentage = Math.Max(trackBarOpacity4.Minimum, percentage);
            trackBarOpacity4.Value = percentage;

            percentage = (int)(plugin.TimelineView5.MyOpacity * 100);

            labelCurrOpacity5.Text = String.Format("{0}%", percentage);

            percentage = Math.Min(trackBarOpacity5.Maximum, percentage);
            percentage = Math.Max(trackBarOpacity5.Minimum, percentage);
            trackBarOpacity5.Value = percentage;
        }

        private void trackBarOpacity_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            TimelineView tv = getTimelineView(tb.Name);

            tv.MyOpacity = ((double)tb.Value) / 100;
        }

        private void checkBoxPlaySoundByACT_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineView.PlaySoundByACT = this.checkBoxPlaySoundByACT.Checked;
        }

        private void checkBoxAutoStop_CheckedChanged(object sender, EventArgs e)
        {
            plugin.Controller.AutoStop = this.checkBoxAutoStop.Checked;
        }

        private void checkBoxAutoloadAfterChangeZone_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineAutoLoader.Autoload = this.checkBoxAutoloadAfterChangeZone.Checked;
        }

        private void checkBoxAutohide_CheckedChanged(object sender, EventArgs e)
        {
            plugin.AutoHide = this.checkBoxAutohide.Checked;
        }

        private void checkBoxOver10_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.Over10 = cb.Checked;
        }

        private void checkBoxUnder10_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.Under10 = cb.Checked;
        }

        private void checkBoxShowCasting_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.ShowCasting = cb.Checked;
        }

        private void checkBoxPopup_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.PopupMode = cb.Checked;
        }

        private void CheckBoxOverlayVisible_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            VisibilityControl vc = plugin.VisibilityControl;

            char overlayNumber = cb.Name[cb.Name.Length - 1];

            if (overlayNumber == '2')
                vc = plugin.VisibilityControl2;
            else if (overlayNumber == '3')
                vc = plugin.VisibilityControl3;
            else if (overlayNumber == '4')
                vc = plugin.VisibilityControl4;
            else if (overlayNumber == '5')
                vc = plugin.VisibilityControl5;

            vc.OverlayVisible = cb.Checked;
        }

        private void checkBoxReverseOrder_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = plugin.TimelineView;

            char overlayNumber = cb.Name[cb.Name.Length - 1];

            if (overlayNumber == '2')
                tv = plugin.TimelineView2;
            else if (overlayNumber == '3')
                tv = plugin.TimelineView3;
            else if (overlayNumber == '4')
                tv = plugin.TimelineView4;
            else if (overlayNumber == '5')
                tv = plugin.TimelineView5;

            tv.ReverseOrder = cb.Checked;
        }
    }
}
