using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    internal class FizMod
    {
        public int Azonosito { get; set; }
        public string Megnevezes { get; set; }

        public FizMod() { }

        public List<FizMod> GetFizMod(DB dbc)
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<FizMod> Fizmods = new List<FizMod>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetFizModList", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                FizMod fizmod = new FizMod();
                fizmod.Azonosito = (int)dr["Azonosito"];
                fizmod.Megnevezes = (string)dr["Megnevezes"];

                Fizmods.Add(fizmod);
            }

            return Fizmods;
        }
    }
}
