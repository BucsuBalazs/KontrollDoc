using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// Segéd osztály a doktorzs és azon típusaival való munkálatokhoz
    /// </summary>
    public class KapcssDokTorzs
    {
        /// <summary>
        /// A DokTorzs példányok listája.
        /// </summary>
        public List<DokTorzs> dokTorzs = new List<DokTorzs>();

        /// <summary>
        /// A DokTorzsTipus példányok listája.
        /// </summary>
        public List<DokTorzsTipus> dokTorzsTipus = new List<DokTorzsTipus>();

        /// <summary>
        /// Inicializálja a <see cref="KapcssDokTorzs"/> osztály új példányát megadott adatbázis-környezettel.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public KapcssDokTorzs(DB dbc) 
        { 
            DokTorzs dokTorzs = new DokTorzs(dbc);
            this.dokTorzs = dokTorzs.GetDokTorzsTable();

            DokTorzsTipus dokTorzsTipus = new DokTorzsTipus(dbc);
            this.dokTorzsTipus = dokTorzsTipus.GetDokTorzsTipusTable();
        }

        /// <summary>
        /// Lekéri a dokTorzsTipus neveinek a listáját az adatbázisból.
        /// </summary>
        /// <param name="megnevezes">A dokTorzsTipus neve.</param>
        /// <returns>dokTorzsTipus nevek listája.</returns>
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

        /// <summary>
        /// Lekéri a megaott DokTorzsTipushoz tartozó példányok listáját az adatbázisból.
        /// </summary>
        /// <param name="megnevezes">A dokTorzsTipus neve.</param>
        /// <returns> DokTorzsTipushoz tartozó példányok listája</returns>
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
