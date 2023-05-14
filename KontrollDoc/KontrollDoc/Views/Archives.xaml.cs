using FIT_Common;
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
    public partial class Archives : ContentPage
    {
        DB dbc;
        public Archives(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;
        }

        async void GetDoc_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.GetDoc(dbc));
        }

        async void GetRow_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Filing(dbc));
        }
    }
}