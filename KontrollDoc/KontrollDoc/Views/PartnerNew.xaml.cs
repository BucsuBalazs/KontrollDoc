using FIT_Common;
using KontrollDoc.Models;
using System;
using System.Collections.Generic;
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
    /// Egy ContentPage amely az új partner létrehozsához használt eseményeket metódusokat biztosítja.
    /// </summary>
    public partial class PartnerNew : ContentPage
    {
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// Inicializálja a <see cref="PartnerNew"/> osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public PartnerNew(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
        }
        /// <summary>
        /// Az oldal megjelenésekor hívják. Betölti a partnertorzset és megjeleníti a pickerekbe.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Partnertörzsek lekérése.
            PartnerHelyseg helysegek = new PartnerHelyseg();

            // Partnertörzsek betöltése.
            Helyseg_Picker.ItemsSource = helysegek.GetHelysegek(dbc);
            Helyseg_Picker.ItemDisplayBinding = new Binding("Megnevezes");

            BizTip biztipek = new BizTip();
            Bizonylat_Picker.ItemsSource = biztipek.GetBizTip(dbc);
            Bizonylat_Picker.ItemDisplayBinding = new Binding("Megnevezes");

            ArTip ArTipek = new ArTip();
            Artipus_Picker.ItemsSource = ArTipek.GetArTip(dbc);
            Artipus_Picker.ItemDisplayBinding = new Binding("Megnevezes");

            FizMod FizModek = new FizMod();
            Fizetes_Picker.ItemsSource = FizModek.GetFizMod(dbc);
            Fizetes_Picker.ItemDisplayBinding = new Binding("Megnevezes");

        }

        private async void Uj_Clicked(object sender, EventArgs e)
        {
            try
            {
                // új Partner adatainak beállítása
                List<SqlParameter> PartnerUjparams = new List<SqlParameter>();

                SqlParameter Kod = new SqlParameter();
                Kod.ParameterName = "@Kod";
                Kod.Value = Kod_Entry.Text;
                PartnerUjparams.Add(Kod);

                SqlParameter Nev = new SqlParameter();
                Nev.ParameterName = "@Nev";
                Nev.Value = Nev_Entry.Text;
                PartnerUjparams.Add(Nev);

                SqlParameter KNev = new SqlParameter();
                KNev.ParameterName = "@KNev";
                KNev.Value = KNev_Entry.Text;
                PartnerUjparams.Add(KNev);

                SqlParameter HelysegAz = new SqlParameter();
                HelysegAz.ParameterName = "@HelysegAz";
                var helyseg = (PartnerHelyseg)Helyseg_Picker.SelectedItem;
                HelysegAz.Value = helyseg.Azonosito;
                PartnerUjparams.Add(HelysegAz);

                SqlParameter Cim = new SqlParameter();
                Cim.ParameterName = "@Cim";
                Cim.Value = Cim_Entry.Text;
                PartnerUjparams.Add(Cim);

                SqlParameter Tel = new SqlParameter();
                Tel.ParameterName = "@Tel";
                Tel.Value = Tel_Entry.Text;
                PartnerUjparams.Add(Tel);

                SqlParameter Fax = new SqlParameter();
                Fax.ParameterName = "@Fax";
                Fax.Value = FAX_Entry.Text;
                PartnerUjparams.Add(Fax);

                SqlParameter Web = new SqlParameter();
                Web.ParameterName = "@Web";
                Web.Value = Web_Entry.Text;
                PartnerUjparams.Add(Web);

                SqlParameter Email = new SqlParameter();
                Email.ParameterName = "@Email";
                Email.Value = Email_Entry.Text;
                PartnerUjparams.Add(Email);

                SqlParameter BankSzla = new SqlParameter();
                BankSzla.ParameterName = "@BankSzla";
                BankSzla.Value = Bank_Entry.Text;
                PartnerUjparams.Add(BankSzla);

                SqlParameter AdoSzam = new SqlParameter();
                AdoSzam.ParameterName = "@AdoSzam";
                AdoSzam.Value = Ado_Entry.Text;
                PartnerUjparams.Add(AdoSzam);

                SqlParameter JovEng = new SqlParameter();
                JovEng.ParameterName = "@JovEng";
                JovEng.Value = Jov_Entry.Text;
                PartnerUjparams.Add(JovEng);

                SqlParameter HitelKeret = new SqlParameter();
                HitelKeret.ParameterName = "@HitelKeret";
                HitelKeret.Value = int.Parse(Hitel_Entry.Text);
                PartnerUjparams.Add(HitelKeret);

                SqlParameter Megjegyzes = new SqlParameter();
                Megjegyzes.ParameterName = "@Megjegyzes";
                if (Megjegyzes_Entry.Text == null) { Megjegyzes.Value = ""; } else { Megjegyzes.Value = Megjegyzes_Entry.Text; }
                PartnerUjparams.Add(Megjegyzes);

                SqlParameter BizTipAz = new SqlParameter();
                BizTipAz.ParameterName = "@BizTipAz";
                var biztip = (BizTip)Bizonylat_Picker.SelectedItem;
                BizTipAz.Value = biztip.Azonosito;
                PartnerUjparams.Add(BizTipAz);

                SqlParameter FizModAz = new SqlParameter();
                FizModAz.ParameterName = "@FizModAz";
                var fizmod = (FizMod)Fizetes_Picker.SelectedItem;
                FizModAz.Value = fizmod.Azonosito;
                PartnerUjparams.Add(FizModAz);

                SqlParameter FizHat = new SqlParameter();
                FizHat.ParameterName = "@FizHat";
                FizHat.Value = Fizetesi_Hatarido_Entry.Text;
                PartnerUjparams.Add(FizHat);

                SqlParameter ArTipAz = new SqlParameter();
                ArTipAz.ParameterName = "@ArTipAz";
                var artip = (ArTip)Artipus_Picker.SelectedItem;
                ArTipAz.Value = artip.Azonosito;
                PartnerUjparams.Add(ArTipAz);

                SqlParameter Inaktiv = new SqlParameter();
                Inaktiv.ParameterName = "@Inaktiv";
                Inaktiv.Value = Inaktiv_Checkbox.IsChecked;
                PartnerUjparams.Add(Inaktiv);
                // új partner feltöltése.
                dbc.ExecuteSPAB("PartnerUj", PartnerUjparams);

                // Vissza navigálás.
                await Navigation.PopAsync();
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Hiba", "Figyeljen arra hogy minden *-gal jelölt adatot ki kell tölteni! Valamint hogy helyesen töltse ki.", "Ok");
            }
        }
    }
}