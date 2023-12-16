using FIT_Common;
using KontrollDoc.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    /// <summary>
    /// Egy ContentPage amely Dokumentum adatainak való megjelenítéséért és frissítésért felelős
    /// </summary>
    public partial class PartnerEdit : ContentPage
    {
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// Kiválasztott partner azonosítója.
        /// </summary>
        int selectedItemAzonosito;
        /// <summary>
        /// Inicializálja a <see cref="PartnerEdit"/> osztály új példányát a kiválasztott partner meghatározott sorszámával.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <param name="selectedItemAzonosito">A kiválasztott partner sorszáma.</param>
        public PartnerEdit(DB dbc, int selectedItemAzonosito)
        {
            InitializeComponent();

            this.dbc = dbc;
            this.selectedItemAzonosito = selectedItemAzonosito;
        }
        /// <summary>
        /// Az oldal megjelenésekor hívják. Betölti az partnertörzset és a kivaálsztott partner adatait.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // partnertörzs betöltése.
            PartnerHelyseg helysegek= new PartnerHelyseg();

            List<PartnerHelyseg> helysegekList = helysegek.GetHelysegek(dbc);

            Helyseg_Picker.ItemsSource = helysegekList;
            Helyseg_Picker.ItemDisplayBinding = new Binding("Megnevezes");

            BizTip biztipek = new BizTip();

            List<BizTip> biztipekList = biztipek.GetBizTip(dbc);
            Bizonylat_Picker.ItemsSource = biztipekList;
            Bizonylat_Picker.ItemDisplayBinding = new Binding("Megnevezes");

            ArTip ArTipek = new ArTip();

            List<ArTip> ArTipekList = ArTipek.GetArTip(dbc);
            Artipus_Picker.ItemsSource = ArTipekList;
            Artipus_Picker.ItemDisplayBinding = new Binding("Megnevezes");

            FizMod FizModek = new FizMod();

            List<FizMod> FizModekList = FizModek.GetFizMod(dbc);
            Fizetes_Picker.ItemsSource = FizModekList;
            Fizetes_Picker.ItemDisplayBinding = new Binding("Megnevezes");


            // Partner adatainak lekéréhez beállítások
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@Az";
            sqlParameter.Value = selectedItemAzonosito;
            sqlParameters.Add(sqlParameter);
            // Partner adatainak letöltése.
            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("PartnerAdat", sqlParameters);
            System.Data.DataRow dr = dataTable.Rows[0];

            // Partner adatainak betöltése.
            Kod_Entry.Text = (string)dr["Kod"];
            Nev_Entry.Text = (string)dr["Nev"];
            KNev_Entry.Text = (string)dr["KeresoNev"];
            var helyseg = helysegekList.Find(h => h.Azonosito == (int)dr["HelysegAz"]);
            Helyseg_Picker.SelectedItem = helyseg;
            Cim_Entry.Text = (string)dr["Cim"];
            Tel_Entry.Text = (string)dr["Telefon"];
            FAX_Entry.Text = (string)dr["FAX"];
            Web_Entry.Text = (string)dr["WEB"];
            Email_Entry.Text = (string)dr["EMAIL"];
            Bank_Entry.Text = (string)dr["Bankszla"];
            Ado_Entry.Text = (string)dr["Adoszam"];
            Jov_Entry.Text = (string)dr["JovEng"];
            var Hitel = dr["Hitelkeret"];
            Hitel_Entry.Text = Hitel.ToString();
            Megjegyzes_Entry.Text = (string)dr["Megjegyzes"];
            var biztip = biztipekList.Find(b => b.Azonosito == (int)dr["BizTipAz"]);
            Bizonylat_Picker.SelectedItem = biztip;
            var fizmod = FizModekList.Find(f => f.Azonosito == (int)dr["FizModAz"]);
            Fizetes_Picker.SelectedItem = fizmod;
            var hatar = dr["FizHat"];
            Fizetesi_Hatarido_Entry.Text = hatar.ToString();
            var artip = ArTipekList.Find(a => a.Azonosito == (int)dr["ArTipAz"]);
            Artipus_Picker.SelectedItem = artip;
            Inaktiv_Checkbox.IsChecked = (bool)dr["Inaktiv"];

        }
        /// <summary>
        /// A Frissít gomb eseménykezelője.
        /// </summary>
        private async void Frissit_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Partner frissítéséhez beállítások
                List<SqlParameter> PartnerModositparams = new List<SqlParameter>();

                SqlParameter Az = new SqlParameter();
                Az.ParameterName = "@Az";
                Az.Value = selectedItemAzonosito;
                PartnerModositparams.Add(Az);

                SqlParameter Kod = new SqlParameter();
                Kod.ParameterName = "@Kod";
                Kod.Value = Kod_Entry.Text;
                PartnerModositparams.Add(Kod);

                SqlParameter Nev = new SqlParameter();
                Nev.ParameterName = "@Nev";
                Nev.Value = Nev_Entry.Text;
                PartnerModositparams.Add(Nev);

                SqlParameter KNev = new SqlParameter();
                KNev.ParameterName = "@KNev";
                KNev.Value = KNev_Entry.Text;
                PartnerModositparams.Add(KNev);

                SqlParameter HelysegAz = new SqlParameter();
                HelysegAz.ParameterName = "@HelysegAz";
                var helyseg = (PartnerHelyseg)Helyseg_Picker.SelectedItem;
                HelysegAz.Value = helyseg.Azonosito;
                PartnerModositparams.Add(HelysegAz);

                SqlParameter Cim = new SqlParameter();
                Cim.ParameterName = "@Cim";
                Cim.Value = Cim_Entry.Text;
                PartnerModositparams.Add(Cim);

                SqlParameter Tel = new SqlParameter();
                Tel.ParameterName = "@Tel";
                Tel.Value = Tel_Entry.Text;
                PartnerModositparams.Add(Tel);

                SqlParameter Fax = new SqlParameter();
                Fax.ParameterName = "@Fax";
                Fax.Value = FAX_Entry.Text;
                PartnerModositparams.Add(Fax);

                SqlParameter Web = new SqlParameter();
                Web.ParameterName = "@Web";
                Web.Value = Web_Entry.Text;
                PartnerModositparams.Add(Web);

                SqlParameter Email = new SqlParameter();
                Email.ParameterName = "@Email";
                Email.Value = Email_Entry.Text;
                PartnerModositparams.Add(Email);

                SqlParameter BankSzla = new SqlParameter();
                BankSzla.ParameterName = "@BankSzla";
                BankSzla.Value = Bank_Entry.Text;
                PartnerModositparams.Add(BankSzla);

                SqlParameter AdoSzam = new SqlParameter();
                AdoSzam.ParameterName = "@AdoSzam";
                AdoSzam.Value = Ado_Entry.Text;
                PartnerModositparams.Add(AdoSzam);

                SqlParameter JovEng = new SqlParameter();
                JovEng.ParameterName = "@JovEng";
                JovEng.Value = Jov_Entry.Text;
                PartnerModositparams.Add(JovEng);

                SqlParameter HitelKeret = new SqlParameter();
                HitelKeret.ParameterName = "@HitelKeret";
                HitelKeret.Value = decimal.Parse(Hitel_Entry.Text);
                PartnerModositparams.Add(HitelKeret);

                SqlParameter Megjegyzes = new SqlParameter();
                Megjegyzes.ParameterName = "@Megjegyzes";
                Megjegyzes.Value = Megjegyzes_Entry.Text;
                PartnerModositparams.Add(Megjegyzes);

                SqlParameter BizTipAz = new SqlParameter();
                BizTipAz.ParameterName = "@BizTipAz";
                var biztip = (BizTip)Bizonylat_Picker.SelectedItem;
                BizTipAz.Value = biztip.Azonosito;
                PartnerModositparams.Add(BizTipAz);

                SqlParameter FizModAz = new SqlParameter();
                FizModAz.ParameterName = "@FizModAz";
                var fizmod = (FizMod)Fizetes_Picker.SelectedItem;
                FizModAz.Value = fizmod.Azonosito;
                PartnerModositparams.Add(FizModAz);

                SqlParameter FizHat = new SqlParameter();
                FizHat.ParameterName = "@FizHat";
                FizHat.Value = Fizetesi_Hatarido_Entry.Text;
                PartnerModositparams.Add(FizHat);

                SqlParameter ArTipAz = new SqlParameter();
                ArTipAz.ParameterName = "@ArTipAz";
                var artip = (ArTip)Artipus_Picker.SelectedItem;
                ArTipAz.Value = artip.Azonosito;
                PartnerModositparams.Add(ArTipAz);

                SqlParameter Inaktiv = new SqlParameter();
                Inaktiv.ParameterName = "@Inaktiv";
                Inaktiv.Value = Inaktiv_Checkbox.IsChecked;
                PartnerModositparams.Add(Inaktiv);
                // Partner frissítése.
                dbc.ExecuteSPAB("PartnerModosit", PartnerModositparams);
                // vissza navigálás.
                await Navigation.PopAsync();
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Hiba", "Figyeljen arra hogy minden *-gal jelölt adatot ki kell tölteni! Valamint hogy helyesen töltse ki.", "Ok");
            }
        }
    }
}