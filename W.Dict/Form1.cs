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
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey(Dict.ModifierKeys.Alt, Keys.D3);
            Deactivate += Form1_Deactivate;
            HideResult();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            TopLevel = false;
            Hide();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SwitchVisibility();
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
            Player = new WMPLib.WindowsMediaPlayer();
            Player.PlayStateChange += Player_PlayStateChange;
            Player.URL = url;
            Player.controls.play();
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                //Actions on stop
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            SwitchVisibility();
        }

        private void SwitchVisibility()
        {
            if (Visible)
            {
                TopLevel = false;
                Hide();
            }
            else
            {
                Show();
                TopLevel = true;
                textBox1.Focus();
            }
        }
    }
}
