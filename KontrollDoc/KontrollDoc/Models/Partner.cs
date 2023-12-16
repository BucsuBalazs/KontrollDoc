using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// Partner táblával való munkálatokat segítő osztály
    /// </summary>
    internal class Partner
    {
        /// <summary>
        /// Adatbázis kontextus.
        /// </summary>
        DB dbc;
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// kod getter, setter
        /// </summary>
        public string kod { get; set; }
        /// <summary>
        /// Nev getter, setter
        /// </summary>
        public string Nev { get; set; }

        /// <summary>
        /// Inicializálja az <see cref="Partner"/> osztály új példányát.
        /// </summary>
        public Partner() { }
        /// <summary>
        /// Inicializálja a <see cref="Partner"/> osztály új példányát egy megadott adatbázis környezettel.
        /// </summary>
        /// <param name="dbc">The database context.</param>
        public Partner(DB dbc)
        {
            this.dbc = dbc;
        }

        /// <summary>
        /// Lekéri a partnerek listáját az adatbázisból.
        /// </summary>
        /// <returns>A partnerek listája.</returns>
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
