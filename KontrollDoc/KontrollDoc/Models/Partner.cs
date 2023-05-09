using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    internal class Partner
    {
        DB dbc;
        public int Azonosito { get; set; }
        public string kod { get; set; }
        public string Nev { get; set; }

        public Partner() { }
        public Partner(DB dbc)
        {
            this.dbc = dbc;
        }

        public List<Partner> GetPartnerLista()
        {
            List<Partner> partnerlist = new List<Partner>();

            List<SqlParameter> empty = new List<SqlParameter>();
            System.Data.DataTable Partnerek = dbc.GetTableFromSPAB("PartnerLista", empty);

            foreach (System.Data.DataRow dr in Partnerek.Rows)
            {
                Partner partner = new Partner();
                partner.Azonosito = (int)dr["Azonosito"];
                partner.kod = (string)dr["kod"];
                partner.Nev = (string)dr["Nev"];
                partnerlist.Add(partner);
            }

            return partnerlist;

        }
    }
}
