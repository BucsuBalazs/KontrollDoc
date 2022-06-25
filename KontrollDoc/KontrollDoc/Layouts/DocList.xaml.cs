using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Data.SqlClient;
using CAVO3;
using KontrollDoc.Models;

namespace KontrollDoc.Layouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocList : ContentPage
    {
        public DocList()
        {
            InitializeComponent();
        }

        public static DB dbc = KontrollDoc.MainPage.dbc;

        List<Dokumentum> GetDokumentumok()
        {
            List<SqlParameter> empty = new List<SqlParameter>();
            System.Data.DataTable dt = dbc.GetTableFromSPAB("DokumentumLista", empty);

            List<Dokumentum> dokumentumok = new List<Dokumentum>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Dokumentum dokumentum = new Dokumentum();
                dokumentum.Azonosito = (int)row["Azonosito"];
                if (row["FoDokumentum"] != DBNull.Value) { dokumentum.FoDokumentum = (bool)row["FoDokumentum"]; } else { dokumentum.FoDokumentum = null; }
                dokumentum.TipusAz = (int)row["TipusAz"];
                dokumentum.Sorszam = (int)row["Sorszam"];
                if (row["Iktato"] != DBNull.Value) { dokumentum.Iktato = (string)row["Iktato"]; } else { dokumentum.Iktato = null; }
                dokumentum.PartnerAz = (int)row["PartnerAz"];
                dokumentum.TemaAz = (int)row["TemaAz"];
                dokumentum.Targy = (string)row["Targy"];
                dokumentum.HordozoAz = (int)row["HordozoAz"];
                dokumentum.UgyintezoAz = (int)row["UgyintezoAz"];
                dokumentum.Datum = (DateTime)row["Datum"];
                if (row["Hatarido"] != DBNull.Value) { dokumentum.Hatarido = (DateTime)row["Hatarido"]; } else { dokumentum.Hatarido = null; }
                dokumentum.FontossagAz = (int)row["FontossagAz"];
                dokumentum.AllapotAz = (int)row["AllapotAz"];
                dokumentum.ProjecktHivatkozasAz = (int)row["ProjektHivatkozasAz"];
                dokumentum.Megjegyzes = (string)row["Megjegyzes"];
                dokumentum.Telefonszam = (string)row["Telefonszam"];
                if (row["CRU"] != DBNull.Value) { dokumentum.CRU = (int)row["CRU"]; } else { dokumentum.CRU = null; }
                if (row["CRD"] != DBNull.Value) { dokumentum.CRD = (DateTime)row["CRD"]; } else { dokumentum.CRD = null; }
                if (row["LMU"] != DBNull.Value) { dokumentum.LMU = (int)row["LMU"]; } else { dokumentum.LMU = null; }
                if (row["LMD"] != DBNull.Value) { dokumentum.LMD = (DateTime)row["LMD"]; } else { dokumentum.LMD = null; }
                if (row["Bizalmas"] != DBNull.Value) { dokumentum.Bizalmas = (bool)row["Bizalmas"]; } else { dokumentum.Bizalmas = null; }
                if (row["Inaktiv"] != DBNull.Value) { dokumentum.Inaktiv = (bool)row["Inaktiv"]; } else { dokumentum.Inaktiv = null; }
                dokumentumok.Add(dokumentum);
            }
            return dokumentumok;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            List<Dokumentum> dokumentumok = GetDokumentumok();

            DocListView.ItemsSource = dokumentumok;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public static int selectedItemAzonosito = 0;

        async private void DocListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var obj = (Dokumentum)e.Item;
            selectedItemAzonosito = obj.Azonosito;
            await Navigation.PushAsync(new DocDetails());
        }

        async private void New_ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DocDetails());
        }
    }
}