using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ASSWinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon = this.Icon;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYUP = 0x0101;
        const int VK_SNAPSHOT = 0x2C;



        public delegate int keyboardHookProc(int code, int wParam, ref KBDLLHOOKSTRUCT lParam);


        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);



        private int _hookProc(int code, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (code >= 0)
            {
                if (wParam == WM_KEYUP && lParam.vkCode == VK_SNAPSHOT)
                {
                    if (Clipboard.ContainsImage())
                    {
                        pictureBox1.Image = Clipboard.GetImage();
                    }
                }
                return 0;
            }
            else
                return CallNextHookEx(hhook, code, wParam, ref lParam);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        IntPtr hhook;

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr hInstance = LoadLibrary("User32");
            //Call library hook function
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, hInstance, 0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hhook != IntPtr.Zero)
            {
                //Call library unhook function
                UnhookWindowsHookEx(hhook);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public ulong dwExtraInfo;
    }
}
