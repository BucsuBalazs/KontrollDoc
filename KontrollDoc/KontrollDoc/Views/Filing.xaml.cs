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
    public partial class Filing : ContentPage
    {

        List<Dokhelye> doks;
        public Filing(DB dbc)
        {
            InitializeComponent();

            Dokhelye dokhelye = new Dokhelye();

            doks = dokhelye.GetDocHely(dbc);
        }

        private void Irattar_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keres = Irattar_Entry.Text;

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