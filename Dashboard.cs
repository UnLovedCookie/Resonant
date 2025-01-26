using System.Diagnostics;

using System.Management;
using Microsoft.Win32;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.DRS.Structures;
using NvAPIWrapper.Native.DRS;
using System.Net;
using System.Net.NetworkInformation;

namespace Resonant
{
    public partial class Dashboard : Form
    {
        protected override void SetVisibleCore(bool value)
        {
            if (!IsHandleCreated && value)
            {
                value = false;
                CreateHandle();
            }
            base.SetVisibleCore(value);
        }

#pragma warning disable CS8618, CA2211 // Non-nullable + Non-constant
        public static Dashboard Instance;
        public Thread OptimizationThread = new(Optimize.OptimizeScript);
        double progress;
        double totaloptimizations = 6;
#pragma warning restore CS8618, CA2211 // Non-nullable + Non-constant

        public Dashboard()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BackColor = Color.Transparent;
            Show();

            Instance = this;
            GetOptiCount();
        }

        public void GetOptiCount()
        {
            // Get Graphics Card
            ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController");
            List<string> graphicsCard = [];
            foreach (ManagementObject obj in searcher.Get())
                foreach (PropertyData prop in obj.Properties)
                    if (prop.Name == "Description")
                    {
                        if (prop.Value.ToString()?.Contains("NVIDIA", StringComparison.CurrentCultureIgnoreCase) == true)
                            graphicsCard.Add("NVIDIA");
                        else if (prop.Value.ToString()?.Contains("INTEL", StringComparison.CurrentCultureIgnoreCase) == true)
                            graphicsCard.Add("INTEL");
                        else if (prop.Value.ToString()?.Contains("AMD", StringComparison.CurrentCultureIgnoreCase) == true)
                            graphicsCard.Add("AMD");
                    }
            if (graphicsCard.Contains("NVIDIA"))
                totaloptimizations += 1;
            if (graphicsCard.Contains("INTEL"))
                totaloptimizations += 1;
            if (graphicsCard.Contains("AMD"))
                totaloptimizations += 1;
        }

        public void UpdateProg(string label)
        {
            progress++;
            OptiProgLabel.Text = label;
            if ((int)(100 * ((progress - 1) / totaloptimizations)) <= 100)
                OptiProgress.Value = (int)(100 * ((progress - 1) / totaloptimizations));
            if (OptiProgLabel.Text.Contains("Complete"))
            {
                OptimizeBtn.Text = "Restart";
                OptiProgress.Visible = false;
                OptimizeBtn.Enabled = true;
                OptimizeBtn.Visible = true;
                OptiProgLabel.Top += 50;
            }
        }

        private void OptimizeBtn_Click(object sender, MouseEventArgs e)
        {
            if (OptiProgress.Value < 100)
            {
                OptimizeBtn.Enabled = false;
                OptimizeBtn.Visible = false;
                OptiProgress.Visible = true;
                OptiProgLabel.Visible = true;   

                Thread optimizationThread = new Thread(() =>
                {
                    try
                    {
                        Optimize.OptimizeScript();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred during optimization: {ex.Message}",
                            "Optimization Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        OptimizeBtn.Enabled = true;
                        OptimizeBtn.Visible = true;
                        OptiProgress.Visible = false;
                        OptiProgLabel.Visible = false;
                    }
                });
                optimizationThread.Start();

                Resonant.Instance.DisableInfo();
            }
            else
                StartShutDown("-f -r -t 5");
        }
        private static void StartShutDown(string param)
        {
            ProcessStartInfo proc = new()
            {
                FileName = "cmd",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "/C shutdown " + param
            };
            Process.Start(proc);
        }

        private void Coffee_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new();
            tt.SetToolTip(this.Coffee, "Buy me a coffee!");
        }

        private void Coffee_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/resonantx", UseShellExecute = true });
        }
    }
}