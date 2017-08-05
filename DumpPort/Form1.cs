// ---------------------------------------------------- //
//                      WinIo v3.0                      //
//          Direct Hardware Access Under Windows        //
//           Copyright 1998-2010 Yariv Kaplan           //
//               http://www.internals.com               //
// ---------------------------------------------------- //

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace DumpPort
{
    public unsafe partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(String DllName);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr hModule, String ProcName);

        [DllImport("kernel32")]
        private extern static bool FreeLibrary(IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool InitializeWinIoType();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool GetPortValType(UInt16 PortAddr, UInt32* pPortVal, UInt16 Size);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool SetPortValType(UInt16 PortAddr, UInt32 PortVal, UInt16 Size);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool ShutdownWinIoType();

        IntPtr hMod;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            winio.Initialize(); // 注册
            // Check if this is a 32 bit or 64 bit system
            if (IntPtr.Size == 4)
            {
                hMod = LoadLibrary("WinIo32.dll");

            }
            else if (IntPtr.Size == 8)
            {
                hMod = LoadLibrary("WinIo64.dll");
            }

            if (hMod == IntPtr.Zero)
            {
                MessageBox.Show("Can't find WinIo dll.\nMake sure the WinIo library files are located in the same directory as your executable file.", "DumpPort", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            IntPtr pFunc = GetProcAddress(hMod, "InitializeWinIo");

            if (pFunc != IntPtr.Zero)
            {
                InitializeWinIoType InitializeWinIo = (InitializeWinIoType)Marshal.GetDelegateForFunctionPointer(pFunc, typeof(InitializeWinIoType));
                bool Result = InitializeWinIo();

                if (!Result)
                {
                    MessageBox.Show("Error returned from InitializeWinIo.\nMake sure you are running with administrative privileges and that the WinIo library files are located in the same directory as your executable file.", "DumpPort", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    FreeLibrary(hMod);
                    this.Close();
                }
            }

            btnGetValue_Click(this, null);
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            IntPtr pFunc = GetProcAddress(hMod, "ShutdownWinIo");

            if (pFunc != IntPtr.Zero)
            {
                ShutdownWinIoType ShutdownWinIo = (ShutdownWinIoType)Marshal.GetDelegateForFunctionPointer(pFunc, typeof(ShutdownWinIoType));

                ShutdownWinIo();
                FreeLibrary(hMod);
            }
          
            this.Close();
        }

        private void btnGetValue_Click(object sender, EventArgs e)
        {
            IntPtr pFunc = GetProcAddress(hMod, "GetPortVal");

            if (pFunc != IntPtr.Zero)
            {
                UInt16 PortAddr;
                UInt32 PortVal;

                PortAddr = UInt16.Parse(txtPortAddr.Text, System.Globalization.NumberStyles.HexNumber);

                GetPortValType GetPortVal = (GetPortValType)Marshal.GetDelegateForFunctionPointer(pFunc, typeof(GetPortValType));

                // Call WinIo to get value
                bool Result = GetPortVal(PortAddr, &PortVal, 1);

                if (Result)
                {
                    txtValue.Text = PortVal.ToString("X");
                }
                else
                {
                    MessageBox.Show("Error returned from GetPortVal", "DumpPort", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSetValue_Click(object sender, EventArgs e)
        {
            IntPtr pFunc = GetProcAddress(hMod, "SetPortVal");

            if (pFunc != IntPtr.Zero)
            {
                UInt16 PortAddr;
                UInt32 PortVal;

                PortAddr = UInt16.Parse(txtPortAddr.Text, System.Globalization.NumberStyles.HexNumber);
                PortVal = UInt32.Parse(txtValue.Text, System.Globalization.NumberStyles.HexNumber);

                SetPortValType SetPortVal = (SetPortValType)Marshal.GetDelegateForFunctionPointer(pFunc, typeof(SetPortValType));

                // Call WinIo to set value
                bool Result = SetPortVal(PortAddr, PortVal, 1 );

                if (!Result)
                {
                    MessageBox.Show("Error returned from SetPortVal", "DumpPort", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        WinIOLab winio = new WinIOLab();
        private void button1_Click(object sender, EventArgs e)
        {

            testQQ();


            //winio.Shutdown(); // 用完后注销
        }
        private void testTeamview()
        {
            //textBox1.Focus();
            var teamviewer = win32.FindWindow(null, "TeamViewer");
            win32.ShowWindow(teamviewer, win32.ShowWindowCommands.ShowDefault);
            win32.SetForegroundWindow(teamviewer);
            Thread.Sleep(500);
            IntPtr hNext = IntPtr.Zero;
            //第一个对话框
            var diag1 = win32.FindWindowEx(teamviewer, hNext, "#32770", "");
            var ComboBox = win32.FindWindowEx(diag1, hNext, "ComboBox", "");
            var editId = win32.FindWindowEx(ComboBox, hNext, "Edit", "");
            var btnLink = win32.FindWindowEx(diag1, hNext, "Button", "连接到伙伴");
            win32.SetFocus(editId);
            Send(Keys.D1);
            Send(Keys.D2);
            Send(Keys.D3);
            Send(Keys.D4);
        }
        private void testQQ()
        {
            //textBox1.Focus();
           
            IntPtr hNext = IntPtr.Zero;

            //win32.SetFocus(new IntPtr(0x00020BEA));
            win32.SetForegroundWindow(new IntPtr(0x001505AA));
            SendKeys.Send("1234");
            //Send(Keys.D1);
            //Send(Keys.D2);
            //Send(Keys.D3);
            //Send(Keys.D4);
            //Send(Keys.Tab);
            //Send(Keys.D1);
            //Send(Keys.D2);
            //Send(Keys.D3);
            //Send(Keys.D4);

        }
        private void Send(Keys key)
        {
            winio.KeyDown(key);
            winio.KeyUp(key);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            winio.Shutdown();
        }
    }
}