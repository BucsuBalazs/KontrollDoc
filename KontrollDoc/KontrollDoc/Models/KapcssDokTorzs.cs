using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KontrollDoc.Models
{
    public class KapcssDokTorzs
    {
        public List<DokTorzs> dokTorzs = new List<DokTorzs>();

        public List<DokTorzsTipus> dokTorzsTipus = new List<DokTorzsTipus>();

        public KapcssDokTorzs(DB dbc) 
        { 
            DokTorzs dokTorzs = new DokTorzs(dbc);
            this.dokTorzs = dokTorzs.GetDokTorzsTable();

            DokTorzsTipus dokTorzsTipus = new DokTorzsTipus(dbc);
            this.dokTorzsTipus = dokTorzsTipus.GetDokTorzsTipusTable();
        }

        public List<string> GetDokTorzs(string megnevezes)
        {

            var query = from d in this.dokTorzs
                        where this.dokTorzsTipus
                              .Where(t => t.TipusNev == megnevezes)
                              .Select(t => t.Azonosito)
                              .Contains(d.TipusAz)
                              && d.Inaktiv == false
                        select d.Megnevezes;

            List<string> results = query.ToList();
            return results;
        }
        public List<DokTorzs> GetDokTorzsObjects(string megnevezes) 
        {

            var query = from d in this.dokTorzs
                        where this.dokTorzsTipus
                              .Where(t => t.TipusNev == megnevezes)
                              .Select(t => t.Azonosito)
                              .Contains(d.TipusAz)
                        select d;

            List<DokTorzs> results = query.ToList();
            return results;
        }
    }
}
