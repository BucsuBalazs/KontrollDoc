using FIT_Common;
using KontrollDoc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /// <summary>
    /// Az GetDoc osztály egy ContentPage, amely egy adott sorszámú dokumentum fizikai helyét keresi ki és oszt meg információkat.
    /// </summary>
    public partial class GetDoc : ContentPage
    {
        /// <summary>
        /// Használt adatbázis kontextus
        /// </summary>
        DB dbc;

        /// <summary>
        /// Lista a dokumentumok helyéről
        /// </summary>
        List<Dokhelye> doks;

        /// <summary>
        /// dolgozók lista.
        /// </summary>
        List<Dolgozo> dolgozok = new List<Dolgozo>();

        /// <summary>
        /// Inicializálja a GetDoc osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <remarks>
        public GetDoc(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;

            Dokhelye dokhelye= new Dokhelye();

            // beállítjuk a dokumentumok helye listát.
            doks = dokhelye.GetDocHely(dbc);

            // Beállítjuk a dolgozók listát.
            List<SqlParameter> empty = new List<SqlParameter>();
            DataTable dataTable = dbc.GetTableFromSPAB("GetDolgozok", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                Dolgozo dolgozo = new Dolgozo();
                dolgozo.Azonosito = (int)dr["Azonosito"];
                dolgozo.Nev = (string)dr["Nev"];
                dolgozo.Usernev = (string)dr["Usernev"];

                dolgozok.Add(dolgozo);
            }

        }
        /// <summary>
        /// A Sorszam_Entry TextChanged eseménykezelője.
        /// </summary>
        private void Sorszam_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Sorszam = Sorszam_Entry.Text;

            int result;
            if (int.TryParse(Sorszam, out result))
            {
                // kikeressük a kért dokumentumot.
                Dokhelye found = doks.Find(d => d.Dokaz == int.Parse(Sorszam));

                
                if (found != null)
                {
                    // HA talált dokumentumot helyet akkor a helyhez köthető információk megjelenítése.
                    Docfound.IsVisible = true;
                    Irattar1_Label.Text = found.Irattar1;
                    Irattar2_Label.Text = found.Irattar2;
                    Irattar3_Label.Text = found.Irattar3;
                    Egyeb_Label.Text = found.Egyeb;
                    var dolgozo = dolgozok.Find(d => d.Azonosito == found.CRU);
                    Ugyintazo_Label.Text = dolgozo.Nev;
                    Datum_Label.Text = found.CRD.ToString();
                }
                else 
                {
                    Docfound.IsVisible = false;
                }
            }
            else
            {
                Docfound.IsVisible = false;
            }
        }
    }
}