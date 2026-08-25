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
using SleepyTime_2._0.Custom_Controls;
using System.IO;

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

        //list for populating scheduled items.
        private List<ScheduleItem> scheduledItems = new List<ScheduleItem>();

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        //APP SETTINGS
        private string settingsFile;

            //COLOUR THEME
        private string accentColour = "purple";
        private Color primaryAccent;
        private Color secondaryAccent;
        private string mainTheme = "dark";

        Color primaryTheme;
        Color secondaryTheme;

        //ALWAYS ON TOP
        private bool formAOT = false;

        string editTarget;


        public frmMain()
        {
            InitializeComponent();

            readSettingsFile();
            //LOAD IN THE ACCENT COLOUR FROM A FILE OR SOMETHING!!!
            getAccentColour();
            applyAccentColour(primaryAccent, secondaryAccent);

            readScheduleFile();
            populateTimesComboBox();
            updateScheduleUI();

            applyDarkMode(mainTheme);


            //further options for rounded form borders
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(BorderSize);

            tmrMain.Start();
            tmrValidation.Start();
        }

        private void populateTimesComboBox()
        {
            cmbScheduleTime.Items.Clear();

            for (int minutes = 0; minutes < 24 * 60; minutes += 10)
            {
                TimeSpan time = TimeSpan.FromMinutes(minutes);
                cmbScheduleTime.Items.Add(time.ToString(@"hh\:mm"));
            }
        }

        private void readScheduleFile()
        {
            if (!File.Exists("Schedule.txt"))
            {
                File.Create("Schedule.txt");
            }

            string[] lines = File.ReadAllLines("Schedule.txt");

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] data = line.Split('|');

                //MessageBox.Show($"Action: {data[0]} Date: {data[1]} Time: {data[2]} Reminder: {data[3]}");

                if (data.Length != 4)
                    continue;

                if (!DateTime.TryParseExact(
                    data[1],
                    "dd/MM/yyyy",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime date))
                {
                    MessageBox.Show("Date incorrect");
                    continue;
                }



                if (!TimeSpan.TryParse(data[2], out TimeSpan time))
                {
                    MessageBox.Show("Time incorrect");
                    continue;
                }

                //MessageBox.Show("adding scheduled item");
                scheduledItems.Add(
                    new ScheduleItem(
                        data[0],
                        date,
                        time,
                        data[3]
                        )
                    );
                //MessageBox.Show("Added scheduled item");
            }
            //MessageBox.Show(scheduledItems[0].ToString());
            //MessageBox.Show(scheduledItems[1].ToString());
        }

        private void readSettingsFile()
        {
            if (!File.Exists("Settings.txt"))
            {
                File.WriteAllLines("Settings.txt", new[]
                {
                    "purple",
                    "false",
                    "dark"
                });
                //settingsFile = Path.GetFullPath("Settings.txt");
            }

            string[] settings = File.ReadAllLines("Settings.txt");

            accentColour = settings[0];
            switch (accentColour)
            {
                case "purple":
                    cmbAccent.SelectedIndex = 0;
                    break;

                case "blue":
                    cmbAccent.SelectedIndex = 1;
                    break;

                case "green":
                    cmbAccent.SelectedIndex = 2;
                    break;

                case "yellow":
                    cmbAccent.SelectedIndex = 3;
                    break;

                case "red":
                    cmbAccent.SelectedIndex = 4;
                    break;
            }

            tglAOT.Checked = bool.Parse(settings[1]);
            this.TopMost = bool.Parse(settings[1]);

            tglDarkMode.Checked = bool.Parse(settings[2]);
            switch (settings[2])
            {
                case "false":
                    mainTheme = "light";
                    break;

                case "true":
                    mainTheme = "dark";
                    break;
            }

            applyDarkMode(mainTheme);
        }

        private void applyDarkMode(string mode)
        {
            switch (mode)
            {
                case "dark":
                    primaryTheme = Color.FromArgb(13, 15, 28);
                    secondaryTheme = Color.FromArgb(25, 25, 41);
                    break;

                case "light":
                    primaryTheme = Color.Gainsboro;
                    secondaryTheme = Color.DarkGray;
                    break;
            }

            //apply the main and secondary theme to everything on the form. EVERYTHING.
            //MessageBox.Show($"applying {mode} theme");
            foreach (Control c in GetAllControls(this))
            {
                if (c.BackColor == Color.FromArgb(13, 15, 28) || c.BackColor == Color.Gainsboro)
                {
                    c.BackColor = primaryTheme;
                }
                else if (c.BackColor == Color.FromArgb(25, 25, 41) || c.BackColor == Color.DarkGray)
                {
                    c.BackColor = secondaryTheme;
                }
            }

        }

        private void applyAccentColour(Color accentColour, Color secondaryAccent)
        {
            foreach (Control c in GetAllControls(this))
            {
                if (c.ForeColor == Color.FromArgb(140, 71, 203) || c.ForeColor == Color.FromArgb(35, 35, 204) || c.ForeColor == Color.LimeGreen || c.ForeColor == Color.FromArgb(197, 217, 20) || c.ForeColor == Color.FromArgb(222, 13, 13))
                {
                    c.ForeColor = primaryAccent;                  
                }

                if(c is RoundedButton button && c.Tag != "noColourChange")
                {
                    button.BorderColor = primaryAccent;
                }
                
                if (c is Label && Text == ":")
                {
                    c.ForeColor = primaryAccent;
                }

                if (c is ToggleButton toggle)
                {
                    toggle.OnBackColor = primaryAccent;
                }

                if (c is LinkLabel label)
                {
                    label.LinkColor = primaryAccent;
                }               
            }

            lblTimeTitle.ForeColor = primaryAccent;
            btnClearSchedule.BorderColor = Color.FromArgb(247, 62, 62);
            btnStartCountdown.ForeColor = primaryAccent;
            btnSideBarSettings.PerformClick();
        }

        private IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;

                if (c.HasChildren)
                {
                    foreach (Control child in GetAllControls(c))
                    {
                        yield return child;
                    }
                }
            }
        }

        private void getAccentColour()
        {
            //return accent colours here, lighter darker etc.
            switch(accentColour)
            {
                case "purple":
                    primaryAccent = Color.FromArgb(140, 71, 203);
                    secondaryAccent = Color.FromArgb(55, 28, 79);
                    break;

                case "blue":
                    primaryAccent = Color.FromArgb(35, 35, 204);
                    secondaryAccent = Color.FromArgb(19, 19, 99);
                    break;

                case "green":
                    primaryAccent = Color.LimeGreen;
                    secondaryAccent = Color.FromArgb(29, 107, 29);
                    break;

                case "yellow":
                    primaryAccent = Color.FromArgb(197, 217, 20);
                    secondaryAccent = Color.FromArgb(115, 125, 31);
                    break;

                case "red":
                    primaryAccent = Color.FromArgb(222, 13, 13);
                    secondaryAccent = Color.FromArgb(82, 17, 12);
                    break;
            }
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

            btnSidebarCountdown.ForeColor = primaryAccent;
            btnSidebarCountdown.BackColor = secondaryAccent;

            SetActivePanel("pnlCountdown");
        }

        private void btnSidebarSchedule_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSidebarSchedule.ForeColor = primaryAccent;
            btnSidebarSchedule.BackColor = secondaryAccent;

            SetActivePanel("pnlSchedule");
        }

        private void btnSideBarPresets_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSideBarPresets.ForeColor = primaryAccent;
            btnSideBarPresets.BackColor = secondaryAccent;

            SetActivePanel("pnlPresets");
        }

        private void btnSideBarSettings_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSideBarSettings.ForeColor = primaryAccent;
            btnSideBarSettings.BackColor = secondaryAccent;

            SetActivePanel("pnlSettings");
        }

        private void btnSidebarAbout_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnSidebarAbout.ForeColor = primaryAccent;
            btnSidebarAbout.BackColor = secondaryAccent;

            SetActivePanel("pnlAbout");
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            greyOutSidebar();
            btnHelp.ForeColor = primaryAccent;
            btnHelp.BackColor = secondaryAccent;

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
                    btnStartCountdown.ForeColor = Color.FromArgb(247, 62, 62);
                    btnStartCountdown.BorderColor = Color.FromArgb(247, 62, 62);
                    btnStartCountdown.Text = "Cancel";

                    txtHours.ReadOnly = true;
                    txtMinutes.ReadOnly = true;
                    txtSeconds.ReadOnly = true;

                    btnQuick1.Enabled = false;
                    btnQuick2.Enabled = false;
                    btnQuick15.Enabled = false;
                    btnQuick30.Enabled = false;

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

            btnStartCountdown.ForeColor = primaryAccent;
            btnStartCountdown.BorderColor = secondaryAccent;

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
                btnStartCountdown.Enabled = true;
                btnStartCountdown.PerformClick();
                switch (cmbOperation.SelectedIndex.ToString())
                {
                    case "0": // SHUTDOWN
                        Process.Start("Shutdown", "/s");
                        break;

                    case "1": // RESTART
                        Process.Start("Shutdown", "/r");
                        break;

                    case "2": // SLEEP
                        Application.SetSuspendState(PowerState.Suspend, true, true);
                        break;

                    case "3": // LOCK
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
            if (Convert.ToInt32(remainingTime.TotalHours) <= 99)
            {
                remainingTime = remainingTime.Add(TimeSpan.FromMinutes(5));
                UpdateTimerDisplay();
            }
        }

        private void btnAdd15Min_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(remainingTime.TotalHours) <= 99)
            {
                remainingTime = remainingTime.Add(TimeSpan.FromMinutes(15));
                UpdateTimerDisplay();
            }
        }

        private void btnAdd30Min_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(remainingTime.TotalHours) <= 99)
            {
                remainingTime = remainingTime.Add(TimeSpan.FromMinutes(30));
                UpdateTimerDisplay();
            }
        }

        private void btnAdd1Hr_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(remainingTime.TotalHours) <= 99)
            {
                remainingTime = remainingTime.Add(TimeSpan.FromHours(1));
                UpdateTimerDisplay();
            }
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

            SetRemainingTimeFromTextBoxes();

        }

        private void SetRemainingTimeFromTextBoxes()
        {
            if (int.TryParse(txtHours.Text, out int hours) &&
                int.TryParse(txtMinutes.Text, out int mins) &&
                int.TryParse(txtSeconds.Text, out int secs))
            {
                remainingTime = new TimeSpan(0, hours, mins, secs);
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
            remainingTime = new TimeSpan(0, 15, 0);
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void btnQuick30_Click(object sender, EventArgs e)
        {
            remainingTime = new TimeSpan(0, 30, 0);
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void btnQuick1_Click(object sender, EventArgs e)
        {
            remainingTime = new TimeSpan(1, 0, 0);
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void btnQuick2_Click(object sender, EventArgs e)
        {
            remainingTime = new TimeSpan(2, 0, 0);
            UpdateTimerDisplay();
            disableQuickTimers();
            btnStartCountdown.Enabled = true;
            btnStartCountdown.PerformClick();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://github.com/owendthayes/SleepyTime1.0");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://github.com/owendthayes/SleepyTime");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://github.com/owendthayes");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://www.linkedin.com/in/owendthayes/");
        }

        private void OpenLink(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            switch (cmbAccent.SelectedIndex)
            {
                case 0: //purple
                    accentColour = "purple";
                    break;

                case 1: //blue
                    accentColour = "blue";
                    break;

                case 2: //green
                    accentColour = "green";
                    break;

                case 3: //yellow
                    accentColour = "yellow";
                    break;

                case 4: //red
                    accentColour = "red";
                    break;
            }
            getAccentColour();
            applyAccentColour(primaryAccent, secondaryAccent);

            this.TopMost = tglAOT.Checked;

            switch (tglDarkMode.Checked)
            {
                case true:
                    mainTheme = "dark";
                    break;

                case false:
                    mainTheme = "light";
                    break;
            }

            applyDarkMode(mainTheme);

            //save settings
            File.WriteAllLines("Settings.txt", new[]
            {
                accentColour,
                tglAOT.Checked.ToString(),
                tglDarkMode.Checked.ToString()
            });
        }

        private void btnClearSchedule_Click(object sender, EventArgs e)
        {
            if (btnClearSchedule.Text == "Cancel")
            {
                //cancel the saving operations
                btnClearSchedule.Text = "Reset";
                pnlSavedSchedules.Enabled = true;
            }

            cmbScheduleOperation.SelectedIndex = 0;
            cmbScheduleTime.SelectedIndex = 0;
            cmbRemindMe.SelectedIndex = 0;
            cmbScheduleDate.Value = DateTime.Today;
        }

        private void updateScheduleUI()
        {
            string[] operations = { "Shutdown", "Restart", "Sleep", "Lock" };
            string[] reminders = { "No Reminder", "5 Mins", "10 Mins", "15 Mins", "30 Mins", "1 Hour", "2 Hours" };

            pnlSavedSchedules.Controls.Clear();

            int y = 10;

            foreach (ScheduleItem item in scheduledItems)
            {
                Panel row = new Panel();
                
                row.BackColor = Color.FromArgb(25, 25, 41);

                row.Width = pnlSavedSchedules.Width - 40;
                row.Height = 40;
                row.Location = new Point(10, y);

                //add the controls here

                Label lblAction = new Label
                {
                    Text = operations[Convert.ToInt32(item.Action)],
                    Location = new Point(10, 10),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("JetBrains Mono", 12)
                };

                Label lblDate = new Label
                {
                    Text = item.Date.ToString(@"dd/MM/yyyy"),
                    Location = new Point(110, 10),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("JetBrains Mono", 12),
                };

                Label lblTime = new Label
                {
                    Text = item.Time.ToString(@"hh\:mm"),
                    Location = new Point(225, 10),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("JetBrains Mono", 12),
                };

                Label lblReminder = new Label
                {
                    Text = reminders[Convert.ToInt32(item.Reminder)],
                    Location = new Point(310, 10),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("JetBrains Mono", 12),
                };

                RoundedButton btnEditSchedule = new RoundedButton
                {
                    Text = "✎",
                    Location = new Point(450, 5),
                    AutoSize = true,
                    ForeColor = primaryAccent,
                    BorderColor = primaryAccent,
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25
                };

                RoundedButton btnDeleteSchedule = new RoundedButton
                {
                    Text = "🗑",
                    Location = new Point(500, 5),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(247, 62, 62),
                    BorderColor = Color.FromArgb(247, 62, 62),
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25,
                    Tag = "noColourChange"
                };

                btnDeleteSchedule.Click += btnDeleteSchedule_Click;
                btnEditSchedule.Click += btnEditSchedule_Click;

                row.Controls.Add(lblAction);
                row.Controls.Add(lblDate);
                row.Controls.Add(lblTime);
                row.Controls.Add(lblReminder);
                row.Controls.Add(btnEditSchedule);
                row.Controls.Add(btnDeleteSchedule);

                pnlSavedSchedules.Controls.Add(row);
                y += row.Height + 5;
            }
        }



        private void btnEditSchedule_Click(object sender, EventArgs e)
        {
            string[] operations = { "Shutdown", "Restart", "Sleep", "Lock" };
            string[] reminders = { "No Reminder", "5 Mins", "10 Mins", "15 Mins", "30 Mins", "1 Hour", "2 Hours" };

            string[] data = new string[4];

            RoundedButton clickedButton = (RoundedButton)sender;

            Panel parentPanel = (Panel)clickedButton.Parent;

            foreach (Control c in parentPanel.Controls)
            {
                if (c is Label)
                {
                    if (operations.Contains(c.Text))
                    {
                        data[0] = Array.IndexOf(operations, c.Text).ToString();
                    };

                    //get the DATE
                    if (DateTime.TryParseExact(
                    c.Text,
                    "dd/MM/yyyy",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime date))
                    {
                        data[1] = date.ToString();
                    };

                    //get the TIME
                    if (TimeSpan.TryParse(c.Text, out TimeSpan timeDel))
                    {
                        data[2] = timeDel.ToString();
                    };

                    if (reminders.Contains(c.Text))
                    {
                        data[3] = Array.IndexOf(reminders, c.Text).ToString();
                    };

                }
            }

            //located the correct target saved item.
            TimeSpan editTime = TimeSpan.Parse(data[2]);
            string formattedTime = editTime.ToString(@"hh\:mm");
            
            editTarget = $"{data[0]}|{data[1]}|{formattedTime}|{data[3]}";

            //load the option boxes with the corresponding data.
            cmbScheduleOperation.SelectedIndex = Convert.ToInt32(data[0]);
            cmbScheduleDate.Value = DateTime.ParseExact(data[1], "dd/MM/yyyy HH:mm:ss", null);
            cmbScheduleTime.SelectedIndex = cmbScheduleTime.Items.IndexOf(formattedTime);
            cmbRemindMe.SelectedIndex = Convert.ToInt32(data[3]);

            //use edit target to store the old schedule, create a new saved item and overwrite the old one with the new one in the list before writing to file again.

            pnlSavedSchedules.Enabled = false;

            btnClearSchedule.Text = "Cancel";
            btnSaveSchedule.Text = "Update Schedule";
        }

        private void btnDeleteSchedule_Click(object sender, EventArgs e)
        {
            string[] operations = { "Shutdown", "Restart", "Sleep", "Lock" };
            string[] reminders = { "No Reminder", "5 Mins", "10 Mins", "15 Mins", "30 Mins", "1 Hour", "2 Hours" };

            string[] data = new string[4];

            DialogResult exitBox = MessageBox.Show("Delete this Action?", "Delete", MessageBoxButtons.YesNo);
            {
                if (exitBox == DialogResult.Yes)
                {
                    RoundedButton clickedButton = (RoundedButton)sender;

                    Panel parentPanel = (Panel)clickedButton.Parent;

                    foreach (Control c in parentPanel.Controls)
                    {
                        if (c is Label)
                        {
                            if (operations.Contains(c.Text))
                            {
                                data[0] = Array.IndexOf(operations, c.Text).ToString();
                            };

                            //get the DATE
                            if (DateTime.TryParseExact(
                            c.Text,
                            "dd/MM/yyyy",
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime date))
                            {
                                data[1] = date.ToString();
                            };

                            //get the TIME
                            if (TimeSpan.TryParse(c.Text, out TimeSpan timeDel))
                            {
                                data[2] = timeDel.ToString();
                            };

                            if (reminders.Contains(c.Text))
                            {
                                data[3] = Array.IndexOf(reminders, c.Text).ToString();
                            };

                        }
                    }

                    string deletionTarget = $"{data[0]}|{data[1]}|{data[2]}|{data[3]}";

                    foreach (ScheduleItem sI in scheduledItems)
                    {
                        //DEBUGGING testing deletion
                        //MessageBox.Show($"TARGET - {deletionTarget}\nCURRENT - {sI.toString()}\nMATCH - {sI.toString().Equals(deletionTarget)}");
                        if (sI.toString().Equals(deletionTarget))
                        {
                            scheduledItems.Remove(sI);
                            updateScheduleFile();
                            updateScheduleUI();
                            return;
                        }
                    }
                }
            }
        }

        private void updateScheduleFile()
        {
            using (StreamWriter sw = new StreamWriter("Schedule.txt"))
            {
                foreach (ScheduleItem item in scheduledItems)
                {
                    sw.WriteLine(
                        $"{item.Action}|{item.Date:dd/MM/yyyy}|{item.Time:hh\\:mm}|{item.Reminder}"
                        );
                }
            }
        }

        private void btnSaveSchedule_Click(object sender, EventArgs e)
        {
            TimeSpan scheduleTime;

            foreach (ScheduleItem item in scheduledItems)
            {
                //MessageBox.Show($"Saved: {item.Action}|{item.Date}|{item.Time}|{item.Reminder}\nNew: {cmbScheduleOperation.SelectedIndex.ToString()}|{cmbScheduleDate.Value}|{TimeSpan.Parse(cmbScheduleTime.Text)}|{cmbRemindMe.SelectedIndex.ToString()}");
                if (item.Action == cmbScheduleOperation.SelectedIndex.ToString()
                    && item.Date == cmbScheduleDate.Value.Date
                    && item.Time == TimeSpan.Parse(cmbScheduleTime.Text)
                    && item.Reminder == cmbRemindMe.SelectedIndex.ToString())
                {
                    MessageBox.Show("This item already exists");
                    return;
                }

                else if (item.Date == cmbScheduleDate.Value.Date
                    && item.Time == TimeSpan.Parse(cmbScheduleTime.Text))
                {
                    MessageBox.Show("Item already scheduled for this date/time");
                    return;
                }
            }

            DateTime validDate = new DateTime(cmbScheduleDate.Value.Date.Year, cmbScheduleDate.Value.Month, cmbScheduleDate.Value.Day);
            TimeSpan validTime = TimeSpan.Parse(cmbScheduleTime.Text);

            DateTime validationDate = validDate.Date + validTime;

            if (validationDate < DateTime.Now)
            {
                MessageBox.Show("This time has already passed");
                return;
            }

            if (btnSaveSchedule.Text == "Save")
            {
                if (!TimeSpan.TryParse(
                cmbScheduleTime.GetItemText(cmbScheduleTime.SelectedItem),
                out scheduleTime))
                {
                    MessageBox.Show("Invalid time selected");
                    return;
                }

                ScheduleItem newItem = new ScheduleItem(
                    cmbScheduleOperation.GetItemText(cmbScheduleOperation.SelectedIndex),
                    cmbScheduleDate.Value.Date,
                    scheduleTime,
                    cmbRemindMe.GetItemText(cmbRemindMe.SelectedIndex)
                    );

                scheduledItems.Add(newItem);

                updateScheduleFile();
                updateScheduleUI();
            }
            
            else if (btnSaveSchedule.Text == "Update Schedule")
            {
                string[] data = editTarget.Split('|');
                //update the item instead of creating a new one

                //first find the current item in the list and update it. use edittarget??
                //MessageBox.Show(editTarget);
                ScheduleItem target = new ScheduleItem(
                   data[0],
                   DateTime.ParseExact(data[1], "dd/MM/yyyy HH:mm:ss", null),
                   TimeSpan.Parse(data[2]),
                   data[3]);

                foreach (ScheduleItem item in scheduledItems)
                {
                    //MessageBox.Show($"Current Item: {item.toString()}\nTarget Item: {target.toString()}");
                    if (item.Action == target.Action
                        && item.Date == target.Date
                        && item.Time == target.Time
                        && item.Reminder == target.Reminder)
                    {
                        //read the values on the form into a new ScheduleItem
                        ScheduleItem updated = new ScheduleItem(
                            cmbScheduleOperation.SelectedIndex.ToString(),
                            cmbScheduleDate.Value,
                            TimeSpan.Parse(cmbScheduleTime.Text),
                            cmbRemindMe.SelectedIndex.ToString()
                            );

                        //MessageBox.Show(updated.toString());

                        //add the new ScheduleItem to the list
                        scheduledItems[scheduledItems.IndexOf(item)] = updated;

                        //update the file and UI to fully save.
                        updateScheduleFile();
                        updateScheduleUI();

                        MessageBox.Show("Schedule Updated");

                        btnSaveSchedule.Text = "Save";
                        pnlSavedSchedules.Enabled = true;
                        return;
                    }
                }
            } 
        }

        private void tglAOT_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void imgHeaderDivider_Click(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cmbOperation.SelectedIndex = 0;
            cmbScheduleOperation.SelectedIndex = 0;
            cmbScheduleTime.SelectedIndex = 0;
            cmbScheduleDate.MinDate = DateTime.Today;
            cmbRemindMe.SelectedIndex = 0;

            btnClearSchedule.BorderColor = Color.FromArgb(247, 62, 62);

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
