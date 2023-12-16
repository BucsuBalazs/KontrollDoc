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
    /// <summary>
    /// Az Archives osztály egy ContentPage, amely az Irattárakra vonatkozó funkciók navigálására van.
    /// </summary>
    public partial class Archives : ContentPage
    {
        /// <summary>
        /// Használt adatbázis kontextus
        /// </summary>
        DB dbc;

        /// <summary>
        /// Inicializálja a Archives osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <remarks>
        public Archives(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;
        }
        /// <summary>
        /// Dokumentumot gomb eseménykezelője.
        /// </summary>
        async void GetDoc_Clicked(object sender, EventArgs e)
        {
            // Át navigál a GetDoc lapra,
            await Navigation.PushAsync(new Views.GetDoc(dbc));
        }
        /// <summary>
        /// Irattárat gomb eseménykezelője.
        /// </summary>
        async void GetRow_Clicked(object sender, EventArgs e)
        {
            // Át navigál a Filing lapra,
            await Navigation.PushAsync(new Views.Filing(dbc));
        }
    }
}