
namespace SleepyTime_2._0
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.imgHeaderDivider = new System.Windows.Forms.PictureBox();
            this.lblCurrentTime = new System.Windows.Forms.Label();
            this.tmrMain = new System.Windows.Forms.Timer(this.components);
            this.lblTitle = new System.Windows.Forms.Label();
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSidebarCountdown = new System.Windows.Forms.Button();
            this.btnSidebarSchedule = new System.Windows.Forms.Button();
            this.btnSideBarPresets = new System.Windows.Forms.Button();
            this.btnSideBarSettings = new System.Windows.Forms.Button();
            this.btnSidebarAbout = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.imgTimeAnimation = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgHeaderDivider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgTimeAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // imgHeaderDivider
            // 
            this.imgHeaderDivider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(41)))));
            this.imgHeaderDivider.Location = new System.Drawing.Point(-3, 68);
            this.imgHeaderDivider.Name = "imgHeaderDivider";
            this.imgHeaderDivider.Size = new System.Drawing.Size(803, 3);
            this.imgHeaderDivider.TabIndex = 1;
            this.imgHeaderDivider.TabStop = false;
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.AutoSize = true;
            this.lblCurrentTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(41)))));
            this.lblCurrentTime.Font = new System.Drawing.Font("JetBrains Mono", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCurrentTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.lblCurrentTime.Location = new System.Drawing.Point(23, 396);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(84, 31);
            this.lblCurrentTime.TabIndex = 3;
            this.lblCurrentTime.Text = "HH:MM";
            // 
            // tmrMain
            // 
            this.tmrMain.Interval = 1000;
            this.tmrMain.Tick += new System.EventHandler(this.tmrMain_Tick);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("JetBrains Mono", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(800, 64);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "SleepyTime";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDown);
            this.lblTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseMove);
            this.lblTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseUp);
            // 
            // imgLogo
            // 
            this.imgLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.imgLogo.Image = global::SleepyTime_2._0.Properties.Resources.SleepyTimeImg;
            this.imgLogo.Location = new System.Drawing.Point(237, 12);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(44, 41);
            this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLogo.TabIndex = 9;
            this.imgLogo.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(41)))));
            this.pictureBox1.Location = new System.Drawing.Point(127, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(3, 385);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(672, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(65, 37);
            this.btnMinimize.TabIndex = 12;
            this.btnMinimize.Text = "-";
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            this.btnMinimize.MouseEnter += new System.EventHandler(this.btnMinimize_MouseEnter);
            this.btnMinimize.MouseLeave += new System.EventHandler(this.btnMinimize_MouseLeave);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(734, 0);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(65, 37);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "×";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            this.btnExit.MouseEnter += new System.EventHandler(this.btnExit_MouseEnter);
            this.btnExit.MouseLeave += new System.EventHandler(this.btnExit_MouseLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("JetBrains Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.label1.Location = new System.Drawing.Point(704, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Version 2.0";
            // 
            // btnSidebarCountdown
            // 
            this.btnSidebarCountdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnSidebarCountdown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSidebarCountdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSidebarCountdown.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSidebarCountdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.btnSidebarCountdown.Location = new System.Drawing.Point(0, 77);
            this.btnSidebarCountdown.Name = "btnSidebarCountdown";
            this.btnSidebarCountdown.Size = new System.Drawing.Size(121, 37);
            this.btnSidebarCountdown.TabIndex = 14;
            this.btnSidebarCountdown.Text = "Countdown";
            this.btnSidebarCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSidebarCountdown.UseVisualStyleBackColor = false;
            this.btnSidebarCountdown.Click += new System.EventHandler(this.btnSidebarCountdown_Click);
            // 
            // btnSidebarSchedule
            // 
            this.btnSidebarSchedule.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnSidebarSchedule.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSidebarSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSidebarSchedule.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSidebarSchedule.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.btnSidebarSchedule.Location = new System.Drawing.Point(0, 120);
            this.btnSidebarSchedule.Name = "btnSidebarSchedule";
            this.btnSidebarSchedule.Size = new System.Drawing.Size(121, 37);
            this.btnSidebarSchedule.TabIndex = 15;
            this.btnSidebarSchedule.Text = "Schedule";
            this.btnSidebarSchedule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSidebarSchedule.UseVisualStyleBackColor = false;
            this.btnSidebarSchedule.Click += new System.EventHandler(this.btnSidebarSchedule_Click);
            // 
            // btnSideBarPresets
            // 
            this.btnSideBarPresets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnSideBarPresets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSideBarPresets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSideBarPresets.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSideBarPresets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.btnSideBarPresets.Location = new System.Drawing.Point(0, 163);
            this.btnSideBarPresets.Name = "btnSideBarPresets";
            this.btnSideBarPresets.Size = new System.Drawing.Size(121, 37);
            this.btnSideBarPresets.TabIndex = 16;
            this.btnSideBarPresets.Text = "Presets";
            this.btnSideBarPresets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSideBarPresets.UseVisualStyleBackColor = false;
            this.btnSideBarPresets.Click += new System.EventHandler(this.btnSideBarPresets_Click);
            // 
            // btnSideBarSettings
            // 
            this.btnSideBarSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnSideBarSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSideBarSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSideBarSettings.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSideBarSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.btnSideBarSettings.Location = new System.Drawing.Point(0, 206);
            this.btnSideBarSettings.Name = "btnSideBarSettings";
            this.btnSideBarSettings.Size = new System.Drawing.Size(121, 37);
            this.btnSideBarSettings.TabIndex = 17;
            this.btnSideBarSettings.Text = "Settings";
            this.btnSideBarSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSideBarSettings.UseVisualStyleBackColor = false;
            this.btnSideBarSettings.Click += new System.EventHandler(this.btnSideBarSettings_Click);
            // 
            // btnSidebarAbout
            // 
            this.btnSidebarAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.btnSidebarAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSidebarAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSidebarAbout.Font = new System.Drawing.Font("JetBrains Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSidebarAbout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.btnSidebarAbout.Location = new System.Drawing.Point(0, 249);
            this.btnSidebarAbout.Name = "btnSidebarAbout";
            this.btnSidebarAbout.Size = new System.Drawing.Size(121, 37);
            this.btnSidebarAbout.TabIndex = 18;
            this.btnSidebarAbout.Text = "About";
            this.btnSidebarAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSidebarAbout.UseVisualStyleBackColor = false;
            this.btnSidebarAbout.Click += new System.EventHandler(this.btnSidebarAbout_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(41)))));
            this.pictureBox2.Location = new System.Drawing.Point(0, 360);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(130, 93);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(41)))));
            this.label2.Font = new System.Drawing.Font("JetBrains Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(178)))), ((int)(((byte)(181)))));
            this.label2.Location = new System.Drawing.Point(19, 377);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Current Time";
            // 
            // imgTimeAnimation
            // 
            this.imgTimeAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(41)))));
            this.imgTimeAnimation.Location = new System.Drawing.Point(61, 397);
            this.imgTimeAnimation.Name = "imgTimeAnimation";
            this.imgTimeAnimation.Size = new System.Drawing.Size(10, 31);
            this.imgTimeAnimation.TabIndex = 21;
            this.imgTimeAnimation.TabStop = false;
            this.imgTimeAnimation.Visible = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(15)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.imgTimeAnimation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSidebarAbout);
            this.Controls.Add(this.btnSideBarSettings);
            this.Controls.Add(this.btnSideBarPresets);
            this.Controls.Add(this.btnSidebarSchedule);
            this.Controls.Add(this.btnSidebarCountdown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblCurrentTime);
            this.Controls.Add(this.imgLogo);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.imgHeaderDivider);
            this.Controls.Add(this.pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SleepyTime";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMain_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.imgHeaderDivider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgTimeAnimation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox imgHeaderDivider;
        private System.Windows.Forms.Label lblCurrentTime;
        private System.Windows.Forms.Timer tmrMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox imgLogo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSidebarCountdown;
        private System.Windows.Forms.Button btnSidebarSchedule;
        private System.Windows.Forms.Button btnSideBarPresets;
        private System.Windows.Forms.Button btnSideBarSettings;
        private System.Windows.Forms.Button btnSidebarAbout;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox imgTimeAnimation;
    }
}

