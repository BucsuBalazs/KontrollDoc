using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    internal class Dokhelye
    {
        public Dokhelye() { }

        public int Azonosito { get; set; }
        public int Dokaz { get; set;}

        private string _irattar1 = null;
        public string Irattar1
        {
            get { return _irattar1; }
            set { _irattar1 = value; }
        }

        private string _irattar2 = null;
        public string Irattar2
        {
            get { return _irattar2; }
            set { _irattar2 = value; }
        }
        private string _irattar3 = null;
        public string Irattar3
        {
            get { return _irattar3; }
            set { _irattar3 = value; }
        }
        public int? TovabbitvaAz { get; set;}

        private string _egyeb = null;
        public string Egyeb
        {
            get { return _egyeb; }
            set { _egyeb = value; }
        }
        public int CRU { get; set; }
        public DateTime CRD { get; set; }
        public int LMU { get; set; }
        public DateTime LMD { get; set; }

        public List<Dokhelye> GetDocHely(DB dbc)
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<Dokhelye> doks = new List<Dokhelye>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokIrattarList", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                Dokhelye dokhelye = new Dokhelye();
                dokhelye.Azonosito = (int)dr["Azonosito"];
                dokhelye.Dokaz = (int)dr["DokAZ"];
                dokhelye.Irattar1 = dr["Irattar1"] as string ?? null;
                dokhelye.Irattar2 = dr["Irattar2"] as string ?? null;
                dokhelye.Irattar3 = dr["Irattar3"] as string ?? null;
                dokhelye.TovabbitvaAz = dr["TovabbitvaAz"] is DBNull ? null : (int?)dr["TovabbitvaAz"];
                dokhelye.Egyeb = dr["Egyeb"] as string ?? null;
                dokhelye.CRU = (int)dr["CRU"];
                dokhelye.CRD = (DateTime)dr["CRD"];
                dokhelye.LMU = (int)dr["LMU"];
                dokhelye.LMD = (DateTime)dr["LMD"];

                doks.Add(dokhelye);
            }

            return doks;
        }
    }
}
