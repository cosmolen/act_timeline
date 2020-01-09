using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace ACTTimeline
{
    public partial class ACTTabPageControl : UserControl
    {
        private ACTPlugin plugin;

        private bool updateFromOverlayMove;
        private const int NumberOfOverlays = 5;

        private List<NumericUpDown> udOverlayXList;
        private List<NumericUpDown> udOverlayYList;
        private List<NumericUpDown> udNumRowsList;
        private List<NumericUpDown> udBarHeightList;
        private List<NumericUpDown> udBarWidthList;
        private List<TrackBar> trackBarOpacityList;
        private List<Button> buttonFontSelectList;
        private List<CheckBox> CheckBoxOverlayVisibleList;
        private List<CheckBox> checkBoxReverseOrderList;
        private List<CheckBox> checkBoxMoveOverlayByDragList;
        private List<CheckBox> checkBoxShowOverlayButtonsList;
        private List<CheckBox> checkBoxOver10List;
        private List<CheckBox> checkBoxUnder10List;
        private List<CheckBox> checkBoxShowCastingList;
        private List<CheckBox> checkBoxPopupList;
        private List<Label> labelCurrOpacityList;

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

            udOverlayXList = new List<NumericUpDown>();
            udOverlayYList = new List<NumericUpDown>();
            udNumRowsList = new List<NumericUpDown>();
            udBarHeightList = new List<NumericUpDown>();
            udBarWidthList = new List<NumericUpDown>();
            trackBarOpacityList = new List<TrackBar>();
            buttonFontSelectList = new List<Button>();
            CheckBoxOverlayVisibleList = new List<CheckBox>();
            checkBoxReverseOrderList = new List<CheckBox>();
            checkBoxMoveOverlayByDragList = new List<CheckBox>();
            checkBoxShowOverlayButtonsList = new List<CheckBox>();
            checkBoxOver10List = new List<CheckBox>();
            checkBoxUnder10List = new List<CheckBox>();
            checkBoxShowCastingList = new List<CheckBox>();
            checkBoxPopupList = new List<CheckBox>();
            labelCurrOpacityList = new List<Label>();

            for (int i = 0; i < NumberOfOverlays; i++)
            {
                string numbering = i > 0 ? (i + 1).ToString() : "";

                NumericUpDown udOverlayXTemp = (NumericUpDown)this.GetType().GetField("udOverlayX" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                NumericUpDown udOverlayYTemp = (NumericUpDown)this.GetType().GetField("udOverlayY" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                NumericUpDown udNumRowsTemp = (NumericUpDown)this.GetType().GetField("udNumRows" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                NumericUpDown udBarHeightTemp = (NumericUpDown)this.GetType().GetField("udBarHeight" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                NumericUpDown udBarWidthTemp = (NumericUpDown)this.GetType().GetField("udBarWidth" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                TrackBar trackBarOpacityTemp = (TrackBar)this.GetType().GetField("trackBarOpacity" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                Button buttonFontSelectTemp = (Button)this.GetType().GetField("buttonFontSelect" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox CheckBoxOverlayVisibleTemp = (CheckBox)this.GetType().GetField("CheckBoxOverlayVisible" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxReverseOrderTemp = (CheckBox)this.GetType().GetField("checkBoxReverseOrder" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxMoveOverlayByDragTemp = (CheckBox)this.GetType().GetField("checkBoxMoveOverlayByDrag" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxShowOverlayButtonsTemp = (CheckBox)this.GetType().GetField("checkBoxShowOverlayButtons" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxOver10Temp = (CheckBox)this.GetType().GetField("checkBoxOver10" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxUnder10Temp = (CheckBox)this.GetType().GetField("checkBoxUnder10" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxShowCastingTemp = (CheckBox)this.GetType().GetField("checkBoxShowCasting" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                CheckBox checkBoxPopupTemp = (CheckBox)this.GetType().GetField("checkBoxPopup" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                Label labelCurrOpacityTemp = (Label)this.GetType().GetField("labelCurrOpacity" + numbering, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

                udOverlayXList.Add(udOverlayXTemp);
                udOverlayYList.Add(udOverlayYTemp);
                udNumRowsList.Add(udNumRowsTemp);
                udBarHeightList.Add(udBarHeightTemp);
                udBarWidthList.Add(udBarWidthTemp);
                trackBarOpacityList.Add(trackBarOpacityTemp);
                buttonFontSelectList.Add(buttonFontSelectTemp);
                CheckBoxOverlayVisibleList.Add(CheckBoxOverlayVisibleTemp);
                checkBoxReverseOrderList.Add(checkBoxReverseOrderTemp);
                checkBoxMoveOverlayByDragList.Add(checkBoxMoveOverlayByDragTemp);
                checkBoxShowOverlayButtonsList.Add(checkBoxShowOverlayButtonsTemp);
                checkBoxOver10List.Add(checkBoxOver10Temp);
                checkBoxUnder10List.Add(checkBoxUnder10Temp);
                checkBoxShowCastingList.Add(checkBoxShowCastingTemp);
                checkBoxPopupList.Add(checkBoxPopupTemp);
                labelCurrOpacityList.Add(labelCurrOpacityTemp);

                settings.AddControlSetting("OverlayX" + numbering, udOverlayXTemp);
                settings.AddControlSetting("OverlayY" + numbering, udOverlayYTemp);
                settings.AddControlSetting("NumberOfRowsToDisplay" + numbering, udNumRowsTemp);
                settings.AddControlSetting("BarHeight" + numbering, udBarHeightTemp);
                settings.AddControlSetting("BarWidth" + numbering, udBarWidthTemp);
                settings.AddControlSetting("FontString" + numbering, buttonFontSelectTemp);
                settings.AddControlSetting("OpacityPercentage" + numbering, trackBarOpacityTemp);
                settings.AddControlSetting("OverlayVisible" + numbering, CheckBoxOverlayVisibleTemp);
                settings.AddControlSetting("ReverseOrder" + numbering, checkBoxReverseOrderTemp);
                settings.AddControlSetting("MoveOverlayByDrag" + numbering, checkBoxMoveOverlayByDragTemp);
                settings.AddControlSetting("ShowOverlayButtons" + numbering, checkBoxShowOverlayButtonsTemp);
                settings.AddControlSetting("Over10" + numbering, checkBoxOver10Temp);
                settings.AddControlSetting("Under10" + numbering, checkBoxUnder10Temp);
                settings.AddControlSetting("ShowCasting" + numbering, checkBoxShowCastingTemp);
                settings.AddControlSetting("PopupMode" + numbering, checkBoxPopupTemp);

                plugin.TimelineViewList[i].Move += TimelineView_Move;
                plugin.TimelineViewList[i].TimelineFontChanged += TimelineView_TimelineFontChanged;
                plugin.TimelineViewList[i].ColumnWidthChanged += TimelineView_ColumnWidthChanged;
                plugin.TimelineViewList[i].OpacityChanged += TimelineView_OpacityChanged;
            }

            settings.AddControlSetting("BarColor", textBoxBarColor);
            settings.AddControlSetting("BarEmColor", textBoxBarEmColor);
            settings.AddControlSetting("DurationBackColor", textBoxDurationBackColor);
            settings.AddControlSetting("DurationColor", textBoxDurationColor);
            settings.AddControlSetting("FontColor", textBoxFontColor);
            settings.AddControlSetting("FontStrokeColor", textBoxFontStrokeColor);
            settings.AddControlSetting("SolidBar", checkBoxSolidBar);

            plugin.Controller.BarColorUpdate += Controller_BarColorUpdate;
            plugin.Controller.BarEmColorUpdate += Controller_BarEmColorUpdate;
            plugin.Controller.DurationBackColorUpdate += Controller_DurationBackColorUpdate;
            plugin.Controller.DurationColorUpdate += Controller_DurationColorUpdate;
            plugin.Controller.FontColorUpdate += Controller_FontColorUpdate;
            plugin.Controller.FontStrokeColorUpdate += Controller_FontStrokeColorUpdate;
            plugin.Controller.SolidBarUpdate += Controller_SolidBarUpdate;

            plugin.Controller.CurrentTimeUpdate += Controller_CurrentTimeUpdate;
            plugin.Controller.TimelineUpdate += Controller_TimelineUpdate;
            plugin.Controller.PausedUpdate += Controller_PausedUpdate;

            TimelineView_TimelineFontChanged(this, null);
            TimelineView_ColumnWidthChanged(this, null);
            TimelineView_OpacityChanged(this, null);

            Controller_BarColorUpdate(this, null);
            Controller_BarEmColorUpdate(this, null);
            Controller_DurationBackColorUpdate(this, null);
            Controller_DurationColorUpdate(this, null);
            Controller_FontColorUpdate(this, null);
            Controller_FontStrokeColorUpdate(this, null);

            Controller_SolidBarUpdate(this, null);

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

        private static string ColorToString(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void Controller_BarColorUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_BarColorUpdate(sender, e)));
                return;
            }

            Color c = plugin.Controller.BarColor;

            textBoxBarColor.Text = ColorToString(c);
            buttonBarColor.BackColor = c;
        }

        private void Controller_BarEmColorUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_BarEmColorUpdate(sender, e)));
                return;
            }

            Color c = plugin.Controller.BarEmColor;

            textBoxBarEmColor.Text = ColorToString(c);
            buttonBarEmColor.BackColor = c;
        }

        private void Controller_DurationBackColorUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_DurationBackColorUpdate(sender, e)));
                return;
            }

            Color c = plugin.Controller.DurationBackColor;

            textBoxDurationBackColor.Text = ColorToString(c);
            buttonDurationBackColor.BackColor = c;
        }

        private void Controller_DurationColorUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_DurationColorUpdate(sender, e)));
                return;
            }

            Color c = plugin.Controller.DurationColor;

            textBoxDurationColor.Text = ColorToString(c);
            buttonDurationColor.BackColor = c;
        }

        private void Controller_FontColorUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_FontColorUpdate(sender, e)));
                return;
            }

            Color c = plugin.Controller.FontColor;

            textBoxFontColor.Text = ColorToString(c);
            buttonFontColor.BackColor = c;
        }

        private void Controller_FontStrokeColorUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_FontStrokeColorUpdate(sender, e)));
                return;
            }

            Color c = plugin.Controller.FontStrokeColor;

            textBoxFontStrokeColor.Text = ColorToString(c);
            buttonFontStrokeColor.BackColor = c;
        }

        private void Controller_SolidBarUpdate(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Controller_SolidBarUpdate(sender, e)));
                return;
            }

            checkBoxSolidBar.Checked = plugin.Controller.SolidBar;
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

            for (int i = 0; i < plugin.NumberOfOverlays; i++)
            {
                if (sender == plugin.TimelineViewList[i])
                {
                    udx = udOverlayXList[i];
                    udy = udOverlayYList[i];
                    break;
                }
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
            buttonLoad.Enabled = false;

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
            TimelineView result = plugin.TimelineViewList[0];

            int overlayNumber = (int)Char.GetNumericValue(name[name.Length - 1]);

            if (overlayNumber > 1)
                result = plugin.TimelineViewList[overlayNumber - 1];

            return result;
        }

        private VisibilityControl getVisibilityControl(string name)
        {
            VisibilityControl result = plugin.VisibilityControlList[0];

            int overlayNumber = (int)Char.GetNumericValue(name[name.Length - 1]);

            if (overlayNumber > 1)
                result = plugin.VisibilityControlList[overlayNumber - 1];

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

        private void trackbar_ValueChanged(object sender, EventArgs e)
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
            for (int i = 0; i < plugin.NumberOfOverlays; i++)
            {
                buttonFontSelectList[i].Text = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(plugin.TimelineViewList[i].TimelineFont);
            }
        }

        private void buttonFontSelect_Click(object sender, EventArgs e)
        {
            FontDialog fontdialog = new FontDialog();

            Button btn = (Button)sender;
            TimelineView tv = getTimelineView(btn.Name);

            fontdialog.Font = tv.TimelineFont;

            if (fontdialog.ShowDialog() == DialogResult.OK)
            {
                tv.TimelineFont = fontdialog.Font;
            }
        }

        private void buttonFontSelect_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                TimelineView tv = getTimelineView(btn.Name);

                tv.TimelineFont = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString(btn.Text) as Font;
            }
            catch
            {
                // pass
            }
        }

        private void TimelineView_ColumnWidthChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < plugin.NumberOfOverlays; i++)
            {
                udBarHeightList[i].Value = plugin.TimelineViewList[i].BarHeight;
                udBarWidthList[i].Value = plugin.TimelineViewList[i].BarWidth;
            }
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
            for (int i = 0; i < plugin.NumberOfOverlays; i++)
            {
                int percentage = (int)(plugin.TimelineViewList[i].MyOpacity * 100);

                percentage = Math.Min(trackBarOpacityList[i].Maximum, percentage);
                percentage = Math.Max(trackBarOpacityList[i].Minimum, percentage);

                trackBarOpacityList[i].Value = percentage;
                labelCurrOpacityList[i].Text = String.Format("{0}%", percentage);
            }
        }

        private void trackBarOpacity_ValueChanged(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            TimelineView tv = getTimelineView(tb.Name);

            tv.MyOpacity = (double) tb.Value / 100;
        }

        private void checkBoxPlaySoundByACT_CheckedChanged(object sender, EventArgs e)
        {
            plugin.TimelineViewList[0].PlaySoundByACT = this.checkBoxPlaySoundByACT.Checked;
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

        private void checkBoxReverseOrder_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TimelineView tv = getTimelineView(cb.Name);

            tv.ReverseOrder = cb.Checked;
        }

        private void CheckBoxOverlayVisible_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            VisibilityControl vc = getVisibilityControl(cb.Name);

            vc.OverlayVisible = cb.Checked;
        }

        private void buttonColorClick(string setting, Button b, TextBox tb)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color c = dialog.Color;
                plugin.Controller.GetType().GetProperty(setting).SetValue(plugin.Controller, c);
                tb.Text = ColorToString(c);
                b.BackColor = c;
            }
        }

        private void textBoxColorApply(string setting, Button b, TextBox tb)
        {
            Color c = (Color)plugin.Controller.GetType().GetProperty(setting).GetValue(plugin.Controller);

            try
            {
                c = ColorTranslator.FromHtml(tb.Text);
            }
            catch
            {
                // pass
            }

            plugin.Controller.GetType().GetProperty(setting).SetValue(plugin.Controller, c);
            tb.Text = ColorToString(c);
            b.BackColor = c;
        }

        private void textBoxColorTextChanged(string setting, Button b, TextBox tb)
        {
            Color c = (Color)plugin.Controller.GetType().GetProperty(setting).GetValue(plugin.Controller);

            try
            {
                c = ColorTranslator.FromHtml(tb.Text);
            }
            catch
            {
                c = (Color)plugin.Controller.GetType().GetProperty("Default" + setting).GetValue(plugin.Controller); ;
            }

            if (!tb.Focused)
            {
                plugin.Controller.GetType().GetProperty(setting).SetValue(plugin.Controller, c);
            }

            b.BackColor = c;
        }

        private bool? textBoxColorKeyPress(KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (c == 13)
                return null;

            if ((ModifierKeys & Keys.Control) != Keys.Control && c != '#' && c != '\b' && !((c <= 57 && c >= 48) || (c <= 70 && c >= 65) || (c <= 102 && c >= 92)))
                return true;

            return false;
        }

        // bar color
        private void buttonBarColor_Click(object sender, EventArgs e)
        {
            buttonColorClick("BarColor", buttonBarColor, textBoxBarColor);
        }

        private void textBoxBarColor_Apply(object sender, EventArgs e)
        {
            textBoxColorApply("BarColor", buttonBarColor, textBoxBarColor);
        }

        private void textBoxBarColor_TextChanged(object sender, EventArgs e)
        {
            textBoxColorTextChanged("BarColor", buttonBarColor, textBoxBarColor);
        }

        private void textBoxBarColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool? validated = textBoxColorKeyPress(e);

            if (validated == null)
            {
                textBoxBarColor_Apply(this, null);
            }
            else
            {
                e.Handled = (bool)validated;
            }
        }

        private void buttonResetBarColor_Click(object sender, EventArgs e)
        {
            plugin.Controller.BarColor = plugin.Controller.DefaultBarColor;
        }

        // bar em color
        private void buttonBarEmColor_Click(object sender, EventArgs e)
        {
            buttonColorClick("BarEmColor", buttonBarEmColor, textBoxBarEmColor);
        }

        private void textBoxBarEmColor_Apply(object sender, EventArgs e)
        {
            textBoxColorApply("BarEmColor", buttonBarEmColor, textBoxBarEmColor);
        }

        private void textBoxBarEmColor_TextChanged(object sender, EventArgs e)
        {
            textBoxColorTextChanged("BarEmColor", buttonBarEmColor, textBoxBarEmColor);
        }

        private void textBoxBarEmColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool? validated = textBoxColorKeyPress(e);

            if (validated == null)
            {
                textBoxBarEmColor_Apply(this, null);
            }
            else
            {
                e.Handled = (bool)validated;
            }
        }

        private void buttonResetBarEmColor_Click(object sender, EventArgs e)
        {
            plugin.Controller.BarEmColor = plugin.Controller.DefaultBarEmColor;
        }

        // duration back color
        private void buttonDurationBackColor_Click(object sender, EventArgs e)
        {
            buttonColorClick("DurationBackColor", buttonDurationBackColor, textBoxDurationBackColor);
        }

        private void textBoxDurationBackColor_Apply(object sender, EventArgs e)
        {
            textBoxColorApply("DurationBackColor", buttonDurationBackColor, textBoxDurationBackColor);
        }

        private void textBoxDurationBackColor_TextChanged(object sender, EventArgs e)
        {
            textBoxColorTextChanged("DurationBackColor", buttonDurationBackColor, textBoxDurationBackColor);
        }

        private void textBoxDurationBackColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool? validated = textBoxColorKeyPress(e);

            if (validated == null)
            {
                textBoxDurationBackColor_Apply(this, null);
            }
            else
            {
                e.Handled = (bool)validated;
            }
        }

        private void buttonResetDurationBackColor_Click(object sender, EventArgs e)
        {
            plugin.Controller.DurationBackColor = plugin.Controller.DefaultDurationBackColor;
        }

        // duration color
        private void buttonDurationColor_Click(object sender, EventArgs e)
        {
            buttonColorClick("DurationColor", buttonDurationColor, textBoxDurationColor);
        }

        private void textBoxDurationColor_Apply(object sender, EventArgs e)
        {
            textBoxColorApply("DurationColor", buttonDurationColor, textBoxDurationColor);
        }

        private void textBoxDurationColor_TextChanged(object sender, EventArgs e)
        {
            textBoxColorTextChanged("DurationColor", buttonDurationColor, textBoxDurationColor);
        }

        private void textBoxDurationColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool? validated = textBoxColorKeyPress(e);

            if (validated == null)
            {
                textBoxDurationColor_Apply(this, null);
            }
            else
            {
                e.Handled = (bool)validated;
            }
        }

        private void buttonResetDurationColor_Click(object sender, EventArgs e)
        {
            plugin.Controller.DurationColor = plugin.Controller.DefaultDurationColor;
        }

        // font color
        private void buttonFontColor_Click(object sender, EventArgs e)
        {
            buttonColorClick("FontColor", buttonFontColor, textBoxFontColor);
        }

        private void textBoxFontColor_Apply(object sender, EventArgs e)
        {
            textBoxColorApply("FontColor", buttonFontColor, textBoxFontColor);
        }

        private void textBoxFontColor_TextChanged(object sender, EventArgs e)
        {
            textBoxColorTextChanged("FontColor", buttonFontColor, textBoxFontColor);
        }

        private void textBoxFontColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool? validated = textBoxColorKeyPress(e);

            if (validated == null)
            {
                textBoxFontColor_Apply(this, null);
            }
            else
            {
                e.Handled = (bool)validated;
            }
        }

        private void buttonResetFontColor_Click(object sender, EventArgs e)
        {
            plugin.Controller.FontColor = plugin.Controller.DefaultFontColor;
        }

        // font stroke color
        private void buttonFontStrokeColor_Click(object sender, EventArgs e)
        {
            buttonColorClick("FontStrokeColor", buttonFontStrokeColor, textBoxFontStrokeColor);
        }
        private void textBoxFontStrokeColor_Apply(object sender, EventArgs e)
        {
            textBoxColorApply("FontStrokeColor", buttonFontStrokeColor, textBoxFontStrokeColor);
        }

        private void textBoxFontStrokeColor_TextChanged(object sender, EventArgs e)
        {
            textBoxColorTextChanged("FontStrokeColor", buttonFontStrokeColor, textBoxFontStrokeColor);
        }

        private void textBoxFontStrokeColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool? validated = textBoxColorKeyPress(e);

            if (validated == null)
            {
                textBoxFontStrokeColor_Apply(this, null);
            }
            else
            {
                e.Handled = (bool)validated;
            }
        }

        private void buttonResetFontStrokeColor_Click(object sender, EventArgs e)
        {
            plugin.Controller.FontStrokeColor = plugin.Controller.DefaultFontStrokeColor;
        }

        private void checkBoxSolidBar_CheckedChanged(object sender, EventArgs e)
        {
            plugin.Controller.SolidBar = checkBoxSolidBar.Checked;
        }
    }
}
