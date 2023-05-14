using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    internal class BizTip
    {

        public int Azonosito { get; set; }
        public string Megnevezes { get; set; }

        public BizTip() { }

        public List<BizTip> GetBizTip(DB dbc) 
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<BizTip> bizTips = new List<BizTip>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetBiztip", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                BizTip bizTip = new BizTip();
                bizTip.Azonosito = (int)dr["Azonosito"];
                bizTip.Megnevezes = (string)dr["Megnevezes"];

                bizTips.Add(bizTip);
            }

            return bizTips;
        }
    }
}
