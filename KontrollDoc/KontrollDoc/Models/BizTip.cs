using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// BizTip táblát reprezentáló osztály
    /// </summary>
    internal class BizTip
    {
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// Megnevezes getter, setter
        /// </summary>
        public string Megnevezes { get; set; }
        /// <summary>
        /// Inicializálja az <see cref="BizTip"/> osztály új példányát.
        /// </summary>
        public BizTip() { }
        /// <summary>
        /// Lekéri az BizTip példányok listáját az adatbázisból.
        /// </summary>
        /// <param name="dbc">Az adatbázis contextus</param>
        /// <returns>Az BizTip példányok listája.</returns>

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
