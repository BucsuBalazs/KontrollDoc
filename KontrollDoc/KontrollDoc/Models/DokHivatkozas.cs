using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    internal class DokHivatkozas
    {
        private int _Azonosito;         // 1
        private string _DokFilenev;     // 2
        private string _DokPath;        // 3
        private bool _Inaktiv;          // 4
        private string _ImageData;      // 5

        public DokHivatkozas(
            int azon,
            string dokfilenev,
            string dokpath,
            bool inaktiv,
            string imagedata

            )
        {
            this._Azonosito = azon;
            this._DokFilenev = dokfilenev;
            this._DokPath = dokpath;
            this._Inaktiv = inaktiv;
            this._ImageData = imagedata;
        }

        public int Azonosito { get; set; }
        public string DokFilenev { get; set; }
        public string DokPath { get; set; }
        public bool Inaktiv { get; set; }
        public string ImageData { get; set; }
    }
}
