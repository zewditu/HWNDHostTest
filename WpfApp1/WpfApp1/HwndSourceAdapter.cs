using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Microsoft.VisualStudio.PlatformUI;

using WinForms = System.Windows.Forms;

namespace WpfApp1
{
    internal class HwndSourceAdapter: WinForms.IWin32Window
    {
    }
}
