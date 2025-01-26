using System;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using NvAPIWrapper.Mosaic;

namespace Resonant
{
    public partial class Resonant : Form
    {
        private readonly Random random = new();
        private readonly SnowOverlay overlay;

#pragma warning disable CS8618, CA2211 // Non-nullable + Non-constant
        public static Resonant Instance;
        Form Pf;
#pragma warning restore CS8618, CA2211 // Non-nullable + Non-constant

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        public Resonant()
        {
            InitializeComponent();
            Instance = this;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            Loadform(new Dashboard());
            versionID.Text = "v" + GetCurrentVersion();

            // Initialize the snow overlay
            // overlay = new SnowOverlay(this);
            // overlay.Show();
        }

        private static string GetCurrentVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return version ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.3";
        }

        public void Loadform(Object Form)
        {
            Form f = (Form)Form;
            if (mainpanel.Tag as string != f.Name)
            {
                if (mainpanel.Controls.Count > 0)
                    mainpanel.Controls.RemoveAt(0);
                f.TopLevel = false;
                f.Dock = DockStyle.Fill;
                mainpanel.Controls.Add(f);
                mainpanel.Tag = f.Name;
                f.Show();
            }
            if (this.mainpanel.Tag as string != "Info")
                Pf = (Form)Form;
        }

        public void Back()
        {
            Loadform(Pf);
        }
        public void DisableInfo()
        {
            Info.Enabled = false;
        }
        private void Menubar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Exit_MouseEnter(object sender, EventArgs e)
        {
            this.Exit.BackColor = ColorTranslator.FromHtml("#404040");
        }
        private void Exit_MouseLeave(object sender, EventArgs e)
        {
            this.Exit.BackColor = ColorTranslator.FromHtml("#181818");
        }
        private void Mini_MouseEnter(object sender, EventArgs e)
        {
            this.Mini.BackColor = ColorTranslator.FromHtml("#404040");
        }
        private void Mini_MouseLeave(object sender, EventArgs e)
        {
            this.Mini.BackColor = ColorTranslator.FromHtml("#181818");
        }
        private void Mini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Info_Click(object sender, EventArgs e)
        {
            if (this.mainpanel.Tag as string != "Info")
                Loadform(new Info());
            else
                Loadform(Pf);
        }

        private void Fade_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.05;
            if (this.Opacity >= 1)
                Fade.Stop();
        }

        private void Taskbar_Paint(object sender, PaintEventArgs e)
        {
            using Bitmap bmp = new(mainpanel.Width, mainpanel.Height);
            mainpanel.DrawToBitmap(bmp, mainpanel.ClientRectangle);
            Rectangle intersection = Rectangle.Intersect(taskbar.Bounds, mainpanel.Bounds);
            Rectangle sourceRect = new(intersection.Left - mainpanel.Left, intersection.Top - mainpanel.Top, intersection.Width, intersection.Height);
            e.Graphics.DrawImage(bmp, 0, 0, sourceRect, GraphicsUnit.Pixel);
            using Brush b = new SolidBrush(Color.FromArgb(100, Color.Black));
            e.Graphics.FillRectangle(b, e.ClipRectangle);
        }

        private void Snowfall_Tick(object sender, EventArgs e)
        {
            overlay.Snowflake(new Point(random.Next(this.ClientSize.Width), 0));
        }
    }
}

public class SnowOverlay : Form
{
    private readonly List<Point> snowflakes = [];
    protected override void SetVisibleCore(bool value)
    {
        if (!IsHandleCreated && value)
        {
            value = false;
            CreateHandle();
        }
        base.SetVisibleCore(value);
    }
    public SnowOverlay(Form parent)
    {
        this.BackColor = Color.Black;
        this.TransparencyKey = Color.Black;
        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.StartPosition = FormStartPosition.Manual;
        this.Size = parent.ClientSize;
        this.Location = parent.PointToScreen(Point.Empty);
        this.Owner = parent;
        parent.Move += (s, e) => this.Location = parent.PointToScreen(Point.Empty);

        Show();
    }

    public void Snowflake(Point location)
    {
        snowflakes.Add(location);

        // Move snowflakes down
        for (int i = 0; i < snowflakes.Count; i++)
            snowflakes[i] = new Point(snowflakes[i].X, snowflakes[i].Y + 5);

        // Remove snowflakes that have fallen below the form
        snowflakes.RemoveAll(s => s.Y > this.ClientSize.Height);

        this.Invalidate(); // Force repaint
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        foreach (var snowflake in snowflakes)
        {
            e.Graphics.FillEllipse(Brushes.White, snowflake.X, snowflake.Y, 5, 5);
        }
    }
}