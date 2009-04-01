using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

// Source: http://www.seesharpdot.net/?tag=wh_keyboard_ll

namespace StopCrop
{
    class KeyboardHook
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            KeyboardProcedure lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private KeyboardProcedure keyboardProcedure;
         
        private KeyboardProcedure CurrentKeyboardProcedure
        {
            get
            {
                if (keyboardProcedure == null)
                {
                    keyboardProcedure = HookCallback;
                }
                return keyboardProcedure;
            }
            set { keyboardProcedure = value; }
        }

        private IntPtr keyboardHookId = IntPtr.Zero;
         
        public KeyboardHook()
        {
            keyboardHookId = SetHook(CurrentKeyboardProcedure);
        }
         
        private IntPtr SetHook(KeyboardProcedure proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(currentModule.ModuleName), 0);
            }
        }
         
        private delegate IntPtr KeyboardProcedure(
            int nCode, IntPtr wParam, IntPtr lParam);
         
        public event KeyEventHandler KeyPressDetected;
         
        private void OnKeyPressDetected(object sender, KeyEventArgs args)
        {
            if (KeyPressDetected != null)
            {
                KeyPressDetected(sender, args);
            }
        }
         
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam ==
                (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
         
                Keys key = ((Keys)vkCode);
         
                if (key == Keys.LControlKey ||
                    key == Keys.RControlKey)
                {
                    key = key | Keys.Control;
                }
         
                if (key == Keys.LShiftKey ||
                    key == Keys.RShiftKey)
                {
                    key = key | Keys.Shift;
                }
         
                if (key == Keys.LMenu ||
                    key == Keys.RMenu)
                {
                    key = key | Keys.Alt;
                }
         
               OnKeyPressDetected(null, new KeyEventArgs(key));
            }
            return CallNextHookEx(keyboardHookId, nCode, wParam, lParam);
        }
         
        public void Dispose()
        {
            UnhookWindowsHookEx(keyboardHookId);
        }

    }
}
