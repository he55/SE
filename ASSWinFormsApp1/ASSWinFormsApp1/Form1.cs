using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ASSWinFormsApp1
{
    public partial class Form1 : Form
    {
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


        Settings settings = Settings.Load();
        SoundPlayer soundPlayer;
        Window1 window1 = new Window1();

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;

            textBox1.Text = settings.savePath;
            comboBox1.SelectedIndex = settings.saveExt;
            checkBox1.Checked = settings.isPre;
            checkBox2.Checked = settings.isSou;
            checkBox3.Checked = Helper.CheckStartOnBoot();

            soundPlayer = new SoundPlayer();
            soundPlayer.SoundLocation = "camerashutter.wav";
        }

        void saveImage()
        {
            if (Clipboard.ContainsImage())
            {
                string tmp = Path.GetTempFileName();
                System.Drawing.Image image = Clipboard.GetImage();
                image.Save(tmp, System.Drawing.Imaging.ImageFormat.Png);

                if (settings.isSou)
                {
                    playSou();
                }

                if (settings.isPre)
                {
                    window1.ImagePath = tmp;
                }
            }
        }

        void playSou()
        {
            soundPlayer.Play();
        }

        IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode == HC_ACTION)
            {
                if (((int)wParam == WM_KEYUP || (int)wParam == WM_SYSKEYUP) && lParam.vkCode == VK_SNAPSHOT)
                {
                    saveImage();
                }
            }
            return CallNextHookEx(hhook, nCode, wParam, ref lParam);
        }


        HookProc hookProc;
        IntPtr hhook;

        private void Form1_Load(object sender, EventArgs e)
        {
            hookProc = new HookProc(LowLevelKeyboardProc);
            IntPtr hh = GetModuleHandle(null);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hh, 0);

            window1.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hhook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hhook);
            }

            Settings.Save();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

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
            settings.saveExt = comboBox1.SelectedIndex;
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            settings.isPre = checkBox1.Checked;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            settings.isSou = checkBox2.Checked;
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                Helper.SetStartOnBoot();
            else
                Helper.RemoveStartOnBoot();
        }
    }

    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}
