using System.Diagnostics;
using System.Security.Policy;

namespace Resonant
{
    public partial class Info : Form
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
        public Info()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BackColor = Color.Transparent;
            Show();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            Resonant.Instance.Back();
        }

        private void Donate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/resonantx", UseShellExecute = true });
        }

        private void Discord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://discord.com/invite/dptDHp9p9k", UseShellExecute = true });
        }

        private void Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://sites.google.com/view/resonantx", UseShellExecute = true });
        }
    }
}