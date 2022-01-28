﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ASSWinFormsApp1
{
    public partial class MainForm : Form
    {
        HookProc hookProc;
        IntPtr hhook;
        Settings settings = Settings.Load();
        SoundPlayer soundPlayer;
        PreviewWindow window1;
        string tmp;
        int idx = 1;

        public MainForm()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;

            textBox1.Text = settings.savePath;
            comboBox1.SelectedIndex = settings.saveExtension;
            comboBox2.SelectedIndex = settings.saveName;
            comboBox3.SelectedIndex = settings.openApp;
            checkBox1.Checked = settings.isShowPreview;
            checkBox2.Checked = settings.isPlaySound;
            checkBox3.Checked = Helper.CheckStartOnBoot();

            soundPlayer = new SoundPlayer(Properties.Resources.Screenshot);
            window1 = new PreviewWindow();
            window1.openAction = OpenApp;
        }


        #region MyRegion

        void OpenApp()
        {
            if (settings.openApp == 0)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"C:\Windows\System32\mspaint.exe",
                    Arguments = $"\"{tmp}\"",
                    UseShellExecute = true
                });
            }

            window1.SetHide();
        }

        void SaveImage()
        {
            if (Clipboard.ContainsImage())
            {
                if (!Directory.Exists(settings.savePath))
                {
                    Directory.CreateDirectory(settings.savePath);
                }

                string ext = "png";
                ImageFormat imageFormat = ImageFormat.Png;
                switch (settings.saveExtension)
                {
                    case 0:
                        imageFormat = ImageFormat.Png;
                        ext = "png";
                        break;
                    case 1:
                        imageFormat = ImageFormat.Jpeg;
                        ext = "jpg";
                        break;
                    case 2:
                        imageFormat = ImageFormat.Bmp;
                        ext = "bmp";
                        break;
                }

                if (settings.saveName == 0)
                {
                    string name = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    tmp = Path.Combine(settings.savePath, $"{name}.{ext}");
                }
                else
                {
                    do
                    {
                        tmp = Path.Combine(settings.savePath, $"{idx}.{ext}");
                        idx++;
                    } while (File.Exists(tmp));
                }

                Image image = Clipboard.GetImage();
                image.Save(tmp, imageFormat);

                if (settings.isPlaySound)
                {
                    soundPlayer.Play();
                }

                if (settings.isShowPreview)
                {
                    window1.SetImage(tmp);
                }
            }
        }

        IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode == HC_ACTION)
            {
                if (((int)wParam == WM_KEYUP || (int)wParam == WM_SYSKEYUP) && lParam.vkCode == VK_SNAPSHOT)
                {
                    SaveImage();
                }
            }
            return CallNextHookEx(hhook, nCode, wParam, ref lParam);
        }

        #endregion


        #region MyRegion

        private void Form1_Load(object sender, EventArgs e)
        {
            hookProc = new HookProc(LowLevelKeyboardProc);
            IntPtr hh = GetModuleHandle(null);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hh, 0);

            window1.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                if (settings.isFirstRun)
                {
                    notifyIcon1.ShowBalloonTip(1000, "", "程序正在后台运行", ToolTipIcon.None);
                    settings.isFirstRun = false;
                }
            }
            else
            {
                UnhookWindowsHookEx(hhook);
                Settings.Save();
            }
        }

        #endregion


        #region MyRegion

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                settings.savePath = textBox1.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            settings.saveExtension = comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            settings.saveName = comboBox2.SelectedIndex;
        }

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            settings.openApp = comboBox3.SelectedIndex;
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            settings.isShowPreview = checkBox1.Checked;
            if (!settings.isShowPreview)
            {
                window1.SetHide();
            }
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            settings.isPlaySound = checkBox2.Checked;
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                Helper.SetStartOnBoot();
            else
                Helper.RemoveStartOnBoot();
        }

        #endregion


        #region MyRegion

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion


        #region MyRegion

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }


        const int HC_ACTION = 0;
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYUP = 0x0101;
        const int WM_SYSKEYUP = 0x0105;
        const int VK_SNAPSHOT = 0x2C;

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, int dwThreadId);

        [DllImport("User32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("User32.dll", SetLastError = false, ExactSpelling = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle([Optional] string lpModuleName);

        #endregion
    }
}
