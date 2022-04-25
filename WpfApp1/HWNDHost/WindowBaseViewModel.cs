using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.Shell;

namespace HWNDHost
{
    internal class WindowBaseViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowBaseViewModel"/> class.
        /// </summary>
        /// <param name="hubServiceProvider">The hub service provider</param>
        public WindowBaseViewModel()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var localHandler = this.PropertyChanged;
            if (localHandler != null)
            {
                localHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
