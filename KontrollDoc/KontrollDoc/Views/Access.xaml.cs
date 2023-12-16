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
    /// <summary>
    /// Az Access osztály egy ContentPage, amely a felhasznló jogköréről való információ megosztásra van.
    /// </summary>
    public partial class Access : ContentPage
    {

        /// <summary>
        /// Inicializálja az Access osztály új példányát.
        /// </summary>
        /// <param name="Jogosultsag">A felhasználó jogosultsagi szintje.</param>

        public Access(long Jogosultsag)
        {
            InitializeComponent();

            // Informálja a User-t a Jogosultságáról
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