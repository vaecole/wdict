using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using W.Dict.YoudaoSDK;

namespace W.Dict
{
    public partial class Form1 : Form
    {
        private bool _isFirstStart = true;
        KeyboardHook hook = new KeyboardHook();
        private TextTranslateService TranslateService = new TextTranslateService();
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        private readonly ILogger<Form1> _logger;
        public Form1(ILogger<Form1> logger)
        {
            _logger = logger;
            _logger.LogDebug(20, "Doing hard work! {Action}", "Mark");
            InitializeComponent();
            Shown += Form1_Shown;
            FormClosed += Form1_FormClosed;
            // register the event that is fired after the key press.
            hook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);

            // register the alt + 3 combination as hot key.
            hook.RegisterHotKey(Dict.ModifierKeys.Alt, Keys.D4);
            Deactivate += Form1_Deactivate;
            HideResult();
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += (s, e) => outputDevice?.Stop();
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            TopMost = false;
            TopLevel = false;
            Hide();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!_isFirstStart)
            {
                开机自动启动ToolStripMenuItem.Checked = AutoStartup.GetIfStartUp();
                textBox1.Focus();
            }
            else
            {
                SwitchVisibility();
            }
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
        private int ExpandExtraHeight = 235;
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
            audioFile = new AudioFileReader(audioUrl);
            outputDevice.Init(audioFile);
            outputDevice.Play();
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
                textBox1.SelectAll();
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

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }
    }
}
