using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DumpPort
{
    public class WinIOLab
    {
        #region 驱动级键盘输入定义
        //键盘输入
        //  public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo); 
        //[DllImport("User32.dll")]
        //public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);
        //private const int KEYEVENTF_KEYUP = 2;

        //winio开始
        public const int KBC_KEY_CMD = 0x64;
        public const int KBC_KEY_DATA = 0x60;
        [DllImport("WinIo32.dll")]
        public static extern bool InitializeWinIo();
        [DllImport("WinIo32.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal,
                    byte bSize);
        [DllImport("WinIo32.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal,
                    byte bSize);
        [DllImport("WinIo32.dll")]
        public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize,
                        IntPtr PhysicalMemoryHandle);
        [DllImport("WinIo32.dll")]
        public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle,
                        byte pbLinAddr);
        [DllImport("WinIo32.dll")]
        public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("WinIo32.dll")]
        public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("WinIo32.dll")]
        public static extern void ShutdownWinIo();
        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);
        public void Initialize()
        {
            if (InitializeWinIo())
            {
                KBCWait4IBE();
            }
        }
        public void Shutdown()
        {
            ShutdownWinIo();
            KBCWait4IBE();
        }
        ///等待键盘缓冲区为空
        public void KBCWait4IBE()
        {
            int dwVal = 0;
            do
            {
                bool flag = GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }
        /// 模拟键盘标按下
        public void KeyDown(Keys vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            //  SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            //KBCWait4IBE();
            //SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            //KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);
        }
        /// 模拟键盘弹出
        public void KeyUp(Keys vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            //SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            //KBCWait4IBE();
            //SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            //KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
        }
        /// 模拟一次按键
        public void KeyDownUp(Keys vKeyCoad)
        {
            Initialize();
            KeyUp(vKeyCoad);
            Thread.Sleep(100);
            KeyDown(vKeyCoad);
            Shutdown();
        }
        /// 模拟鼠标按下
        public void MyMouseDown(int vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((byte)vKeyCoad, 0);
            KBCWait4IBE(); // '等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD3, 1);// '发送写入命令
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);// '写入信息按下键
        }
        /// 模拟鼠标弹出
        public void MyMouseUp(int vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((byte)vKeyCoad, 0);
            KBCWait4IBE(); // '等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD3, 1); //'发送写入命令
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);// '写入信息释放键 
        }

        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        /*鼠标点击的函数*/
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /*设置鼠标的坐标*/
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int dx, int dy);

        /*获取鼠标的坐标*/
        [DllImport("user32.dll")]
        public static extern void GetCursorPos(ref Point lpPoint);
        //winio结束
        #endregion

    }
}
