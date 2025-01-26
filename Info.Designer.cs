namespace Resonant
{
    partial class Info
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Info));
            InfoLabel = new Label();
            InfoText = new Label();
            Back = new PictureBox();
            Discord = new LinkLabel();
            Donate = new LinkLabel();
            Cat = new PictureBox();
            Website = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)Back).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Cat).BeginInit();
            SuspendLayout();
            // 
            // InfoLabel
            // 
            InfoLabel.BackColor = Color.Transparent;
            InfoLabel.Dock = DockStyle.Top;
            InfoLabel.Font = new Font("Segoe UI Semibold", 35F, FontStyle.Bold, GraphicsUnit.Point, 0);
            InfoLabel.ForeColor = Color.White;
            InfoLabel.Location = new Point(0, 0);
            InfoLabel.Name = "InfoLabel";
            InfoLabel.Padding = new Padding(25, 20, 0, 0);
            InfoLabel.Size = new Size(750, 79);
            InfoLabel.TabIndex = 0;
            InfoLabel.Text = "Info";
            InfoLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // InfoText
            // 
            InfoText.BackColor = Color.Transparent;
            InfoText.Dock = DockStyle.Fill;
            InfoText.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            InfoText.ForeColor = Color.White;
            InfoText.Location = new Point(0, 79);
            InfoText.Margin = new Padding(5);
            InfoText.Name = "InfoText";
            InfoText.Padding = new Padding(30, 15, 0, 0);
            InfoText.Size = new Size(750, 401);
            InfoText.TabIndex = 1;
            InfoText.Text = "Resonant is a free optimization\r\nutility for windows.\r\n\r\nCreated by Aiden\r\n-                -\r\n-\r\n\r\nShyWolf • Discord Staff\r\nBry • AMD Configuration";
            // 
            // Back
            // 
            Back.BackColor = Color.Transparent;
            Back.Cursor = Cursors.Hand;
            Back.Image = (Image)resources.GetObject("Back.Image");
            Back.Location = new Point(688, 12);
            Back.Name = "Back";
            Back.Padding = new Padding(10);
            Back.Size = new Size(50, 50);
            Back.SizeMode = PictureBoxSizeMode.StretchImage;
            Back.TabIndex = 2;
            Back.TabStop = false;
            Back.Click += Back_Click;
            // 
            // Discord
            // 
            Discord.ActiveLinkColor = Color.FromArgb(128, 128, 255);
            Discord.AutoSize = true;
            Discord.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            Discord.LinkColor = Color.FromArgb(192, 192, 255);
            Discord.Location = new Point(46, 241);
            Discord.Name = "Discord";
            Discord.Size = new Size(110, 37);
            Discord.TabIndex = 3;
            Discord.TabStop = true;
            Discord.Text = "Discord";
            Discord.LinkClicked += Discord_LinkClicked;
            // 
            // Donate
            // 
            Donate.ActiveLinkColor = Color.FromArgb(128, 128, 255);
            Donate.AutoSize = true;
            Donate.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            Donate.LinkColor = Color.FromArgb(192, 192, 255);
            Donate.Location = new Point(46, 278);
            Donate.Name = "Donate";
            Donate.Size = new Size(219, 37);
            Donate.TabIndex = 4;
            Donate.TabStop = true;
            Donate.Text = "Buy me a coffee!";
            Donate.LinkClicked += Donate_LinkClicked;
            // 
            // Cat
            // 
            Cat.Image = (Image)resources.GetObject("Cat.Image");
            Cat.Location = new Point(433, 28);
            Cat.Name = "Cat";
            Cat.Size = new Size(338, 425);
            Cat.SizeMode = PictureBoxSizeMode.Zoom;
            Cat.TabIndex = 5;
            Cat.TabStop = false;
            // 
            // Website
            // 
            Website.ActiveLinkColor = Color.FromArgb(128, 128, 255);
            Website.AutoSize = true;
            Website.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            Website.LinkColor = Color.FromArgb(192, 192, 255);
            Website.Location = new Point(169, 241);
            Website.Name = "Website";
            Website.Size = new Size(115, 37);
            Website.TabIndex = 6;
            Website.TabStop = true;
            Website.Text = "Website";
            Website.LinkClicked += Website_LinkClicked;
            // 
            // Info
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(24, 24, 24);
            ClientSize = new Size(750, 480);
            Controls.Add(Back);
            Controls.Add(Website);
            Controls.Add(Donate);
            Controls.Add(Discord);
            Controls.Add(Cat);
            Controls.Add(InfoText);
            Controls.Add(InfoLabel);
            DoubleBuffered = true;
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Info";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Info";
            TransparencyKey = Color.White;
            ((System.ComponentModel.ISupportInitialize)Back).EndInit();
            ((System.ComponentModel.ISupportInitialize)Cat).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label InfoLabel;
        private Label InfoText;
        private PictureBox Back;
        private LinkLabel Discord;
        private LinkLabel Donate;
        private PictureBox Cat;
        private LinkLabel Website;
    }
}