using FIT_Common;
using KontrollDoc.Models;
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
    /// Az Filing osztály egy ContentPage, amely egy adott sorszámú dokumentum fizikai helyét keresi ki és oszt meg információkat.
    /// </summary>
    public partial class Filing : ContentPage
    {

        /// <summary>
        /// Lista a dokumentumok helyéről
        /// </summary>
        List<Dokhelye> doks;
        /// <summary>
        /// Inicializálja a Filing osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <remarks>
        public Filing(DB dbc)
        {
            InitializeComponent();

            Dokhelye dokhelye = new Dokhelye();

            // beállítjuk a dokumentumok helye listát.
            doks = dokhelye.GetDocHely(dbc);
        }
        /// <summary>
        /// A Irattar_Entry TextChanged eseménykezelője.
        /// </summary>
        private void Irattar_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keres = Irattar_Entry.Text;

            // Irattár keresése a felhasználó által megadott adattal
            var szur = doks.FindAll(d =>
                !string.IsNullOrEmpty(d.Irattar1) ||
                !string.IsNullOrEmpty(d.Irattar2) ||
                !string.IsNullOrEmpty(d.Irattar3) ||
                !string.IsNullOrEmpty(d.Egyeb) );

            var found = szur.FindAll(d =>
                (d.Irattar1.Contains(keres)) ||
                (d.Irattar2.Contains(keres)) ||
                (d.Irattar3.Contains(keres)) ||
                (d.Egyeb.Contains(keres)));

            if (found.Count > 0)
            {
                // ha találtunk adatokat akkor azoknak a megjelenítése.
                IrattarList.ItemsSource = found;
                IrattarList.IsVisible = true;

            }
            else 
            {
                IrattarList.ItemsSource = null;
                IrattarList.IsVisible = false;
            }
            if (Irattar_Entry.Text == "") 
            {
                IrattarList.ItemsSource = null;
                IrattarList.IsVisible = false;
            }

        }
    }
}