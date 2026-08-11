using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace SleepyTime_2._0
{
    public partial class frmMain : Form
    {
        //rounded borders values
        private int borderRadius = 30, BorderSize = 2;
        private Color boderColour = Color.Yellow;

        private bool countdownStarted = false;
        private TimeSpan remainingTime;
        private bool countdownEnded = false;

        //Drag and Drop functionality for form header.
        private bool Dragging = false;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public frmMain()
        {
            InitializeComponent();

            //further options for rounded form borders
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(BorderSize);


            tmrMain.Start();
            tmrValidation.Start();
        }

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = (DateTime.Now.ToString("HH:mm"));
            imgTimeAnimation.Visible = !imgTimeAnimation.Visible;

        }


        //drag and drop functionality for header of form.
        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
        }

        private void lblTitle_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void lblTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            }
        }

        //methods for form rounded borders
        private GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        //draw the rounded borders ready to be displayed.
        private void FormRegionAndBorder(Form form, float radius, Graphics graph, Color borderColour, float borderSize)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                using (GraphicsPath roundPath = GetRoundedPath(form.ClientRectangle, radius))
                using (Pen penBorder = new Pen(borderColour, borderSize)) 
                using (Matrix transform = new Matrix())
                {
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    form.Region = new Region(roundPath);
                    if (borderSize >= 1)
                    {
                        Rectangle rect = form.ClientRectangle;
                        float scaleX = 1.0F - ((borderSize + 1) / rect.Width);
                        float scaleY = 1.0F - ((borderSize + 1) / rect.Height);

                        transform.Scale(scaleX, scaleY);
                        transform.Translate(borderSize / 1.6F, borderSize / 1.6F);

                        graph.Transform = transform;
                        graph.DrawPath(penBorder, roundPath);

                    }
                }

            }
        }

        //draw rounded borders.
        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            FormRegionAndBorder(this, borderRadius, e.Graphics, Color.FromArgb(13, 15, 28), 2);
        }

        private void btnExit_MouseEnter(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.FromArgb(169, 5, 5);
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.FromArgb(25, 25, 41);
        }

        private void btnExit_MouseLeave(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.FromArgb(13, 15, 28);
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.FromArgb(13, 15, 28);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult exitBox = MessageBox.Show("Are you sure you want to exit?", "Close SleepyTime", MessageBoxButtons.YesNo);
            {
                if (exitBox == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void greyOutSidebar()
        {
            btnSidebarAbout.ForeColor = Color.FromArgb(177, 178, 181);
            btnSidebarCountdown.ForeColor = Color.FromArgb(177, 178, 181);
            btnSideBarSettings.ForeColor = Color.FromArgb(177, 178, 181);
            btnSidebarSchedule.ForeColor = Color.FromArgb(177, 178, 181);
            btnSideBarPresets.ForeColor = Color.FromArgb(177, 178, 181);
            btnHelp.ForeColor = Color.FromArgb(177, 178, 181);

            btnSidebarAbout.BackColor = Color.FromArgb(13, 15, 28);
            btnSidebarCountdown.BackColor = Color.FromArgb(13, 15, 28);
            btnSideBarSettings.BackColor = Color.FromArgb(13, 15, 28);
            btnSidebarSchedule.BackColor = Color.FromArgb(13, 15, 28);
            btnSideBarPresets.BackColor = Color.FromArgb(13, 15, 28);
            btnHelp.BackColor = Color.FromArgb(13, 15, 28);
        }

        private void SetActivePanel(string operation)
        {
            Panel[] panels =
            {
                pnlCountdown,
                pnlSchedule,
                pnlPresets,
                pnlSettings,
                pnlAbout,
                pnlHelp
            };

            foreach (Panel panel in panels)
            {
                if (!panel.Name.Equals(operation))
                {
                    panel.Visible = false;
                    panel.Enabled = false;
                }
                else
                {
                    panel.Visible = true;
                    panel.Enabled = true;
                }
            }    
            
        }

        private void btnSidebarCountdown_Click(object sender, EventArgs e)
        {
            greyOutSidebar();

            btnSidebarCountdown.ForeColor = Color.FromArgb(126, 39, 201);
            btnSidebarCountdown.BackColor = Color.FromArgb(25, 22, 46);

            SetActivePanel("pnlCountdown");
        }

        private void btnSidebarSchedule_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSidebarSchedule.ForeColor = Color.FromArgb(126, 39, 201);
            btnSidebarSchedule.BackColor = Color.FromArgb(25, 22, 46);

            SetActivePanel("pnlSchedule");
        }

        private void btnSideBarPresets_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSideBarPresets.ForeColor = Color.FromArgb(126, 39, 201);
            btnSideBarPresets.BackColor = Color.FromArgb(25, 22, 46);

            SetActivePanel("pnlPresets");
        }

        private void btnSideBarSettings_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSideBarSettings.ForeColor = Color.FromArgb(126, 39, 201);
            btnSideBarSettings.BackColor = Color.FromArgb(25, 22, 46);

            SetActivePanel("pnlSettings");
        }

        private void btnSidebarAbout_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSidebarAbout.ForeColor = Color.FromArgb(126, 39, 201);
            btnSidebarAbout.BackColor = Color.FromArgb(25, 22, 46);

            SetActivePanel("pnlAbout");
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnHelp.ForeColor = Color.FromArgb(126, 39, 201);
            btnHelp.BackColor = Color.FromArgb(25, 22, 46);

            SetActivePanel("pnlHelp");
        }


        private void NumbersOnly(object sender, KeyPressEventArgs e)
        {
            char numsOnly = e.KeyChar;

            if (!Char.IsDigit(numsOnly) && numsOnly != 8)
            {
                e.Handled = true;
            }
        }

        private void btnStartCountdown_Click(object sender, EventArgs e)
        {
            if (!countdownStarted)
            {
                //start the countdown
                if (int.TryParse(txtHours.Text, out int Hours) &&
                   int.TryParse(txtMinutes.Text, out int Minutes) &&
                   int.TryParse(txtSeconds.Text, out int Seconds))
                {
                    btnClearTimer.Enabled = false;
                    btnClearTimer.Visible = false;

                    countdownStarted = true;
                    btnStartCountdown.ForeColor = Color.Red;
                    btnStartCountdown.BorderColor = Color.Red;
                    btnStartCountdown.Text = "Cancel";

                    txtHours.ReadOnly = true;
                    txtMinutes.ReadOnly = true;
                    txtSeconds.ReadOnly = true;

                    txtHours.Cursor = Cursors.Arrow;
                    txtMinutes.Cursor = Cursors.Arrow;
                    txtSeconds.Cursor = Cursors.Arrow;

                    TimeSpan time = new TimeSpan(0, Hours, Minutes, Seconds);
                    remainingTime = time;
                    countdownEnded = false;
                    tmrCountDown.Start();
                }
                else
                {
                    MessageBox.Show("Please enter a valid time");
                }
            }
            else if (countdownEnded)
            {
                CancelCountdown();
            }
            else
            {
                tmrCountDown.Stop();
                DialogResult exitBox = MessageBox.Show("Cancel the Countdown?", "Cancel Shutdown", MessageBoxButtons.YesNo);
                {
                    if (exitBox == DialogResult.Yes)
                    {
                        CancelCountdown();
                    }
                    else
                    {
                        tmrCountDown.Start();
                    }
                }
            } 
        }

        private void CancelCountdown()
        {
            enableQuickTimers();

            btnClearTimer.Enabled = true;
            btnClearTimer.Visible = true;

            countdownStarted = false;
            btnStartCountdown.ForeColor = Color.FromArgb(141, 74, 205);
            btnStartCountdown.BorderColor = Color.FromArgb(141, 74, 205);
            btnStartCountdown.Text = "Start Countdown";

            txtHours.ReadOnly = false;
            txtMinutes.ReadOnly = false;
            txtSeconds.ReadOnly = false;

            txtHours.Cursor = Cursors.IBeam;
            txtMinutes.Cursor = Cursors.IBeam;
            txtSeconds.Cursor = Cursors.IBeam;

            tmrCountDown.Stop();
        }

        private void tmrCountDown_Tick(object sender, EventArgs e)
        {
            if (remainingTime.TotalSeconds > 0)
            {
                remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

                UpdateTimerDisplay();
            }
            else
            {
                tmrCountDown.Stop();
                countdownEnded = true;
                switch (cmbOperation.SelectedIndex.ToString())
                {
                    case "0": // SHUTDOWN
                        btnStartCountdown.PerformClick();
                        Process.Start("Shutdown", "/s");
                        break;

                    case "1": // RESTART
                        btnStartCountdown.PerformClick();
                        Process.Start("Shutdown", "/r");
                        break;

                    case "2": // SLEEP
                        btnStartCountdown.PerformClick();
                        Application.SetSuspendState(PowerState.Suspend, true, true);
                        break;

                    case "3": // LOCK
                        btnStartCountdown.PerformClick();
                        Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
                        break;
                }
            }
        }
        private void UpdateTimerDisplay()
        {
            txtHours.Text = ((int)remainingTime.TotalHours).ToString("00");
            txtMinutes.Text = remainingTime.Minutes.ToString("00");
            txtSeconds.Text = remainingTime.Seconds.ToString("00");
        }

        private void btnAdd5Min_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromMinutes(5));
            UpdateTimerDisplay();
        }

        private void btnAdd15Min_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromMinutes(15));
            UpdateTimerDisplay();
        }

        private void btnAdd30Min_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromMinutes(30));
            UpdateTimerDisplay();
        }

        private void btnAdd1Hr_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromHours(1));
            UpdateTimerDisplay();
        }

        private void btnClearTimer_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(remainingTime);
            UpdateTimerDisplay();
        }

        private void tmrValidation_Tick(object sender, EventArgs e)
        {
            if ((string.IsNullOrEmpty(txtHours.Text) || txtHours.Text == "00" || txtHours.Text == "0") && (string.IsNullOrEmpty(txtMinutes.Text) || txtMinutes.Text == "00" || txtMinutes.Text == "0") && (string.IsNullOrEmpty(txtSeconds.Text) || txtSeconds.Text == "00" || txtSeconds.Text == "0"))
            {
                btnStartCountdown.Enabled = false;
            }
            else
            {
                btnStartCountdown.Enabled = true;
            }
        }

        private void NotEmpty(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.Text = "00";
            }

            if (Convert.ToInt32(tb.Text) >= 60 && tb != txtHours)
            {
                tb.Text = "59";
            }
        }

        private void disableQuickTimers()
        {
            btnQuick15.Enabled = false;
            btnQuick30.Enabled = false;
            btnQuick1.Enabled = false;
            btnQuick2.Enabled = false;
        }

        private void enableQuickTimers()
        {
            btnQuick15.Enabled = true;
            btnQuick30.Enabled = true;
            btnQuick1.Enabled = true;
            btnQuick2.Enabled = true;
        }

        private void btnQuick15_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromMinutes(15));
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void btnQuick30_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromMinutes(30));
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void btnQuick1_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromHours(1));
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void btnQuick2_Click(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Add(TimeSpan.FromHours(2));
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cmbOperation.SelectedIndex = 0;

            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;

            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.FlatAppearance.BorderSize = 0;

            btnSidebarCountdown.FlatStyle = FlatStyle.Flat;
            btnSidebarCountdown.FlatAppearance.BorderSize = 0;

            btnSidebarSchedule.FlatStyle = FlatStyle.Flat;
            btnSidebarSchedule.FlatAppearance.BorderSize = 0;

            btnSideBarPresets.FlatStyle = FlatStyle.Flat;
            btnSideBarPresets.FlatAppearance.BorderSize = 0;

            btnSideBarSettings.FlatStyle = FlatStyle.Flat;
            btnSideBarSettings.FlatAppearance.BorderSize = 0;

            btnSidebarAbout.FlatStyle = FlatStyle.Flat;
            btnSidebarAbout.FlatAppearance.BorderSize = 0;

            btnHelp.FlatStyle = FlatStyle.Flat;
            btnHelp.FlatAppearance.BorderSize = 0;

            btnSidebarCountdown.PerformClick();
        }
    }
}
