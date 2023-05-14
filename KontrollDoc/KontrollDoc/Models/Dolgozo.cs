using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    internal class Dolgozo
    {
        public Dolgozo() { }

        public int Azonosito { get; set; }

        private string _nev = null;
        public string Nev
        {
            get { return _nev; }
            set { _nev = value; }
        }

        private string _usernev = null;

        public string Usernev
        {
            get { return _usernev; }
            set { _usernev = value; }
        }
    }
}
