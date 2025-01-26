namespace Resonant
{
    partial class Resonant
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Resonant));
            menubar = new Panel();
            CoutXLabel = new Label();
            CoutXLogo = new PictureBox();
            Mini = new PictureBox();
            Exit = new PictureBox();
            mainpanel = new Panel();
            taskbar = new Panel();
            Info = new PictureBox();
            versionID = new Label();
            Fade = new System.Windows.Forms.Timer(components);
            Snowfall = new System.Windows.Forms.Timer(components);
            menubar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CoutXLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Mini).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Exit).BeginInit();
            taskbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Info).BeginInit();
            SuspendLayout();
            // 
            // menubar
            // 
            menubar.BackColor = Color.FromArgb(24, 24, 24);
            menubar.Controls.Add(CoutXLabel);
            menubar.Controls.Add(CoutXLogo);
            menubar.Controls.Add(Mini);
            menubar.Controls.Add(Exit);
            menubar.Dock = DockStyle.Top;
            menubar.ForeColor = SystemColors.ControlText;
            menubar.Location = new Point(0, 0);
            menubar.Name = "menubar";
            menubar.Size = new Size(750, 40);
            menubar.TabIndex = 0;
            menubar.MouseDown += Menubar_MouseDown;
            // 
            // CoutXLabel
            // 
            CoutXLabel.Dock = DockStyle.Left;
            CoutXLabel.Font = new Font("Segoe UI Semibold", 12F);
            CoutXLabel.ForeColor = Color.White;
            CoutXLabel.Location = new Point(40, 0);
            CoutXLabel.Name = "CoutXLabel";
            CoutXLabel.Size = new Size(135, 40);
            CoutXLabel.TabIndex = 4;
            CoutXLabel.Text = "Resonant";
            CoutXLabel.TextAlign = ContentAlignment.MiddleLeft;
            CoutXLabel.MouseDown += Menubar_MouseDown;
            // 
            // CoutXLogo
            // 
            CoutXLogo.BackgroundImageLayout = ImageLayout.Zoom;
            CoutXLogo.Dock = DockStyle.Left;
            CoutXLogo.Image = (Image)resources.GetObject("CoutXLogo.Image");
            CoutXLogo.Location = new Point(0, 0);
            CoutXLogo.Name = "CoutXLogo";
            CoutXLogo.Padding = new Padding(6);
            CoutXLogo.Size = new Size(40, 40);
            CoutXLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            CoutXLogo.TabIndex = 3;
            CoutXLogo.TabStop = false;
            CoutXLogo.WaitOnLoad = true;
            CoutXLogo.MouseDown += Menubar_MouseDown;
            // 
            // Mini
            // 
            Mini.BackgroundImageLayout = ImageLayout.Zoom;
            Mini.Dock = DockStyle.Right;
            Mini.Image = (Image)resources.GetObject("Mini.Image");
            Mini.Location = new Point(670, 0);
            Mini.Name = "Mini";
            Mini.Padding = new Padding(13);
            Mini.Size = new Size(40, 40);
            Mini.SizeMode = PictureBoxSizeMode.StretchImage;
            Mini.TabIndex = 2;
            Mini.TabStop = false;
            Mini.WaitOnLoad = true;
            Mini.Click += Mini_Click;
            Mini.MouseEnter += Mini_MouseEnter;
            Mini.MouseLeave += Mini_MouseLeave;
            // 
            // Exit
            // 
            Exit.BackgroundImageLayout = ImageLayout.Zoom;
            Exit.Dock = DockStyle.Right;
            Exit.Image = (Image)resources.GetObject("Exit.Image");
            Exit.Location = new Point(710, 0);
            Exit.Name = "Exit";
            Exit.Padding = new Padding(13);
            Exit.Size = new Size(40, 40);
            Exit.SizeMode = PictureBoxSizeMode.StretchImage;
            Exit.TabIndex = 1;
            Exit.TabStop = false;
            Exit.WaitOnLoad = true;
            Exit.Click += Exit_Click;
            Exit.MouseEnter += Exit_MouseEnter;
            Exit.MouseLeave += Exit_MouseLeave;
            // 
            // mainpanel
            // 
            mainpanel.BackColor = Color.Transparent;
            mainpanel.BackgroundImage = (Image)resources.GetObject("mainpanel.BackgroundImage");
            mainpanel.BackgroundImageLayout = ImageLayout.Zoom;
            mainpanel.Dock = DockStyle.Fill;
            mainpanel.Location = new Point(0, 40);
            mainpanel.Name = "mainpanel";
            mainpanel.Size = new Size(750, 480);
            mainpanel.TabIndex = 8;
            // 
            // taskbar
            // 
            taskbar.BackColor = Color.Transparent;
            taskbar.Controls.Add(Info);
            taskbar.Controls.Add(versionID);
            taskbar.Dock = DockStyle.Bottom;
            taskbar.Location = new Point(0, 490);
            taskbar.Name = "taskbar";
            taskbar.Size = new Size(750, 30);
            taskbar.TabIndex = 9;
            taskbar.Paint += Taskbar_Paint;
            // 
            // Info
            // 
            Info.Cursor = Cursors.Hand;
            Info.Dock = DockStyle.Left;
            Info.Image = (Image)resources.GetObject("Info.Image");
            Info.Location = new Point(0, 0);
            Info.Name = "Info";
            Info.Padding = new Padding(5);
            Info.Size = new Size(30, 30);
            Info.SizeMode = PictureBoxSizeMode.StretchImage;
            Info.TabIndex = 7;
            Info.TabStop = false;
            Info.Click += Info_Click;
            // 
            // versionID
            // 
            versionID.Dock = DockStyle.Right;
            versionID.ForeColor = Color.White;
            versionID.Location = new Point(588, 0);
            versionID.Name = "versionID";
            versionID.Padding = new Padding(5);
            versionID.Size = new Size(162, 30);
            versionID.TabIndex = 6;
            versionID.Text = "Version 1.0.0";
            versionID.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Fade
            // 
            Fade.Enabled = true;
            Fade.Interval = 1;
            Fade.Tick += Fade_Tick;
            // 
            // Snowfall
            // 
            Snowfall.Interval = 50;
            Snowfall.Tick += Snowfall_Tick;
            // 
            // Resonant
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(18, 18, 18);
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(750, 520);
            Controls.Add(taskbar);
            Controls.Add(mainpanel);
            Controls.Add(menubar);
            DoubleBuffered = true;
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Resonant";
            Opacity = 0D;
            Text = "Resonant";
            menubar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)CoutXLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)Mini).EndInit();
            ((System.ComponentModel.ISupportInitialize)Exit).EndInit();
            taskbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Info).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel menubar;
        private PictureBox Exit;
        private PictureBox Mini;
        private PictureBox CoutXLogo;
        private Label CoutXLabel;
        private Panel mainpanel;
        private Panel taskbar;
        private Label versionID;
        private PictureBox Info;
        private System.Windows.Forms.Timer Fade;
        private System.Windows.Forms.Timer Snowfall;
    }
}
