using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    internal class Dokhelye
    {
        private int _Azonosito;
        private int _DokAz;
        private string _Irattar1;
        private string _Irattar2;
        private string _Irattar3;
        private int _TovabbitvaAz;
        private string _Egyeb;
        private int _CRU;
        private DateTime _CRD;
        private int _LMU;
        private DateTime _LMD;

        public Dokhelye(
            int azon,
            int dokaz,
            string irattar1,
            string irattar2,
            string irattar3,
            int tovabbitvaaz,
            string egyeb,
            int cru,
            DateTime crd,
            int lmu,
            DateTime lmd
            ) 
        {
            this._Azonosito = azon;
            this._DokAz = dokaz;
            this._Irattar1 = irattar1;
            this._Irattar2 = irattar2;
            this._Irattar3 = irattar3;
            this._TovabbitvaAz = tovabbitvaaz;
            this._Egyeb = egyeb;
            this._CRU = cru;
            this._CRD = crd;
            this._LMU = lmu;
            this._LMD = lmd;
        }

        public int Azonosito { get; set; }
        public int Dokaz { get; set;}
        public string Irattar1 { get; set;}
        public string Irattar2 { get; set; }
        public string Irattar3 { get; set; }
        public int TovabbitvaAz { get; set;}
        public string Egyeb { get; set;}
        public int CRU { get; set; }
        public DateTime CRD { get; set; }
        public int LMU { get; set; }
        public DateTime LMD { get; set; }
    }
}
