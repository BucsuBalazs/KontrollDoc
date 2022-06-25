using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    internal class DokTorzs
    {
        private int _Azonosito;         // 1
        private string _Megnevezes;     // 2
        private int _TipusAz;           // 3
        private bool _Inaktiv;          // 4

        public DokTorzs(
            int azon,
            string megnevezes,
            int tipusaz,
            bool inaktiv)
        {
            this._Azonosito = azon;
            this._Megnevezes = megnevezes;
            this._TipusAz = tipusaz;
            this._Inaktiv = inaktiv;
        }

        public int Azonosito { get; set; }
        public string Megnevezes { get; set; }
        public string TipusAz { get; set; }
        public bool Inaktiv { get; set; }
    }
}
