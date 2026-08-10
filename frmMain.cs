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

namespace SleepyTime_2._0
{
    public partial class frmMain : Form
    {
        //rounded borders values
        private int borderRadius = 30, BorderSize = 2;
        private Color boderColour = Color.Yellow;

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
        }

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = ("Current Time: " + DateTime.Now.ToString("HH:mm"));
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
            FormRegionAndBorder(this, borderRadius, e.Graphics, Color.FromArgb(45, 31, 48), 2);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //if (cmbOperations.Items.Count>0)
            //{
            //    cmbOperations.SelectedIndex = 0;
            //}
        }
    }
}
