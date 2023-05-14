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
    public partial class GetDoc : ContentPage
    {
        DB dbc;

        List<Dokhelye> doks;
        List<Dolgozo> dolgozok = new List<Dolgozo>();
        public GetDoc(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;

            Dokhelye dokhelye= new Dokhelye();

            doks = dokhelye.GetDocHely(dbc);

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

        private void Sorszam_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Sorszam = Sorszam_Entry.Text;

            int result;
            if (int.TryParse(Sorszam, out result))
            {
                Dokhelye found = doks.Find(d => d.Dokaz == int.Parse(Sorszam));

                if (found != null)
                {

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