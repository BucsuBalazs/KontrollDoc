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
    public partial class Access : ContentPage
    {
        public Access(long Jogosultsag)
        {
            InitializeComponent();

            if (Jogosultsag == 1) 
            {
                string show = "Önnek van hozzáférése a Kontroll Dokumentumkezelő rendszeréhez";
                Acces_Label.Text = show;
                Acces.IsVisible= true;
            }
            else 
            {
                string show = "Önnek nincs hozzáférése a Kontroll Dokumentumkezelő rendszeréhez kérem keresse meg munka társunkat.";
                NoAcces_Label.Text = show;
                NoAcces.IsVisible= true; 
            }
        }
    }
}