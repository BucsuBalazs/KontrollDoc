using FIT_Common;
using KontrollDoc.Services.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Scan : ContentPage
    {
        DB dbc = null;
        public Scan(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            Scanner_Picker.ItemsSource = await DependencyService.Get<IUWPScanService>().GetScannersAsync();
        }

        private async void Scan_Button_Clicked(object sender, EventArgs e)
        {
            //List<string> scannerers = await DependencyService.Get<IUWPScanService>().GetScannersAsync();
            try {
                var fileResult = await DependencyService.Get<IUWPScanService>().ScanAsync(Scanner_Picker.SelectedItem.ToString(), "JPEG");
                await DisplayAlert("Siker", fileResult.FullPath, "Ok");
            }
            catch (NullReferenceException)
            {
                await DisplayAlert("Error", "Válassz szkennert", "Ok");
            }
            catch (Exception) 
            {
                await DisplayAlert("Error", "Hiba törtnét. Nézze meg hogy működik-e a nyomtató, és hogy megfelelően van-e beállítva.", "Ok");
            }
        }
    }
}