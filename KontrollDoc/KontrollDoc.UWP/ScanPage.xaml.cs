using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Printers;

using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using Xamarin.Forms;

namespace KontrollDoc.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScanPage : Windows.UI.Xaml.Controls.Page
    {
        public ScanPage()
        {
            this.InitializeComponent();

            //ImageScanner
            //ImageScanner myScanner = ImageScanner.FromIdAsync(deviceId);

            //scann
            //ImageScanner myScanner =  ImageScanner.FromIdAsync(10);
        }

    }
}
