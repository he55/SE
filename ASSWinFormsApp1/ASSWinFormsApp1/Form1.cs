using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASSWinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Icon=this.Icon;
        }



        public static readonly int WH_KEYBOARD_LL   = 13;
        public static readonly int WM_KEYDOWN       = 0x100;
        public static readonly int WM_KEYUP         = 0x101;
        public static readonly int WM_SYSKEYDOWN    = 0x104;
        public static readonly int WM_SYSKEYUP      = 0x105;





    public struct KeyboardHookData {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }   

    public delegate int keyboardHookProc(int code, int wParam, ref KeyboardHookData lParam);   
    

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance); 
           
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookData lParam);  

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);    




        protected IntPtr hhook = IntPtr.Zero;

        public virtual void hook(){
            //Get library instance
            IntPtr hInstance = LoadLibrary("User32");
            //Call library hook function
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, hInstance, 0);
        }

        public virtual void unhook(){     
            //Call library unhook function
            UnhookWindowsHookEx(hhook); 
        }

        private int _hookProc(int code, int wParam, ref KeyboardHookData lParam){
            if (code >= 0) {          
                return 0;
            }
            else
                return CallNextHookEx(hhook, code, wParam, ref lParam);
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
