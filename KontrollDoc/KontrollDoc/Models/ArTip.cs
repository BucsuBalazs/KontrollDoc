using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    internal class ArTip
    {
        public int Azonosito { get; set; }
        public string Megnevezes { get; set; }

        public ArTip() { }

        public List<ArTip> GetArTip(DB dbc)
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<ArTip> ArTips = new List<ArTip>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetArtipList", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                ArTip arTip = new ArTip();
                arTip.Azonosito = (int)dr["Azonosito"];
                arTip.Megnevezes = (string)dr["Megnevezes"];

                ArTips.Add(arTip);
            }

            return ArTips;
        }
    }
}
