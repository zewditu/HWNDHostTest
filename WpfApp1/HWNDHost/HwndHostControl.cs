using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
namespace HWNDHost
{
    public class HwndHostControl : HwndHost
    {
        private HwndSourceAdapter sourceAdapter;

        public HwndHostControl(HwndSourceAdapter sourceAdapter)
        {
            this.sourceAdapter = sourceAdapter;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var childHandle = this.sourceAdapter.Handle;

            if (childHandle != IntPtr.Zero)
            {
               int GWL_STYLE = (-16);
               uint WS_CHILD = 0x40000000;

                SetWindowLong(childHandle, GWL_STYLE, WS_CHILD);
                SetParent(childHandle, hwndParent.Handle);
            }

            this.AddLogicalChild(this.sourceAdapter);
            this.sourceAdapter.BringToFront();
            return new HandleRef(this, childHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (this.sourceAdapter != null)
            {
                this.sourceAdapter.Dispose();
                this.sourceAdapter = null;
            }
        }

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }
}

