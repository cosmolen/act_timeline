using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace ACTTimeline
{
    public class TimelineView : Form
    {
        private DataGridView dataGridView;
        private OverlayButtonsForm buttons;
        private CachedSoundPlayer soundplayer;

        private int numberOfRowsToDisplay;
        public int NumberOfRowsToDisplay
        {
            get { return numberOfRowsToDisplay; }
            set
            {
                numberOfRowsToDisplay = value;
                UpdateLayout();
            }
        }

        private int barHeight;
        public int BarHeight
        {
            get { return barHeight; }
            set
            {
                barHeight = value;
                OnColumnWidthChanged();
            }
        }

        private int barWidth;
        public int BarWidth
        {
            get { return barWidth; }
            set
            {
                barWidth = value;
                OnColumnWidthChanged();
            }
        }

        private bool moveByDrag;
        public bool MoveByDrag
        {
            get { return moveByDrag; }
            set
            {
                moveByDrag = value;
                Win32APIUtils.SetWS_EX_TRANSPARENT(Handle, !moveByDrag);
            }
        }

        private bool showOverlayButtons;
        public bool ShowOverlayButtons
        {
            get { return showOverlayButtons; }
            set
            {
                showOverlayButtons = value;
                OnVisibleChanged(EventArgs.Empty);
            }
        }

        private Font timelineFont;
        public Font TimelineFont
        {
            get { return timelineFont; }
            set
            {
                timelineFont = value;
                OnTimelineFontChanged();
            }
        }

        private bool over10;
        public bool Over10
        {
            get { return over10; }
            set
            {
                over10 = value;
                if (controller != null)
                    controller.OnCurrentTimeUpdate();
            }
        }

        private bool under10;
        public bool Under10
        {
            get { return under10; }
            set
            {
                under10 = value;
                if(controller != null)
                    controller.OnCurrentTimeUpdate();
            }
        }

        private bool showCasting;
        public bool ShowCasting
        {
            get { return showCasting; }
            set
            {
                showCasting = value;
                if (controller != null)
                    controller.OnCurrentTimeUpdate();
            }
        }

        private bool popupMode;
        public bool PopupMode
        {
            get { return popupMode; }
            set
            {
                popupMode = value;
                timeLeftColumn.popupMode = value;
                UpdateLayout();

                if (controller != null)
                    controller.OnCurrentTimeUpdate();
            }
        }

        private bool overlayVisible;
        public bool OverlayVisible
        {
            get { return overlayVisible; }
            set
            {
                overlayVisible = value;
                OnVisibleChanged(EventArgs.Empty);
            }
        }


        public event EventHandler TimelineFontChanged;
        public void OnTimelineFontChanged()
        {
            timeLeftColumn.DefaultCellStyle.Font = TimelineFont;
            timeLeftColumn.TimelineFont = TimelineFont;
            UpdateLayout();

            if (TimelineFontChanged != null)
                TimelineFontChanged(this, EventArgs.Empty);
        }

        public event EventHandler ColumnWidthChanged;
        public void OnColumnWidthChanged()
        {
            UpdateLayout();

            if (ColumnWidthChanged != null)
                ColumnWidthChanged(this, EventArgs.Empty);
        }

        // trigger OnOpacityChanged on change.
        public double MyOpacity
        {
            get { return Opacity; }
            set
            {
                Opacity = value;
                OnOpacityChanged();
            }
        }

        public event EventHandler OpacityChanged;
        public void OnOpacityChanged()
        {
            if (OpacityChanged != null)
                OpacityChanged(this, EventArgs.Empty);
        }


        private bool playSoundByACT;
        public bool PlaySoundByACT
        {
            get { return playSoundByACT; }
            set
            {
                playSoundByACT = value;
                OnPlaySoundByACTChanged();
            }
        }

        public event EventHandler PlaySoundByACTChanged;
        public void OnPlaySoundByACTChanged()
        {
            WarmUpSoundPlayerCache();

            if (PlaySoundByACTChanged != null)
                PlaySoundByACTChanged(this, EventArgs.Empty);
        }

        private TimelineController controller;
        TimeLeftColumn timeLeftColumn;
        
        public TimelineView(TimelineController controller_)
        {
            controller = controller_;
            controller.TimelineUpdate += controller_TimelineUpdate;
            controller.CurrentTimeUpdate += controller_CurrentTimeUpdate;

            barHeight = 25;
            barWidth = 200;

            SetupUI();

            this.MouseDown += TimelineView_MouseDown;
            this.VisibleChanged += TimelineView_VisibleChanged;
            this.Move += TimelineView_Move;
            this.FormClosing += TimelineView_FormClosing;

            typeof(DataGridView).
                GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).
                SetValue(dataGridView, true, null);
            dataGridView.SelectionChanged += (object sender, EventArgs args) => dataGridView.ClearSelection();
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.Add(timeLeftColumn = new TimeLeftColumn { Controller = controller_ });

            MyOpacity = 0.8;
            NumberOfRowsToDisplay = 3;
            MoveByDrag = true;
            ShowOverlayButtons = true;
            Over10 = true;
            Under10 = true;
            ShowCasting = true;
            PopupMode = false;
            OverlayVisible = true;
            UpdateLayout();

            soundplayer = new CachedSoundPlayer();

            TimelineFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        }

        void TimelineView_VisibleChanged(object sender, EventArgs e)
        {
            buttons.Visible = Visible && showOverlayButtons && overlayVisible;
        }

        private void SetupUI()
        {
            dataGridView = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            this.SuspendLayout();

            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeColumns = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.ColumnHeadersVisible = false;
            dataGridView.Enabled = false;
            dataGridView.Location = new Point(0, 0);
            dataGridView.Margin = new Padding(0);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.ScrollBars = ScrollBars.None;
            dataGridView.Size = new Size(200, 80);
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.BackgroundColor = Color.FromArgb(80, 80, 80);
            dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(80, 80, 80);

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(200, 80);
            this.Controls.Add(dataGridView);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "TimelineView";
            this.Text = "Timeline";
            this.TopMost = true;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            this.ResumeLayout(false);

            TransparencyKey = Color.FromArgb(80, 80, 80);

            buttons = new OverlayButtonsForm(controller);
        }

        void UpdateLayout()
        {
            dataGridView.RowTemplate.Height = barHeight;
            int gridHeight = dataGridView.RowTemplate.Height * numberOfRowsToDisplay;
            ClientSize = new Size(barWidth, gridHeight);
            dataGridView.Size = ClientSize;
            
            timeLeftColumn.Width = barWidth;

            // update buttons location
            TimelineView_Move(this, EventArgs.Empty);

            controller.OnCurrentTimeUpdate();
        }

        void TimelineView_Move(object sender, EventArgs e)
        {
            buttons.Location = new Point(this.Location.X + barWidth - buttons.Width, this.Location.Y - buttons.Height);
        }

        void TimelineView_FormClosing(object sender, FormClosingEventArgs e)
        {
            buttons.Close();
        }

        void TimelineView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && MoveByDrag)
            {
                Win32APIUtils.DragMove(Handle);
            }
        }

        void controller_TimelineUpdate(object sender, EventArgs e)
        {
            if (controller.Timeline == null)
                return;

            WarmUpSoundPlayerCache();
        }

        private void WarmUpSoundPlayerCache()
        {
            if (controller.Timeline == null)
                return;

            if (playSoundByACT)
                return;

            foreach (AlertSound sound in controller.Timeline.AlertSoundAssets.All)
            {
                soundplayer.WarmUpCache(sound.Filename);
            }
        }

        void controller_CurrentTimeUpdate(object sender, EventArgs e)
        {
            Timeline timeline = controller.Timeline;
            if (timeline == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action(() => { controller_CurrentTimeUpdate(sender, e); }));
                return;
            }
            else
            {
                // play pending alerts
                var pendingAlerts = timeline.PendingAlertsAt(controller.CurrentTime);
                foreach (ActivityAlert pendingAlert in pendingAlerts)
                {
                    if (pendingAlert.Sound != null)
                    {
                        soundplayer.PlaySound(pendingAlert.Sound.Filename);
                    }
                    if (pendingAlert.TtsSpeaker != null &&
                        !string.IsNullOrWhiteSpace(pendingAlert.TtsSentence))
                    {
                        pendingAlert.TtsSpeaker.Synthesizer.SpeakAsync(pendingAlert.TtsSentence);
                    }
                    pendingAlert.Processed = true;
                }

                // sync dataGridView
                dataGridView.DataSource = null;
                if (popupMode)
                {
                    dataGridView.DataSource = timeline.VisibleItemsAtMostWithoutCasting(controller.CurrentTime - 5, controller.CurrentTime, numberOfRowsToDisplay).ToList();
                }
                else if (over10)
                {
                    if (under10)
                        if (showCasting)  dataGridView.DataSource = timeline.VisibleItemsAt(controller.CurrentTime, numberOfRowsToDisplay).ToList();
                        else              dataGridView.DataSource = timeline.VisibleItemsAtWithoutCasting(controller.CurrentTime, numberOfRowsToDisplay).ToList();
                    else                  dataGridView.DataSource = timeline.VisibleItemsAtLeast(controller.CurrentTime, controller.CurrentTime + 10, numberOfRowsToDisplay).ToList();
                }
                else
                {
                    if (under10)
                        if (showCasting)  dataGridView.DataSource = timeline.VisibleItemsAtMost(controller.CurrentTime, controller.CurrentTime + 10, numberOfRowsToDisplay).ToList();
                        else              dataGridView.DataSource = timeline.VisibleItemsAtMostWithoutCasting(controller.CurrentTime, controller.CurrentTime + 10, numberOfRowsToDisplay).ToList();
                    else if (showCasting) dataGridView.DataSource = timeline.VisibleItemsAtMost(controller.CurrentTime, controller.CurrentTime, numberOfRowsToDisplay).ToList();
                }
                if(!overlayVisible)
                    dataGridView.DataSource = null;
            }
        }

        void ProcessAlert(ActivityAlert alert)
        {
            //TTSクラスならACT本体に読み上げさせる
            if (alert.Sound is AlertTTS)
            {
                ActGlobals.oFormActMain.TTS(alert.Sound.Filename);
            }
            else if (PlaySoundByACT)
            {
                ActGlobals.oFormActMain.PlaySoundMethod(alert.Sound.Filename, 100);
            } 
            else 
            {
                soundplayer.PlaySound(alert.Sound.Filename);
            }

            alert.Processed = true;
        }
    }

    class TimeLeftColumn : DataGridViewColumn
    {
        public TimelineController Controller;
        public Font TimelineFont;
        public bool popupMode;

        public TimeLeftColumn()
        {
            this.ReadOnly = true;
            this.CellTemplate = new TimeLeftCell();
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            this.DataPropertyName = "Self";
        }
    }

    class TimeLeftCell : DataGridViewTextBoxCell
    {
        public const float BLUE_BAR_START = 30.0F;
        public const float RED_BAR_START = 10.0F;
        public const float THRESHOLD = 0.0F;
        
        public const int MARGIN = 0; // px

        private Font valueFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        static private readonly StringFormat ValueStringFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        static private readonly Pen ValuePen = new Pen(Brushes.Black, 2.0F) { LineJoin = LineJoin.Round };
        static private readonly Brush ValueFill = Brushes.White;

        static private readonly StringFormat NameStringFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        static private readonly StringFormat PopupStringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public static Color BarColorAtTimeLeft(float timeLeft)
        {
            if (timeLeft > 10)
                return Color.FromArgb(64, 80, 176);
            else
                return Color.Red;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, "", "", errorText, cellStyle, advancedBorderStyle, paintParts);
            
            TimelineActivity activity = (TimelineActivity)value;
            TimelineController controller = ((TimeLeftColumn)OwningColumn).Controller;
            valueFont = ((TimeLeftColumn)OwningColumn).TimelineFont;

            if(((TimeLeftColumn)OwningColumn).popupMode)
            {
                double timeTillStart = activity.TimeFromStart - controller.CurrentTime;
                float timeTillStartF = (float)timeTillStart;

                string text = activity.Name;
                PopupName(graphics, cellBounds, text, timeTillStartF);
                return;
            }

            {
                double timeTillStart = activity.TimeFromStart - controller.CurrentTime;
                float timeTillStartF = (float)timeTillStart;
                float durationF = (float)activity.Duration;

                if (durationF < 0.1F)
                    durationF = 0.1F;

                PaintBar(graphics, cellBounds, timeTillStartF, durationF);
            }

            {
                string text = activity.Name;
                PaintName(graphics, cellBounds, text);
            }

            {
                double timeTillEnd = activity.EndTime - controller.CurrentTime;
                string text = ((int)timeTillEnd > 9) ? (Math.Floor(timeTillEnd).ToString("0")) : ((timeTillEnd > 0) ? (timeTillEnd.ToString("0.0")) : ("ACTION!"));
                PaintText(graphics, cellBounds, text);
            }
        }

        private RectangleF DrawAreaFromCellBounds(Rectangle cellBounds)
        {
            return new RectangleF(cellBounds.X + MARGIN, cellBounds.Y + MARGIN, cellBounds.Width - MARGIN * 2, cellBounds.Height - MARGIN * 2);
        }

        private void PaintText(Graphics graphics, Rectangle cellBounds, string valueString)
        {
            RectangleF drawArea = DrawAreaFromCellBounds(cellBounds);

            RectangleF textRect = drawArea;

            GraphicsPath pathText = new GraphicsPath();
            pathText.AddString(valueString, valueFont.FontFamily, (int)valueFont.Style, valueFont.Size, textRect, ValueStringFormat);

            GraphicsState state = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawPath(ValuePen, pathText);
            graphics.FillPath(ValueFill, pathText);
            graphics.Restore(state);
        }

        private void PaintName(Graphics graphics, Rectangle cellBounds, string valueString)
        {
            RectangleF drawArea = DrawAreaFromCellBounds(cellBounds);

            RectangleF textRect = drawArea;

            GraphicsPath pathText = new GraphicsPath();
            pathText.AddString(valueString, valueFont.FontFamily, (int)valueFont.Style, valueFont.Size, textRect, NameStringFormat);

            GraphicsState state = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawPath(ValuePen, pathText);
            graphics.FillPath(ValueFill, pathText);
            graphics.Restore(state);
        }

        private void PopupName(Graphics graphics, Rectangle cellBounds, string valueString, float timeTillStart)
        {
            float size = 1.0F;

            RectangleF drawArea = DrawAreaFromCellBounds(cellBounds);

            RectangleF textRect = drawArea;

            if(timeTillStart > -0.15F)
                size = 1.0F - 0.2F * timeTillStart / 0.15F;
            else if(timeTillStart > -0.3F)
                size = 1.4F + 0.2F * timeTillStart / 0.3F;

            GraphicsPath pathText = new GraphicsPath();
            pathText.AddString(valueString, valueFont.FontFamily, (int)valueFont.Style, valueFont.Size * size, textRect, PopupStringFormat);

            GraphicsState state = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawPath(ValuePen, pathText);
            graphics.FillPath(ValueFill, pathText);
            graphics.Restore(state);
        }

        private void PaintBar(Graphics graphics, Rectangle cellBounds, float timeTillStart, float duration)
        {
            RectangleF drawArea = DrawAreaFromCellBounds(cellBounds);
            RectangleF barFill = drawArea;

            Color colorA;
            Color colorB;
            Rectangle gradientRect;
            if (timeTillStart > BLUE_BAR_START)
            {
                float bar = 1.0F;
                
                barFill.Width *= bar;

                colorA = BarColorAtTimeLeft(timeTillStart);
                colorB = ControlPaint.Dark(colorA, 0.2F);
                gradientRect = Rectangle.Ceiling(barFill);
            }
            else if (timeTillStart > 10)
            {
                float bar = timeTillStart / BLUE_BAR_START;
                if (bar > 1.0F)
                    bar = 1.0F;
                
                barFill.Width *= bar;

                colorA = BarColorAtTimeLeft(timeTillStart);
                colorB = ControlPaint.Dark(colorA, 0.2F);
                gradientRect = Rectangle.Ceiling(barFill);
            }
            else if (timeTillStart > 0)
            {
                float bar = timeTillStart / RED_BAR_START;
                if (bar > 1.0F)
                    bar = 1.0F;

                barFill.Width *= bar;

                colorA = BarColorAtTimeLeft(timeTillStart);
                colorB = ControlPaint.Dark(colorA, 0.2F);
                gradientRect = Rectangle.Ceiling(barFill);
            }
            else
            {
                float bar = -timeTillStart / duration;
                if (bar > 1.0F)
                    bar = 1.0F;
                
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(247, 154, 0)), barFill);

                barFill.Width *= bar;

                colorA = Color.White;
                colorB = Color.White;
                gradientRect = Rectangle.Ceiling(barFill);
            }

            if (barFill.Width < 1.0F)
                return;

            Brush barBrush = new LinearGradientBrush(gradientRect, colorA, colorB, LinearGradientMode.Horizontal) { WrapMode = WrapMode.TileFlipX };
            graphics.FillRectangle(barBrush, barFill);
        }
    }
}
