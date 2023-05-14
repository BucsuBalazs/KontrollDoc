using KontrollDoc.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

[assembly: Xamarin.Forms.Dependency(typeof(NativeNavigationService))]

namespace KontrollDoc.UWP
{
    public class NativeNavigationService : INativeNavigationService
    {

        public async Task NavigateToScanPageAsync()
        {
            var rootFrame = Window.Current.Content as Frame;
            await rootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                rootFrame.Navigate(typeof(ScanPage));
            }).AsTask();
        }
    }
}
