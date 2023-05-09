using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Linq;

namespace KontrollDoc.Models
{
    public class DokTorzs
    {
        DB dbc;
        public int Azonosito { get; set; }
        public string Megnevezes { get; set; }
        public int TipusAz { get; set; }
        public bool Inaktiv { get; set; }


        public DokTorzs(DB dbc) 
        {
            this.dbc = dbc;
        }
        public DokTorzs()
        {
        }

        public List<string> GetDokTorzs(string megnevezes)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@Tipus";
            sqlParameter.Value = megnevezes;
            sqlParameters.Add(sqlParameter);

            List<string> Tipus_Picker_List = new List<string>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetDokTorzs", sqlParameters);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                string str = (string)dr["Megnevezes"];
                Tipus_Picker_List.Add(str);
            }
            return Tipus_Picker_List;
        }

        public List<DokTorzs> GetDokTorzsTable() {

            List<SqlParameter> empty = new List<SqlParameter>();

            List<DokTorzs> dokTorzs = new List<DokTorzs>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokTorzsLista", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                DokTorzs dok = new DokTorzs();
                dok.Azonosito = (int)dr["Azonosito"];
                dok.Megnevezes = (string)dr["Megnevezes"];
                dok.TipusAz = (int)dr["TipusAz"];
                dok.Inaktiv = (bool)dr["Inaktiv"];

                dokTorzs.Add(dok);
            }

            return dokTorzs;
        }
    }
}
