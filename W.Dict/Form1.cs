using System;
using System.Windows.Forms;
using W.YoudaoSDK;
using Newtonsoft.Json;

namespace W.Dict
{
    public partial class Form1 : Form
    {
        KeyboardHook hook = new KeyboardHook();
        private TextTranslateService TranslateService = new TextTranslateService();
        public Form1()
        {
            InitializeComponent();
            Shown += Form1_Shown;
            FormClosed += Form1_FormClosed;
            // register the event that is fired after the key press.
            hook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);

            // register the alt + 3 combination as hot key.
            hook.RegisterHotKey(Dict.ModifierKeys.Alt, Keys.D3);
            Deactivate += Form1_Deactivate;
            HideResult();
            Player = new WMPLib.WindowsMediaPlayer();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            TopMost = false;
            TopLevel = false;
            Hide();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            开机自动启动ToolStripMenuItem.Checked = AutoStartup.GetIfStartUp();
            textBox1.Focus();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            hook.Dispose();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            HideResult();

            if (string.IsNullOrWhiteSpace(textBox1.Text))
                return;
            var query = textBox1.Text.Trim();
            var result = TranslateService.Translate(query);
            ShowResult(result.ToString());
        }

        private bool IsExpanded = true;
        private int ExpandExtraHeight = 178;
        private void ShowResult(string result)
        {
            if (!IsExpanded)
            {
                Height = Height + ExpandExtraHeight;
                IsExpanded = true;
            }

            textBox2.Text = result;
        }

        private void HideResult()
        {
            textBox2.Text = string.Empty;

            if (IsExpanded)
            {
                Height = Height - ExpandExtraHeight;
                IsExpanded = false;
            }
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            SwitchVisibility();
        }

        private void btn_audio_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                return;

            var query = textBox1.Text.Trim();
            var audioUrl = "http://dict.youdao.com/dictvoice?audio=" + query;
            PlayFile(audioUrl);
        }

        WMPLib.WindowsMediaPlayer Player;

        private void PlayFile(string url)
        {
            Player.URL = url;
            Player.controls.play();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            SwitchVisibility();
        }

        private void SwitchVisibility()
        {
            if (Visible)
            {
                TopMost = false;
                TopLevel = false;
                Hide();
            }
            else
            {
                Show();
                TopLevel = true;
                textBox1.Focus();
                TopMost = true;
                Activate();
            }
        }

        private void 开机自动启动ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            AutoStartup.SetStartup(开机自动启动ToolStripMenuItem.Checked);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
