namespace ACTTimeline
{
    partial class OverlayButtonsForm
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonRewind = new System.Windows.Forms.Button();
            this.buttonPlayPause = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonRewind
            // 
            this.buttonRewind.BackColor = System.Drawing.Color.White;
            this.buttonRewind.FlatAppearance.BorderSize = 0;
            this.buttonRewind.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRewind.Font = new System.Drawing.Font("Gulim", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRewind.ForeColor = System.Drawing.SystemColors.Desktop;
            this.buttonRewind.Location = new System.Drawing.Point(0, 0);
            this.buttonRewind.Margin = new System.Windows.Forms.Padding(0);
            this.buttonRewind.Name = "buttonRewind";
            this.buttonRewind.Size = new System.Drawing.Size(31, 18);
            this.buttonRewind.TabIndex = 4;
            this.buttonRewind.Text = "<<";
            this.buttonRewind.UseVisualStyleBackColor = false;
            this.buttonRewind.Click += new System.EventHandler(this.buttonRewind_Click);
            // 
            // buttonPlayPause
            // 
            this.buttonPlayPause.BackColor = System.Drawing.Color.White;
            this.buttonPlayPause.FlatAppearance.BorderSize = 0;
            this.buttonPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPlayPause.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonPlayPause.ForeColor = System.Drawing.SystemColors.Desktop;
            this.buttonPlayPause.Location = new System.Drawing.Point(31, 0);
            this.buttonPlayPause.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPlayPause.Name = "buttonPlayPause";
            this.buttonPlayPause.Size = new System.Drawing.Size(33, 18);
            this.buttonPlayPause.TabIndex = 3;
            this.buttonPlayPause.Text = "▷";
            this.buttonPlayPause.UseVisualStyleBackColor = false;
            this.buttonPlayPause.Click += new System.EventHandler(this.buttonPlayPause_Click);
            // 
            // OverlayButtonsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(64, 19);
            this.Controls.Add(this.buttonPlayPause);
            this.Controls.Add(this.buttonRewind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OverlayButtonsForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonRewind;
        private System.Windows.Forms.Button buttonPlayPause;

    }
}
