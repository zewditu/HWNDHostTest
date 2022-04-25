using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Microsoft.VisualStudio.PlatformUI;

using WinForms = System.Windows.Forms;
namespace HWNDHost
{
    public sealed class HwndSourceAdapter : WinForms.IWin32Window, IDisposable
    {
        //private HwndSource hwnd;
        private bool isDisposed = false;

        /// <summary>
        /// Creates a new Hwnd that wraps the specified WPF visual element
        /// </summary>
        /// <param name="hubServiceProvider">Hub service provider</param>
        /// <param name="frameworkElement">WPF framework element to wrap with the HWND</param>
        internal HwndSourceAdapter(FrameworkElement frameworkElement)
        {
            this.FrameworkElement = frameworkElement;

            // We explicitly set the width and height to 0, as the shell won't resize us
            // properly if we use the default values of 1.
            var parameters = new HwndSourceParameters
            {
                Height = 0,
                Width = 0,
            };

            // By default HwndSourceParameters don't specify the clip flag. If we
            // don't have it, overlapping elements will get drawn into our HWND
            parameters.WindowStyle |= NativeMethods.WS_CLIPSIBLINGS;

            // We seed the view with an initial size of 0 as we don't yet have a size
            // from the shell, and without a size any WPF layout/render that occurs would
            // use the maximum size.
            WindowBaseView baseView = new WindowBaseView()
            {
                Width = 0,
                Height = 0,
                DataContext = new WindowBaseViewModel(),
                Content = this.FrameworkElement,
            };

            // FrameworkElement won't be able to inherit these default settings from VS shell, set them 
            // back manually therefore we don't need to set on each individual control using this wrapper
            TextOptions.SetTextFormattingMode(baseView, TextFormattingMode.Display);
            ImageThemingUtilities.SetThemeScrollBars(baseView, true);
            baseView.UseLayoutRounding = true;

            this.Hwnd = new HwndSource(parameters);

            // We need to set style parameter after the hwnd is created. We do this because
            // during creation of hwnd it adds flags that we do not want.
            int style = NativeMethods.GetWindowLong(this.Hwnd.Handle, NativeMethods.GWL_STYLE);
            style &= ~NativeMethods.WS_BORDER;
            int result = NativeMethods.SetWindowLong(this.Hwnd.Handle, NativeMethods.GWL_STYLE, style);
            Debug.Assert(result != 0, "Failed to call SetWindowLong to set the border style");

            this.Hwnd.RootVisual = baseView;
        }

        public HwndSource Hwnd { get; set; }

        /// <inheritdoc />
        public IntPtr Handle
        {
            get
            {
                return this.Hwnd.Handle;
            }
        }

        /// <summary>
        /// Gets the <see cref="FrameworkElement"/> wrapped by this <see cref="HwndSourceAdapter"/>.
        /// </summary>
        public FrameworkElement FrameworkElement { get; private set; }

        /// <inheritdoc />

        public bool BringToFront()
        {
            return NativeMethods.BringWindowToTop(this.Handle);
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                var documentBaseView = this.Hwnd.RootVisual as WindowBaseView;
                if (documentBaseView != null && documentBaseView.Content is IDisposable disposableContent)
                {
                    disposableContent.Dispose();
                }

                if (documentBaseView != null && documentBaseView.DataContext is IDisposable disposableVM)
                {
                    disposableVM.Dispose();
                }

                this.Hwnd.Dispose();
                this.Hwnd = null;
                this.isDisposed = true;
            }
        }

        private class NativeMethods
        {
            /// <summary>
            /// HWND clip sibling flag
            /// </summary>
            public const int WS_CLIPSIBLINGS = 0x04000000;

            /// <summary>
            /// Window style flag 
            /// </summary>
            public const int GWL_STYLE = -16;

            /// <summary>
            /// HWND border flag
            /// </summary>
            public const int WS_BORDER = 0x00800000;

            /// <summary>
            /// Retrieves information about the specified window. The function also retrieves the value at a specified offset into the extra window memory.
            /// </summary>
            /// <param name="hwnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
            /// <param name="nIndex">
            /// The zero-based offset to the value to be retrieved. 
            /// Valid values are in the range zero through the number of bytes of extra window memory, 
            /// minus the size of an integer.
            /// </param>
            /// <remarks>
            /// More information
            /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms633585%28v=vs.85%29.aspx
            /// </remarks>
            /// <returns>style value as integer.</returns>
            [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed.")]
            [DllImport("user32.dll")]
            [SuppressMessage("Style", "IDE0049:Simplify Names", Justification = "<Pending>")]
            public static extern Int32 GetWindowLong(IntPtr hwnd, int nIndex);

            /// <summary>
            /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
            /// </summary>
            /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
            /// <param name="nIndex">
            /// The zero-based offset to the value to be set. 
            /// Valid values are in the range zero through the number of bytes of extra window memory, 
            /// minus the size of an integer. 
            /// </param>
            /// <param name="dwNewLong">The replacement value.</param>
            /// <remarks>
            /// More information
            /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms633591(v=vs.85).aspx
            /// </remarks>
            /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.</returns>
            [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed.")]
            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BringWindowToTop(IntPtr hwnd);
        }
    }
}
