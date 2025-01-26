namespace Resonant
{
    partial class Dashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dashboard));
            pictureBox2 = new PictureBox();
            OptimizeBtn = new BrbVideoManager.Controls.RoundedButton();
            OptiProgress = new ProgressBar();
            OptiProgLabel = new Label();
            Coffee = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Coffee).BeginInit();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.BackgroundImageLayout = ImageLayout.None;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(47, 98);
            pictureBox2.Margin = new Padding(0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Padding = new Padding(0, 30, 0, 0);
            pictureBox2.Size = new Size(657, 110);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            // 
            // OptimizeBtn
            // 
            OptimizeBtn.BackColor = Color.FromArgb(24, 24, 24);
            OptimizeBtn.BorderColor = Color.FromArgb(24, 24, 24);
            OptimizeBtn.BorderDownColor = Color.FromArgb(24, 24, 24);
            OptimizeBtn.BorderDownWidth = 0F;
            OptimizeBtn.BorderOverColor = Color.FromArgb(24, 24, 24);
            OptimizeBtn.BorderOverWidth = 0F;
            OptimizeBtn.BorderRadius = 50;
            OptimizeBtn.BorderWidth = 0F;
            OptimizeBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(9, 9, 9);
            OptimizeBtn.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            OptimizeBtn.ForeColor = Color.White;
            OptimizeBtn.Location = new Point(241, 264);
            OptimizeBtn.Name = "OptimizeBtn";
            OptimizeBtn.Size = new Size(272, 107);
            OptimizeBtn.TabIndex = 9;
            OptimizeBtn.Text = "Optimize";
            OptimizeBtn.UseCompatibleTextRendering = true;
            OptimizeBtn.UseVisualStyleBackColor = false;
            OptimizeBtn.MouseDown += OptimizeBtn_Click;
            // 
            // OptiProgress
            // 
            OptiProgress.BackColor = Color.FromArgb(255, 255, 192);
            OptiProgress.ForeColor = Color.FromArgb(255, 192, 192);
            OptiProgress.Location = new Point(205, 288);
            OptiProgress.Name = "OptiProgress";
            OptiProgress.Size = new Size(351, 29);
            OptiProgress.TabIndex = 10;
            OptiProgress.Visible = false;
            // 
            // OptiProgLabel
            // 
            OptiProgLabel.Font = new Font("Segoe UI", 16F);
            OptiProgLabel.ForeColor = Color.White;
            OptiProgLabel.Location = new Point(205, 320);
            OptiProgLabel.Name = "OptiProgLabel";
            OptiProgLabel.Size = new Size(351, 44);
            OptiProgLabel.TabIndex = 11;
            OptiProgLabel.Text = "Starting...";
            OptiProgLabel.TextAlign = ContentAlignment.TopCenter;
            OptiProgLabel.Visible = false;
            // 
            // Coffee
            // 
            Coffee.Cursor = Cursors.Hand;
            Coffee.Image = (Image)resources.GetObject("Coffee.Image");
            Coffee.Location = new Point(698, 406);
            Coffee.Name = "Coffee";
            Coffee.Size = new Size(40, 40);
            Coffee.SizeMode = PictureBoxSizeMode.Zoom;
            Coffee.TabIndex = 12;
            Coffee.TabStop = false;
            Coffee.Click += Coffee_Click;
            Coffee.MouseHover += Coffee_MouseHover;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(24, 24, 24);
            ClientSize = new Size(750, 480);
            Controls.Add(OptimizeBtn);
            Controls.Add(Coffee);
            Controls.Add(OptiProgLabel);
            Controls.Add(OptiProgress);
            Controls.Add(pictureBox2);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "Dashboard";
            Text = "dashboard";
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)Coffee).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox2;
        private BrbVideoManager.Controls.RoundedButton OptimizeBtn;
        private ProgressBar OptiProgress;
        private Label OptiProgLabel;
        private PictureBox Coffee;
    }
}