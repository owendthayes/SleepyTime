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
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;

namespace SleepyTime_2._0
{
    public partial class frmMain : Form
    {
        //DELETE THIS LATER
        private bool notifShown = false;


        //rounded borders values
        private int borderRadius = 30, BorderSize = 2;

        private bool countdownStarted = false;
        private TimeSpan remainingTime;
        private bool countdownEnded = false;

        //Drag and Drop functionality for form header.
        private bool Dragging = false;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        //list for populating scheduled items.
        private List<ScheduleItem> scheduledItems = new List<ScheduleItem>();
        private List<PresetItem> presetItems = new List<PresetItem>();

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
        private Color textColor = Color.Black;
        private Color secondaryTextColor = Color.FromArgb(177, 178, 181);

        Color primaryTheme;
        Color secondaryTheme;

        private bool quickActionsHidden = false;

        //ALWAYS ON TOP
        private bool formAOT = false;

        ScheduleItem scheduleEditTarget;
        PresetItem presetEditTarget;

        private string[] operations = { "Shutdown", "Restart", "Sleep", "Lock" };
        private string[] reminders = { "No Reminder", "5 Mins", "10 Mins", "15 Mins", "30 Mins", "1 Hour", "2 Hours" };
        private string[] reminderMins = { "0", "5", "10", "15", "30", "60", "120" };

        public frmMain()
        {
            InitializeComponent();

            readSettingsFile();
            readPresetFile();
            //LOAD IN THE ACCENT COLOUR FROM A FILE OR SOMETHING!!!
            getAccentColour();
            applyAccentColour(primaryAccent, secondaryAccent);

            readScheduleFile();
            populateTimesComboBox();
            updatePresetUI();
            updateScheduleUI();

            applyDarkMode(mainTheme);


            //further options for rounded form borders
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(BorderSize);

            lblCurrentTime.Text = (DateTime.Now.ToString("HH:mm"));

            tmrMain.Start();
            tmrValidation.Start();
            tmrCurrentTime.Start();
        }

        private void populateTimesComboBox()
        {
            cmbScheduleTime.Items.Clear();
            cmbPresetTime.Items.Clear();

            for (int minutes = 0; minutes < 24 * 60; minutes += 5)
            {
                TimeSpan time = TimeSpan.FromMinutes(minutes);
                cmbScheduleTime.Items.Add(time.ToString(@"hh\:mm"));
                cmbPresetTime.Items.Add(time.ToString(@"hh\:mm"));
            }
        }

        private void readPresetFile()
        {
            if (!File.Exists("Preset.txt"))
            {
                File.Create("Preset.txt");
            }

            string[] lines = File.ReadAllLines("Preset.txt");

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] data = line.Split('|');

                if (data.Length != 6)
                    continue;

                PresetItem item = new PresetItem(data[0], data[1], data[2], TimeSpan.Parse(data[3]), data[4], Convert.ToBoolean(data[5]));
                presetItems.Add(item);
            }
        }

        private void readScheduleFile()
        {
            bool messageShown = false;

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

                if (data.Length != 5)
                    continue;

                if (!DateTime.TryParseExact(
                    data[1],
                    "dd/MM/yyyy",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime date))
                {
                    MessageBox.Show("Date incorrect", "Error");
                    continue;
                }



                if (!TimeSpan.TryParse(data[2], out TimeSpan time))
                {
                    MessageBox.Show("Time incorrect", "Error");
                    continue;
                }

                DateTime givenDate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);

                if (DateTime.Now < givenDate)
                {
                    scheduledItems.Add(
                        new ScheduleItem(
                            data[0],
                            date,
                            time,
                            data[3],
                            bool.Parse(data[4])
                            )
                        );
                }
                else if (messageShown == false)
                {
                    MessageBox.Show("The date of one or more of your saved schedules has passed\nThey have been removed.", "Notice");
                    messageShown = true;
                }

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
                    "false"
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
                case "False":
                    mainTheme = "light";
                    break;

                case "True":
                    mainTheme = "dark";
                    break;
            }

            applyDarkMode(mainTheme);
        }

        private void applyDarkMode(string mode)
        {
            string whiteLogo = Path.GetFullPath("Resources\\SleepyTimeImg.png");
            string blackLogo = Path.GetFullPath("Resources\\SleepyTimeImgBlack.png");

            textColor = Color.Black;
            switch (mode)
            {
                case "dark":
                    primaryTheme = Color.FromArgb(13, 15, 28);
                    secondaryTheme = Color.FromArgb(25, 25, 41);
                    textColor = Color.White;
                    secondaryTextColor = Color.FromArgb(177, 178, 181);
                    imgLogo.Image = Image.FromFile(whiteLogo);
                    imgAboutLogo.Image = Image.FromFile(whiteLogo);
                    break;

                case "light":
                    primaryTheme = Color.FromArgb(245, 245, 250);
                    secondaryTheme = Color.FromArgb(211, 211, 230);
                    textColor = Color.Black;
                    secondaryTextColor = Color.FromArgb(39, 39, 41);
                    imgLogo.Image = Image.FromFile(blackLogo);
                    imgAboutLogo.Image = Image.FromFile(blackLogo);
                    break;
            }

            foreach (Control c in GetAllControls(this))
            {
                if (c.ForeColor == Color.White || c.ForeColor == Color.Black)
                {
                    c.ForeColor = textColor;
                }

                if (c.BackColor == Color.FromArgb(13, 15, 28) || c.BackColor == Color.FromArgb(245, 245, 250))
                {
                    c.BackColor = primaryTheme;
                }

                if (c.BackColor == Color.FromArgb(25, 25, 41) || c.BackColor == Color.FromArgb(211, 211, 230))
                {
                    c.BackColor = secondaryTheme;
                }

                if (c.ForeColor == Color.FromArgb(177, 178, 181) || c.ForeColor == Color.FromArgb(39, 39, 41))
                {
                    c.ForeColor = secondaryTextColor;
                }
            }
            this.BackColor = primaryTheme;


        }

        private void applyAccentColour(Color accentColour, Color secondaryAccent)
        {
            foreach (Control c in GetAllControls(this))
            {
                if (c.ForeColor == Color.FromArgb(140, 71, 203) || c.ForeColor == Color.FromArgb(35, 35, 204) || c.ForeColor == Color.LimeGreen || c.ForeColor == Color.FromArgb(197, 217, 20) || c.ForeColor == Color.FromArgb(222, 13, 13))
                {
                    c.ForeColor = primaryAccent;
                }

                if (c is RoundedButton button && c.Tag != "noColourChange" && c.ForeColor != Color.FromArgb(247, 62, 62))
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
            switch (accentColour)
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
            //if there are no saved scheduled items, dont run this.
            if (scheduledItems.Count < 1)
            {
                return;
            }

            //find the soonest time that a reminder should trigger
            DateTime soonestReminder = DateTime.MaxValue;
            ScheduleItem soonestItemReminder = null;

            foreach (ScheduleItem sI in scheduledItems)
            {
                if (sI.Reminder == "0")
                {
                    continue;
                }

                //calculate what time the reminder should trigger
                DateTime reminderTime = sI.Date.Date + sI.Time - TimeSpan.FromMinutes(Convert.ToDouble(reminderMins[Convert.ToInt32(sI.Reminder)]));

                //if this reminder is the soonest, then update "soonestReminder"
                if (reminderTime < soonestReminder)
                {
                    soonestReminder = reminderTime;
                    soonestItemReminder = sI;
                }
            }

            //MessageBox.Show($"NOW: {DateTime.Now.ToString(@"dd/MM/yyyy HH:mm")}\nREMINDER: {soonestReminder.ToString(@"dd/MM/yyyy HH:mm")}");

            //if the time now matches the reminder time then send the reminder.
            if (DateTime.Now.ToString(@"dd/MM/yyyy HH:mm") == soonestReminder.ToString(@"dd/MM/yyyy HH:mm"))
            {
                sendReminderNotification(soonestItemReminder.Reminder, soonestItemReminder);
                scheduledItems[scheduledItems.IndexOf(soonestItemReminder)].ReminderSent = true;
            }

            //find the soonest time that a scheduled item will happen
            DateTime soonestAction = DateTime.MaxValue;
            ScheduleItem soonestItemAction = null;

            foreach (ScheduleItem sI in scheduledItems)
            {
                //calculate what time the action should occur
                DateTime actionTime = sI.Date.Date + sI.Time;

                if (actionTime < soonestAction)
                {
                    soonestAction = actionTime;
                    soonestItemAction = sI;
                }
            }
            
            //if the time matches the scheduled time then perform the action
            if (DateTime.Now.ToString(@"dd/MM/yyyy HH:mm") == soonestAction.ToString(@"dd/MM/yyyy HH:mm"))
            {
                performAction(soonestItemAction.Action);
                scheduledItems.Remove(soonestItemAction);
                updateScheduleFile();
                updateScheduleUI();
            }
        }

        private void performAction(string action)
        {
            switch (action)
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

        private void sendReminderNotification(string reminder, ScheduleItem soonest)
        {
            string timePeriod = "";
            string[] operations = { "Shutdown", "Restart", "Sleep", "Lock" };
            string notifAction = operations[Convert.ToInt32(soonest.Action)];

            switch (reminder)
            {
                case "1": // 5 mins
                    timePeriod = "5 Minutes";
                    break;

                case "2": // 10 mins
                    timePeriod = "10 Minutes";
                    break;

                case "3": // 15 mins
                    timePeriod = "15 Minutes";
                    break;

                case "4": // 30 mins
                    timePeriod = "30 Minutes";
                    break;

                case "5": // 1 hr
                    timePeriod = "1 Hour";
                    break;

                case "6": // 2 hr
                    timePeriod = "2 Hours";
                    break;
            }
            ntfReminder.Icon = new System.Drawing.Icon(Path.GetFullPath("Resources\\SleepyTimeIcon.ico"));
            ntfReminder.Text = "Some Text";
            ntfReminder.Visible = true;
            ntfReminder.BalloonTipTitle = $"Your computer will {notifAction} in {timePeriod}";
            ntfReminder.BalloonTipText = "Click to open SleepyTime";

            //show the notification for one minute, not working.
            ntfReminder.ShowBalloonTip(60000);

            updateScheduleFile();
            updateScheduleUI();
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
            FormRegionAndBorder(this, borderRadius, e.Graphics, primaryTheme, 2);
        }

        private void btnExit_MouseEnter(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.FromArgb(169, 5, 5);
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            btnMinimize.BackColor = secondaryTheme;
        }

        private void btnExit_MouseLeave(object sender, EventArgs e)
        {
            btnExit.BackColor = primaryTheme;
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.BackColor = primaryTheme;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult exitBox = MessageBox.Show("Are you sure you want to exit?\n\nScheduled actions will not occur when SleepyTime is closed.", "Close SleepyTime", MessageBoxButtons.YesNo);
            {
                if (exitBox == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void greyOutSidebar()
        {

            btnSidebarAbout.ForeColor = secondaryTextColor;
            btnSidebarCountdown.ForeColor = secondaryTextColor;
            btnSideBarSettings.ForeColor = secondaryTextColor;
            btnSidebarSchedule.ForeColor = secondaryTextColor;
            btnSideBarPresets.ForeColor = secondaryTextColor;
            btnHelp.ForeColor = secondaryTextColor;

            btnSidebarAbout.BackColor = primaryTheme;
            btnSidebarCountdown.BackColor = primaryTheme;
            btnSideBarSettings.BackColor = primaryTheme;
            btnSidebarSchedule.BackColor = primaryTheme;
            btnSideBarPresets.BackColor = primaryTheme;
            btnHelp.BackColor = primaryTheme;
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
                    MessageBox.Show("Please enter a valid time", "Error");
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
            btnStartCountdown.BorderColor = primaryAccent;

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
                performAction(cmbOperation.SelectedIndex.ToString());
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
                btnSaveSchedule.Text = "Save";
                pnlSavedSchedules.Enabled = true;
            }

            cmbScheduleOperation.SelectedIndex = 0;
            cmbScheduleTime.SelectedIndex = 0;
            cmbRemindMe.SelectedIndex = 0;
            cmbScheduleDate.Value = DateTime.Today;
        }

        private void updateScheduleUI()
        {
            foreach (Control ctr in pnlSavedSchedules.Controls.Cast<Control>().ToList())
            {
                if (ctr != lblSavedItems)
                {
                    pnlSavedSchedules.Controls.Remove(ctr);
                    ctr.Dispose();
                }
            }

            // inform the user if there are no items currently saved.
            lblSavedItems.Visible = scheduledItems.Count == 0;
            lblSavedItems.BringToFront();

            int y = 10;

            scheduledItems = scheduledItems
                .OrderBy(item => item.Date.Date + item.Time)
                .ToList();

            foreach (ScheduleItem item in scheduledItems)
            {
                Panel row = new Panel();

                row.BackColor = secondaryTheme;

                row.Width = pnlSavedSchedules.Width - 40;
                row.Height = 40;
                row.Location = new Point(10, y);

                //add the controls here

                Label lblAction = new Label
                {
                    Text = operations[Convert.ToInt32(item.Action)],
                    Location = new Point(10, 10),
                    AutoSize = true,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12)
                };

                Label lblDate = new Label
                {
                    Text = item.Date.ToString(@"dd/MM/yyyy"),
                    Location = new Point(110, 10),
                    AutoSize = true,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12),
                };

                Label lblTime = new Label
                {
                    Text = item.Time.ToString(@"hh\:mm"),
                    Location = new Point(225, 10),
                    AutoSize = true,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12),
                };

                Label lblReminder = new Label
                {
                    Text = reminders[Convert.ToInt32(item.Reminder)],
                    Location = new Point(310, 10),
                    AutoSize = true,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12),
                };

                RoundedButton btnEditSchedule = new RoundedButton
                {
                    Text = "✎",
                    Location = new Point(450, 5),
                    AutoSize = true,
                    ForeColor = primaryAccent,
                    BorderColor = primaryAccent,
                    BackColor = primaryTheme,
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25,
                    Cursor = Cursors.Hand,
                    Tag = item
                };

                RoundedButton btnDeleteSchedule = new RoundedButton
                {
                    Text = "🗑",
                    Location = new Point(500, 5),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(247, 62, 62),
                    BorderColor = Color.FromArgb(247, 62, 62),
                    BackColor = primaryTheme,
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25,
                    Tag = item,
                    Cursor = Cursors.Hand
                };

                //if a notification has already sent, dont allow the user to edit the schedule.
                if (item.ReminderSent)
                {
                    btnEditSchedule.Enabled = false;
                }

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
            //get the target
            RoundedButton btn = (RoundedButton)sender;
            ScheduleItem target = btn.Tag as ScheduleItem;

            //disable and update UI elements to allow for saving later.
            pnlSavedSchedules.Enabled = false;
            btnSaveSchedule.Text = "Update Schedule";
            btnClearSchedule.Text = "Cancel";

            //load the option boxes with the corresponding data.
            cmbScheduleOperation.SelectedIndex = Convert.ToInt32(target.Action);
            cmbScheduleDate.Value = target.Date;
            cmbScheduleTime.SelectedIndex = cmbScheduleTime.Items.IndexOf(target.Time.ToString(@"hh\:mm"));
            cmbRemindMe.SelectedIndex = Convert.ToInt32(target.Reminder);

            pnlSavedSchedules.Enabled = false;

            btnClearSchedule.Text = "Cancel";
            btnSaveSchedule.Text = "Update Schedule";

            scheduleEditTarget = target;
        }

        private void btnDeleteSchedule_Click(object sender, EventArgs e)
        {
            RoundedButton btn = (RoundedButton)sender;
            ScheduleItem target = btn.Tag as ScheduleItem;

            DialogResult exitBox = MessageBox.Show("Delete this Action?", "Delete", MessageBoxButtons.YesNo);
            {               
                foreach (ScheduleItem sI in scheduledItems)
                {
                    //DEBUGGING testing deletion
                    //MessageBox.Show($"TARGET - {deletionTarget}\nCURRENT - {sI.toString()}\nMATCH - {sI.toString().Equals(deletionTarget)}");
                    if (sI == target)
                    {
                        scheduledItems.Remove(sI);
                        updateScheduleFile();
                        updateScheduleUI();
                        return;
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
                        $"{item.Action}|{item.Date:dd/MM/yyyy}|{item.Time:hh\\:mm}|{item.Reminder}|{item.ReminderSent}"
                        );
                }
            }
        }

        private void btnSaveSchedule_Click(object sender, EventArgs e)
        {
            TimeSpan scheduleTime;

            DateTime validDate = new DateTime(cmbScheduleDate.Value.Date.Year, cmbScheduleDate.Value.Month, cmbScheduleDate.Value.Day);
            TimeSpan validTime = TimeSpan.Parse(cmbScheduleTime.Text);

            DateTime validationDate = validDate.Date + validTime;

            // check that the chosen reminder time has not already passed.
            TimeSpan proposedReminderTime = TimeSpan.Parse(cmbScheduleTime.Text).Subtract(TimeSpan.FromMinutes(Convert.ToDouble(reminderMins[cmbRemindMe.SelectedIndex])));
            DateTime proposedReminderDate = validDate + proposedReminderTime;


            foreach (ScheduleItem item in scheduledItems)
            {
                DateTime itemDate = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day, item.Time.Hours, item.Time.Minutes, item.Time.Seconds);
                itemDate = itemDate.Subtract(TimeSpan.FromMinutes(Convert.ToDouble(reminderMins[Convert.ToInt32(item.Reminder)])));

                //MessageBox.Show($"Saved: {item.Action}|{item.Date}|{item.Time}|{item.Reminder}\nNew: {cmbScheduleOperation.SelectedIndex.ToString()}|{cmbScheduleDate.Value}|{TimeSpan.Parse(cmbScheduleTime.Text)}|{cmbRemindMe.SelectedIndex.ToString()}");
                if (item.Action == cmbScheduleOperation.SelectedIndex.ToString()
                    && item.Date == cmbScheduleDate.Value.Date
                    && item.Time == TimeSpan.Parse(cmbScheduleTime.Text)
                    && item.Reminder == cmbRemindMe.SelectedIndex.ToString())
                {
                    MessageBox.Show("This item already exists", "Could not save item.");
                    return;
                }

                else if (item.Date == cmbScheduleDate.Value.Date
                    && item.Time == TimeSpan.Parse(cmbScheduleTime.Text)
                    && btnSaveSchedule.Text != "Update Schedule")
                {
                    MessageBox.Show("Item already scheduled for this date/time", "Could not save item.");
                    return;
                }

                else if (itemDate == proposedReminderDate)
                {
                    DialogResult exitBox = MessageBox.Show("You have an reminder scheduled at this time already.\nSave anyway?", "Save", MessageBoxButtons.YesNo);
                    {
                        if (exitBox == DialogResult.No)
                        {
                            return;
                        }
                    }
                }
            }

            if (validationDate < DateTime.Now)
            {
                MessageBox.Show("This time has already passed", "Could not save item.");
                return;
            }


            if (proposedReminderDate < DateTime.Now)
            {
                MessageBox.Show("You can't set a reminder for a time that has already passed", "Could not save item.");
                return;
            }


            if (btnSaveSchedule.Text == "Save")
            {
                if (!TimeSpan.TryParse(
                cmbScheduleTime.GetItemText(cmbScheduleTime.SelectedItem),
                out scheduleTime))
                {
                    MessageBox.Show("Invalid time selected", "Error");
                    return;
                }

                ScheduleItem newItem = new ScheduleItem(
                    cmbScheduleOperation.GetItemText(cmbScheduleOperation.SelectedIndex),
                    cmbScheduleDate.Value.Date,
                    scheduleTime,
                    cmbRemindMe.GetItemText(cmbRemindMe.SelectedIndex),
                    false
                    );
                newItem.ReminderSent = false;

                scheduledItems.Add(newItem);

                updateScheduleFile();
                updateScheduleUI();
            }

            else if (btnSaveSchedule.Text == "Update Schedule")
            {
                foreach (ScheduleItem item in scheduledItems)
                {
                    if (item == scheduleEditTarget)
                    {
                        //read the values on the form into a new ScheduleItem
                        ScheduleItem updated = new ScheduleItem(
                            cmbScheduleOperation.SelectedIndex.ToString(),
                            cmbScheduleDate.Value,
                            TimeSpan.Parse(cmbScheduleTime.Text),
                            cmbRemindMe.SelectedIndex.ToString(),
                            item.ReminderSent
                            );

                        //add the new ScheduleItem to the list
                        scheduledItems[scheduledItems.IndexOf(item)] = updated;

                        //update the file and UI to fully save.
                        updateScheduleFile();
                        updateScheduleUI();

                        MessageBox.Show("Schedule Updated", "Success");

                        btnSaveSchedule.Text = "Save";
                        btnClearSchedule.Text = "Reset";
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

        private void pnlCountdown_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblShowHideQuick_Click(object sender, EventArgs e)
        {
            if (quickActionsHidden == false)
            {
                btnQuick1.Visible = false;
                btnQuick15.Visible = false;
                btnQuick2.Visible = false;
                btnQuick30.Visible = false;
                btnMoreQuick.Visible = false;

                lblShowHideQuick.Text = "Quick Timers ▶";
            }
            else
            {
                btnQuick1.Visible = true;
                btnQuick15.Visible = true;
                btnQuick2.Visible = true;
                btnQuick30.Visible = true;
                btnMoreQuick.Visible = true;

                lblShowHideQuick.Text = "Quick Timers ▼";
            }

            quickActionsHidden = !quickActionsHidden;
        }

        private void ntfReminder_BalloonTipClicked(object sender, EventArgs e)
        {
            //open sleepytime again.
            this.WindowState = FormWindowState.Normal;
        }

        private void tmrCurrentTime_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = (DateTime.Now.ToString("HH:mm"));
            imgTimeAnimation.Visible = !imgTimeAnimation.Visible;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cmbOperation.SelectedIndex = 0;
            cmbScheduleOperation.SelectedIndex = 0;
            cmbScheduleTime.SelectedIndex = 0;
            cmbScheduleDate.MinDate = DateTime.Today;
            cmbRemindMe.SelectedIndex = 0;
            cmbPresetTime.SelectedIndex = 0;
            cmbPresetAction.SelectedIndex = 0;
            cmbPresetRepeat.SelectedIndex = 0;

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

            btnPresetDaysDropDown.FlatStyle = FlatStyle.Flat;
            btnPresetDaysDropDown.FlatAppearance.BorderSize = 0;

            btnSidebarCountdown.PerformClick();
        }
        private void cmbPresetDays_Click(object sender, EventArgs e)
        {
            pnlPresetDays.Visible = !pnlPresetDays.Visible;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnlPresetDays.Visible = !pnlPresetDays.Visible;
        }


        private void listBoxDays_MouseLeave(object sender, EventArgs e)
        {

        }


        private void cmbPresetDays_TextChanged(object sender, EventArgs e)
        {

        }

        private void pnlPresetDays_Leave(object sender, EventArgs e)
        {
            //pnlPresetDays.Visible = false;
        }

        private void cmbPresetRepeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPresetRepeat.SelectedIndex == 1)
            {
                btnPresetDaysDropDown.Visible = true;
                cmbPresetDays.Visible = true;
                label29.Visible = true;
            }
            else
            {
                btnPresetDaysDropDown.Visible = false;
                cmbPresetDays.Visible = false;
                label29.Visible = false;
                pnlPresetDays.Visible = false;
            }
        }

        private void btnPresetSave_Click(object sender, EventArgs e)
        {
            string name = "Unnamed Preset";

            //validation here
            if (!string.IsNullOrEmpty(txtPresetName.Text))
            {
                name = txtPresetName.Text;
            }
            else
            {
                MessageBox.Show("Please enter a name for your preset action");
                return;
            }


            //check selected days for repeating
            string days = "-------";

            if (cmbPresetRepeat.SelectedIndex == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (listBoxDays.GetItemChecked(i))
                    {
                        days = days.Remove(i, 1).Insert(i, "x");
                    }
                }
            }
            else //if repeat is set to "every day" selecting certain days is not necessary
            {
                days = "xxxxxxx";
            }

            //create the new preset item
            PresetItem newItem = new PresetItem(
                name,
                cmbPresetAction.SelectedIndex.ToString(),
                cmbPresetRepeat.SelectedIndex.ToString(),
                TimeSpan.Parse(cmbPresetTime.Text),
                days,
                tglPresetEnabled.Checked
                );

            //decide if we are editing an existing item or creating a new one.
            if (btnPresetSave.Text == "Save Preset")
            {
                //save it to the list
                presetItems.Add(newItem);
            }
            else if (btnPresetSave.Text == "Update")
            {
                presetItems[presetItems.IndexOf(presetEditTarget)] = newItem;
                pnlSavedPresets.Enabled = true;
                btnPresetSave.Text = "Save Preset";
                btnPresetCancel.Text = "Reset";
                btnPresetCancel.PerformClick();
            }

            //write the list to the file
            updatePresetFile();
            updatePresetUI();
        }

        private void updatePresetFile()
        {
            using (StreamWriter sw = new StreamWriter("Preset.txt"))
            {
                foreach (PresetItem item in presetItems)
                {
                    sw.WriteLine(
                        $"{item.Name}|{item.Action}|{item.Repeat}|{item.Time}|{item.Days}|{item.Enabled}"
                        );
                }
            }
        }

        //CHECK THAT THE SAVED ITEMS ARE BEING READ TO THE LIST PROPERLY!!!

        private void updatePresetUI()
        {
            string daysOfWeek = "MTWTFSS";

            //get rid of everything from the panel.
            foreach (Control ctr in pnlSavedPresets.Controls.Cast<Control>().ToList())
            {
                if (ctr != lblSavedItemsPresets)
                {
                    pnlSavedPresets.Controls.Remove(ctr);
                    ctr.Dispose();
                }
            }

            // if there is nothing currently saved in presets then inform the user.
            lblSavedItemsPresets.Visible = presetItems.Count == 0;
            lblSavedItemsPresets.BringToFront();

            // add each preset item as a row in the panel.
            int y = 10;

            foreach (PresetItem item in presetItems)
            {
                string selectedDays = "";
                for (int i = 0; i < 7; i++)
                {
                    if (item.Days[i] == 'x')
                    {
                        selectedDays += daysOfWeek[i];
                    }
                    else
                    {
                        selectedDays += "-";
                    }
                }

                Panel row = new Panel();

                row.BackColor = secondaryTheme;

                row.Width = pnlSavedPresets.Width - 40;
                row.Height = 40;
                row.Location = new Point(10, y);

                //add the controls here

                Label lblName = new Label
                {
                    Text = item.Name,
                    Location = new Point(60, 10),
                    AutoSize = true,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12)
                };

                Label lblAction = new Label
                {
                    Text = operations[Convert.ToInt32(item.Action)],
                    Location = new Point(220, 10),
                    AutoSize = true,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12),
                };

                Label lblTime = new Label
                {
                    Text = item.Time.ToString(@"hh\:mm"),
                    Location = new Point(435, 10),
                    AutoSize = true,
                    Width = 50,
                    ForeColor = textColor,
                    Font = new Font("JetBrains Mono", 12),
                };

                // potentially only do this for when days are selected.
                FlowLayoutPanel flpDays = new FlowLayoutPanel
                {
                    Location = new Point(320, 1),
                    Width = 100,
                    AutoSize = true,
                    BackColor = secondaryTheme
                };

                for (int i = 0; i < 7; i++)
                {
                    Label dayLabel = new Label();

                    dayLabel.Text = daysOfWeek[i].ToString();
                    dayLabel.TextAlign = ContentAlignment.MiddleCenter;

                    dayLabel.Size = new Size(10, 35);
                    dayLabel.Margin = new Padding(2);

                    if (item.Repeat == "1")
                    {
                        if (item.Days[i] == 'x')
                        {
                            dayLabel.ForeColor = primaryAccent;
                        }
                        else
                        {
                            dayLabel.ForeColor = Color.Gray;
                        }
                    }
                    else
                    {
                        dayLabel.ForeColor = primaryAccent;
                    }

                    dayLabel.BackColor = secondaryTheme;

                    dayLabel.Font = new Font(
                        "Jetbrains Mono",
                        10,
                        FontStyle.Regular
                    );

                    dayLabel.Tag = i;

                    flpDays.Controls.Add(dayLabel);

                }

                ToggleButton tglEnabled = new ToggleButton
                {
                    Location = new Point(10, 10),
                    AutoSize = true,
                    OnBackColor = primaryAccent,
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25,
                    Checked = item.Enabled,
                    Tag = item,
                    Cursor = Cursors.Hand
                };

                RoundedButton btnEditPreset = new RoundedButton
                {
                    Text = "✎",
                    Location = new Point(510, 5),
                    AutoSize = true,
                    ForeColor = primaryAccent,
                    BorderColor = primaryAccent,
                    BackColor = primaryTheme,
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25,
                    Tag = item,
                    Cursor = Cursors.Hand
                };

                RoundedButton btnDeletePreset = new RoundedButton
                {
                    Text = "🗑",
                    Location = new Point(560, 5),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(247, 62, 62),
                    BorderColor = Color.FromArgb(247, 62, 62),
                    BackColor = primaryTheme,
                    Font = new Font("JetBrains Mono", 12),
                    Width = 25,
                    Height = 25,
                    Tag = item,
                    Cursor = Cursors.Hand
                };

                btnEditPreset.Click += btnEditPreset_Click;
                btnDeletePreset.Click += btnDeletePreset_Click;
                tglEnabled.CheckedChanged += tglEnabled_CheckedChanged;

                row.Controls.Add(lblName);
                row.Controls.Add(lblAction);
                row.Controls.Add(lblTime);
                row.Controls.Add(flpDays);
                row.Controls.Add(tglEnabled);
                row.Controls.Add(btnEditPreset);
                row.Controls.Add(btnDeletePreset);

                pnlSavedPresets.Controls.Add(row);
                y += row.Height + 5;
            }

        }



        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void btnPresetCancel_Click(object sender, EventArgs e)
        {

            txtPresetName.Text = "";
            cmbPresetAction.SelectedIndex = 0;
            cmbPresetRepeat.SelectedIndex = 0;
            cmbPresetTime.SelectedIndex = 0;
            tglPresetEnabled.Checked = true;
            uncheckAll();
            cmbPresetDays.Text = "None selected";

            if (btnPresetCancel.Text == "Cancel")
            {
                pnlSavedPresets.Enabled = true;
                btnPresetSave.Text = "Save Preset";
                btnPresetCancel.Text = "Reset";
                btnPresetCancel.PerformClick();
            }
        }

        private void uncheckAll()
        {
            for (int i = 0; i < listBoxDays.Items.Count; i++)
            {
                listBoxDays.SetItemChecked(i, false);
            }
            listBoxDays.SelectedIndex = -1;
        }

        private void btnEditPreset_Click(object sender, EventArgs e)
        {
            int xCount = 0;

            //get the target
            RoundedButton btn = (RoundedButton)sender;
            PresetItem target = btn.Tag as PresetItem;

            //disable and update UI elements to allow for saving later.
            pnlSavedPresets.Enabled = false;
            btnPresetSave.Text = "Update";
            btnPresetCancel.Text = "Cancel";

            //load the info into the UI
            txtPresetName.Text = target.Name;
            cmbPresetAction.SelectedIndex = Convert.ToInt32(target.Action);
            cmbPresetRepeat.SelectedIndex = Convert.ToInt32(target.Repeat);
            cmbPresetTime.SelectedIndex = cmbPresetTime.Items.IndexOf(target.Time.ToString(@"hh\:mm"));
            tglPresetEnabled.Checked = target.Enabled;

            for (int i = 0; i < 7; i++)
            {
                if (target.Days[i] == 'x')
                {
                    listBoxDays.SetItemChecked(i, true);
                    xCount++;
                }
                else
                {
                    listBoxDays.SetItemChecked(i, false);
                }
            }

            cmbPresetDays.Text = $"{xCount} days selected";

            //this will be used in the save button function.
            presetEditTarget = target;
        }

        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            RoundedButton btn = (RoundedButton)sender;
            PresetItem target = btn.Tag as PresetItem;

            DialogResult exitBox = MessageBox.Show($"Are you sure you want to delete preset:\n{target.Name}", "Delete", MessageBoxButtons.YesNo);
            {
                if (exitBox == DialogResult.Yes)
                {
                    presetItems.Remove(target);
                    updatePresetFile();
                    updatePresetUI();
                }
            }
        }

        private void listBoxDays_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPresetDays.Text = $"{listBoxDays.CheckedItems.Count.ToString()} days selected";
        }

        private void tglEnabled_CheckedChanged(object sender, EventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            PresetItem target = btn.Tag as PresetItem;

            foreach (PresetItem item in presetItems)
            {
                if (item.Name == target.Name)
                {
                    item.Enabled = btn.Checked;
                    break;
                }
            }
            //rewrite the list to the file, ui update not necessary as this is essentially already handled by the toggle button.
            updatePresetFile();

        }

        private void lblBugReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink("https://forms.gle/haAHduytqaXaEShFA");
        }
    }
}

