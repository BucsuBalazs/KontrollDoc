using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Data;
using System.Data.SqlClient;
using FIT_Common;
using KontrollDoc.Models;

namespace KontrollDoc.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DokTorzsItem : ContentPage
	{
		DB dbc;
		DokTorzs Tapped = null;
		string DokTipus;

		public DokTorzsItem (DB dbc,string DokTipus)
		{
			InitializeComponent ();

			this.dbc = dbc;
			this.DokTipus= DokTipus;
		}

        public DokTorzsItem(DB dbc, DokTorzs Tapped, string DokTipus)
        {
            InitializeComponent();
            this.dbc = dbc;
			this.Tapped = Tapped;
            this.DokTipus = DokTipus;

			Megnevezes_Entry.Text = Tapped.Megnevezes;
			Hasznalat_Checkbox.IsChecked = Tapped.Inaktiv;

			Submit.Text = "Frissít";
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
			if (Tapped != null)
			{
				List<SqlParameter> DokTorzsModosit = new List<SqlParameter>();


				SqlParameter Azonosito = new SqlParameter();
                Azonosito.ParameterName = "@DokTorzsAz";
                Azonosito.Value = Tapped.Azonosito;
                DokTorzsModosit.Add(Azonosito);

                SqlParameter Megnevezes = new SqlParameter();
                Megnevezes.ParameterName = "@Megnevezes";
                Megnevezes.Value = Megnevezes_Entry.Text;
                DokTorzsModosit.Add(Megnevezes);

                SqlParameter TipusAz = new SqlParameter();
                TipusAz.ParameterName = "@TipusAz";
                TipusAz.Value = Tapped.TipusAz;
                DokTorzsModosit.Add(TipusAz);

                SqlParameter Inaktiv = new SqlParameter();
                Inaktiv.ParameterName = "@Inaktiv";
                Inaktiv.Value = Hasznalat_Checkbox.IsChecked;
                DokTorzsModosit.Add(Inaktiv);

                dbc.ExecuteSPAB("DokTorzsModosit", DokTorzsModosit);

            }
			else 
			{
                KapcssDokTorzs getTipusAz = new KapcssDokTorzs(this.dbc);
                var Tipus = getTipusAz.dokTorzsTipus.Find(d => d.TipusNev == DokTipus);

                DokTorzs newDokTorzs= new DokTorzs();
                newDokTorzs.Megnevezes = Megnevezes_Entry.Text;
                newDokTorzs.TipusAz = Tipus.Azonosito;
                newDokTorzs.Inaktiv = Hasznalat_Checkbox.IsChecked;

                List<SqlParameter> DokTorzsUj = new List<SqlParameter>();


                SqlParameter Megnevezes2 = new SqlParameter();
                Megnevezes2.ParameterName = "@Megnevezes";
                Megnevezes2.Value = newDokTorzs.Megnevezes;
                DokTorzsUj.Add(Megnevezes2);

                SqlParameter TipusAz2 = new SqlParameter();
                TipusAz2.ParameterName = "@TipusAz";
                TipusAz2.Value = newDokTorzs.TipusAz;
                DokTorzsUj.Add(TipusAz2);

                SqlParameter Inaktiv2 = new SqlParameter();
                Inaktiv2.ParameterName = "@Inaktiv";
                Inaktiv2.Value = newDokTorzs.Inaktiv;
                DokTorzsUj.Add(Inaktiv2);

                dbc.ExecuteSPAB("DokTorzsUj", DokTorzsUj);
                DokTorzsUj.Clear();
            }

			await Navigation.PopAsync();
        }
    }
}