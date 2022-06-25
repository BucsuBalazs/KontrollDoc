using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CAVO3;

namespace KontrollDoc
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocsPage : ContentPage
    {

        //public DB dbc = KontrollDoc.MainPage.dbc;
        //long uid = KontrollDoc.MainPage.uid;

        public DocsPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SecondPage());
        }
    }
}