using System;
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

        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
        }

        IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode == HC_ACTION)
            {
                if (((int)wParam == WM_KEYUP || (int)wParam == WM_SYSKEYUP) && lParam.vkCode == VK_SNAPSHOT)
                {
                    if (Clipboard.ContainsImage())
                    {
                        pictureBox1.Image = Clipboard.GetImage();
                    }
                }
            }
            return CallNextHookEx(hhook, nCode, wParam, ref lParam);
        }

        IntPtr hhook;

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr hh = GetModuleHandle(null);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, hh, 0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hhook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hhook);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {

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
