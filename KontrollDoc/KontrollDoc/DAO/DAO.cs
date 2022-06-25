/** 
 * Code provided by FIT Számítástechnika.
 * 
 * 
 * Kisebb változtatásokra volt szükség a Xamarin Forms kompatibilitás eléréséhez. -Búcsú Balázs
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;

namespace CAVO3

{

    public delegate void Ret();

    public delegate void LogMuvelet(string sql, List<SqlParameter> prms, int? returnValue, string message);



    public class HibaEventArgs : EventArgs

    {

        public string SQL { get; set; }

        public Exception Hiba { get; set; }

        public string Megjegyzes { get; set; }

    }



    public class DB : IDisposable

    {

        System.Security.SecureString pwdAzure;

        SqlConnection cnAzure;

        System.Security.SecureString pwd;

        SqlConnection cn;

        System.Security.SecureString pwdAB;

        SqlConnection cnAB;

        long _ABId;

        long _ABFelhasznaloId;

        long _FelhasznaloId;

        private bool _securePasswordStore;

        private string _alkalmazasKod = "";

        private bool _silent = false;

        private bool _passThrough = false;

        private string _felhasznaloNev;

        private System.Security.SecureString _jelszo;

        private DateTime _kapcsolodasIdeje;

        private bool _azureKapcsolat;

        Properties s;

        /// <summary>

        /// Adatbáziselérő alap objektum

        /// </summary>

        /// <param name="alkalmazasKod">Szöveges kód, amivel elkülönülhetnek az egyes alkalmazások beállításai.</param>

        /// <param name="securePasswordStore">Jelzi, hogy a az adott alkalmazás milyen jelszótárolót használ.</param>

        public DB(string alkalmazasKod, bool securePasswordStore)

        {

            s = new Properties();

            _alkalmazasKod = alkalmazasKod;

            _securePasswordStore = securePasswordStore;

            _azureKapcsolat = AzureEllenorzes();

        }



        public DB(DB forras)

        {

            s = new Properties();

            SetLoginData(forras);

            _azureKapcsolat = AzureEllenorzes();

        }



        public event EventHandler<HibaEventArgs> Hiba;

        protected virtual void OnHiba(HibaEventArgs e)

        {

            Hiba?.Invoke(this, e);

        }



        private bool AzureEllenorzes()

        {

            GetConnectionAzure();

            try

            {

                cnAzure.Open();

                cnAzure.Close();

                return true;

            }

            catch

            {

                if (cnAzure.State != ConnectionState.Closed) cnAzure.Close();

                cnAzure = null;

                return false;

            }

        }



        /// <summary>

        /// Hibauzenetek elnyomása.

        /// </summary>

        public bool Silent

        {

            get { return _silent; }

            set { _silent = value; }

        }



        /// <summary>

        /// Kontroll adatbázisban lévő felhasználó azonosító.

        /// </summary>

        public Int32 ABFelhasznaloId

        {

            get { return Convert.ToInt32(_ABFelhasznaloId); }

        }



        public Int32 ABId { get { return Convert.ToInt32(_ABId); } }



        public string FelhasznaloNev

        {

            get { return _felhasznaloNev; }

        }



        private SqlConnection GetConnection()

        {

            return _azureKapcsolat ? GetConnectionAzure() : GetConnectionKontrollVezerloHelyi();

        }



        private SqlConnection GetConnectionAzure()

        {

            if (cnAzure == null)

            {

                if (pwdAzure == null)

                    pwdAzure = new System.Security.SecureString();

                if (!pwdAzure.IsReadOnly())

                {

                    pwdAzure.AppendChar('o');

                    pwdAzure.AppendChar('4');

                    pwdAzure.AppendChar('H');

                    pwdAzure.AppendChar('_');

                    pwdAzure.AppendChar('d');

                    pwdAzure.AppendChar('C');

                    pwdAzure.AppendChar('8');

                    pwdAzure.AppendChar('/');

                    pwdAzure.MakeReadOnly();

                }

                SqlConnectionStringBuilder cnsb = new SqlConnectionStringBuilder

                {

                    DataSource = "lndb4zzxgj.database.windows.net",

                    InitialCatalog = "KontrollVezerloAzure",

                    PersistSecurityInfo = false,

                    TrustServerCertificate = false,

                    Encrypt = true,

                    ConnectTimeout = 30

                };



                cnAzure = new SqlConnection

                {

                    ConnectionString = cnsb.ConnectionString,

                    Credential = new SqlCredential("KontrollUser", pwdAzure)

                };



            }

            return cnAzure;

        }



        private SqlConnection GetConnectionFrissito()

        {

            GetKontrollFrissitoFromAzure(out string server, out string adatbazis);

            SqlConnectionStringBuilder cnsb = new SqlConnectionStringBuilder

            {

                DataSource = server,

                InitialCatalog = adatbazis,

                PersistSecurityInfo = false,

                TrustServerCertificate = true,

                Encrypt = true,

                ConnectTimeout = 30

            };



            System.Security.SecureString pwd = new System.Security.SecureString();

            if (!pwd.IsReadOnly())

            {

                pwd.AppendChar('o');

                pwd.AppendChar('4');

                pwd.AppendChar('H');

                pwd.AppendChar('_');

                pwd.AppendChar('d');

                pwd.AppendChar('C');

                pwd.AppendChar('8');

                pwd.MakeReadOnly();

            }



            return new SqlConnection

            {

                ConnectionString = cnsb.ConnectionString,

                Credential = new SqlCredential("KontrollUser", pwd)

            };

        }



        private SqlConnection GetConnectionLog()

        {

            if (cnAzure == null)

            {

                if (pwdAzure == null)

                    pwdAzure = new System.Security.SecureString();

                if (!pwdAzure.IsReadOnly())

                {

                    pwdAzure.AppendChar('o');

                    pwdAzure.AppendChar('4');

                    pwdAzure.AppendChar('H');

                    pwdAzure.AppendChar('_');

                    pwdAzure.AppendChar('d');

                    pwdAzure.AppendChar('C');

                    pwdAzure.AppendChar('8');

                    pwdAzure.MakeReadOnly();

                }

                GetKontrollLogFromAzure(out string server, out string adatbazis);

                SqlConnectionStringBuilder cnsb = new SqlConnectionStringBuilder

                {

                    DataSource = server,

                    InitialCatalog = adatbazis,

                    PersistSecurityInfo = false,

                    TrustServerCertificate = false,

                    Encrypt = true,

                    ConnectTimeout = 30

                };



                cnAzure = new SqlConnection

                {

                    ConnectionString = cnsb.ConnectionString,

                    Credential = new SqlCredential("KontrollUser", pwdAzure)

                };



            }

            return cnAzure;

        }



        private SqlConnection GetConnectionKontrollVezerloHelyi()

        {

            if (cn == null)

            {

                if (pwd == null)

                    pwd = new System.Security.SecureString();

                if (!pwd.IsReadOnly())

                {

                    pwd.AppendChar('o');

                    pwd.AppendChar('4');

                    pwd.AppendChar('H');

                    pwd.AppendChar('_');

                    pwd.AppendChar('d');

                    pwd.AppendChar('C');

                    pwd.AppendChar('8');

                    pwd.MakeReadOnly();

                }

                //cn = new SqlConnection("Data Source=sql.fit.hu,11433; Initial Catalog=KontrollVezerlo; PersistSecurityInfo=false", new SqlCredential("KontrollUser", pwd));

                //if (_passThrough)

                //    cn = new SqlConnection("Data Source=sql.fit.hu,11433; Initial Catalog=KontrollVezerlo; PersistSecurityInfo=false; TrustServerCertificate = true; Encrypt = true; Application Name=KontrollVezerlo", new SqlCredential(_felhasznaloNev, _jelszo));

                //else

                string server, adatbazis;

                GetKontrollVezerloFromAzure(out server, out adatbazis);

                if (string.IsNullOrEmpty(server))

                {

                    server = ElerhetoSzerver();

                    if (string.IsNullOrEmpty(server))

                        return null;

                    if (string.IsNullOrEmpty(adatbazis))

                        adatbazis = "KontrollVezerlo";

                    server += ",11433";

                }

                cn = new SqlConnection

                {

                    ConnectionString = new SqlConnectionStringBuilder

                    {

                        DataSource = server,

                        InitialCatalog = adatbazis,

                        PersistSecurityInfo = false,//; user=KontrollUser; password=" + "o4H" + "_" + "dC8" + ";

                        TrustServerCertificate = true,

                        Encrypt = true,

                        ApplicationName = "KontrollVezerlo"

                    }.ConnectionString,

                    Credential = new SqlCredential("KontrollUser", pwd)

                };

            }

            return cn;

        }



        public SqlConnection GetConnAB() { return cnAB; }



        private SqlConnection GetClientConnection(long adatbazisId, long felhasznaloId)

        {

            return _azureKapcsolat ? GetClientConnectionAzure(adatbazisId, FelhasznaloId) : GetClientConnectionKontrollVezerlo(adatbazisId, felhasznaloId);

        }



        private SqlConnection GetClientConnectionAzure(long AdatbazisId, long FelhasznaloId)

        {

            if (AdatbazisId != _ABId || cnAB == null || _kapcsolodasIdeje.Year != DateTime.Now.Year)

            {

                List<SqlParameter> prms = new List<SqlParameter>()

                {

                    new SqlParameter() { ParameterName="@AdatbazisId",SqlDbType=SqlDbType.Int,Value=AdatbazisId },

                    new SqlParameter() { ParameterName="@FelhasznaloId",SqlDbType=SqlDbType.Int,Value=FelhasznaloId },

                    new SqlParameter() { ParameterName="@SzerverNev",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@AdatbazisNev",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@ABFelhasznalo",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@ABJelszo",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@FelhasznaloNev",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output }

                };



                _ExecuteSP(GetConnectionAzure(), "GetAdatbazisAdatok", prms, _FelhasznaloId);



                if (cnAB != null && cnAB.State != ConnectionState.Closed)

                    cnAB.Close();

                if (cnAB != null)

                    cnAB.Dispose();



                string serverSQL = prms[2].Value.ToString();

                if (string.IsNullOrEmpty(serverSQL))

                    return null;



                if (serverSQL.Contains("sql.fit.hu"))

                {

                    string server = ElerhetoSzerver();

                    if (server != "")

                        serverSQL = serverSQL.Replace("sql.fit.hu", server);

                    else

                        return null;

                }



                SqlConnectionStringBuilder cnStrBuilder = new SqlConnectionStringBuilder() { DataSource = serverSQL, InitialCatalog = prms[3].Value.ToString(), TrustServerCertificate = true, Encrypt = true, ApplicationName = "Kontroll2015" + (_alkalmazasKod == "" ? "" : "-" + _alkalmazasKod) };

                cnAB = new SqlConnection(cnStrBuilder.ConnectionString, new SqlCredential(_felhasznaloNev, _jelszo));

                _ABId = AdatbazisId;

                _kapcsolodasIdeje = DateTime.Now;

                DataTable dt = _GetTableFromSQL(cnAB, "SELECT TOP 1 Azonosito FROM Dolgozo WHERE Inaktiv=0 AND USERNev='" + prms[6].Value.ToString() + "' ORDER BY Azonosito");

                if (dt != null)

                    _ABFelhasznaloId = dt.Rows.Count > 0 ? Convert.ToInt64(dt.Rows[0]["Azonosito"]) : 0;

                else

                    _ABFelhasznaloId = 0;

            }

            return _ABFelhasznaloId > 0 ? cnAB : null;

        }



        private SqlConnection GetClientConnectionKontrollVezerlo(long AdatbazisId, long FelhasznaloId)

        {

            if (AdatbazisId != _ABId || cnAB == null || _kapcsolodasIdeje.Year != DateTime.Now.Year)

            {

                List<SqlParameter> prms = new List<SqlParameter>()

                {

                    new SqlParameter() { ParameterName="@AdatbazisId",SqlDbType=SqlDbType.Int,Value=AdatbazisId },

                    new SqlParameter() { ParameterName="@FelhasznaloId",SqlDbType=SqlDbType.Int,Value=FelhasznaloId },

                    new SqlParameter() { ParameterName="@SzerverNev",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@AdatbazisNev",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@ABFelhasznalo",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@ABJelszo",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@FelhasznaloNev",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output }

                };

                _ExecuteSP(GetConnection(), "GetAdatbazisAdatok", prms, _FelhasznaloId);

                if (cnAB != null && cnAB.State != ConnectionState.Closed)

                    cnAB.Close();

                if (cnAB != null)

                    cnAB.Dispose();



                string serverSQL = prms[2].Value.ToString();

                if (string.IsNullOrEmpty(serverSQL))

                    return null;



                if (serverSQL.Contains("sql.fit.hu"))

                {

                    string server = ElerhetoSzerver();

                    if (server != "")

                        serverSQL = serverSQL.Replace("sql.fit.hu", server);

                    else

                        return null;

                }



                SqlConnectionStringBuilder cnStrBuilder = new SqlConnectionStringBuilder() { DataSource = serverSQL, InitialCatalog = prms[3].Value.ToString(), TrustServerCertificate = true, Encrypt = true, ApplicationName = "Kontroll2015" + (_alkalmazasKod == "" ? "" : "-" + _alkalmazasKod) };

                if (pwdAB != null)

                    pwdAB.Dispose();

                pwdAB = new System.Security.SecureString();

                foreach (char c in prms[5].Value.ToString()) pwdAB.AppendChar(c);

                //pwdAB.MakeReadOnly();

                //cnAB = new SqlConnection(cnStrBuilder.ConnectionString, new SqlCredential(prms[4].Value.ToString(), pwdAB));

                if (_passThrough)

                    cnAB = new SqlConnection(cnStrBuilder.ConnectionString, new SqlCredential(_felhasznaloNev, _jelszo));

                else

                    cnAB = new SqlConnection(cnStrBuilder.ConnectionString + "; user=" + prms[4].Value.ToString() + "; password=" + prms[5].Value.ToString());

                _ABId = AdatbazisId;

                _kapcsolodasIdeje = DateTime.Now;

                DataTable dt = _GetTableFromSQL(cnAB, "SELECT TOP 1 Azonosito FROM Dolgozo WHERE Inaktiv=0 AND USERNev='" + prms[6].Value.ToString() + "' ORDER BY Azonosito");

                if (dt != null)

                    _ABFelhasznaloId = dt.Rows.Count > 0 ? Convert.ToInt64(dt.Rows[0]["Azonosito"]) : 0;

                else

                    _ABFelhasznaloId = 0;

            }

            return (_ABFelhasznaloId > 0 ? cnAB : null);

        }



        private string ElerhetoSzerver()

        {

            Ping p = new Ping();

            string server = "";

            try

            {

                if (p.Send("sql.fit.hu").Status == IPStatus.Success)

                    server = "sql.fit.hu";

            }

            catch (Exception e) { }

            if (server == "")

                try

                {

                    if (p.Send("sql2.fit.hu").Status == IPStatus.Success)

                        server = "sql2.fit.hu";

                }

                catch (Exception e) { }
            server = "sql.fit.hu";
            return server;

        }



        private void GetKontrollVezerloFromAzure(out string server, out string adatbazis)

        {

            List<SqlParameter> prms = new List<SqlParameter>

            {

                    new SqlParameter() { ParameterName="@Szerver",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@Adatbazis",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output }

            };



            try

            {

                _ExecuteSP(GetConnectionAzure(), "GetKontrollVezerloEleres", prms, 0);

                server = prms[0].Value.ToString();

                adatbazis = prms[1].Value.ToString();

            }

            catch

            {

                server = s.HelyiKontrollVezerloSzerver;

                adatbazis = s.HelyiKontrollVezerloAdatbazis;

            }

        }



        private void GetKontrollFrissitoFromAzure(out string server, out string adatbazis)

        {

            List<SqlParameter> prms = new List<SqlParameter>

            {

                    new SqlParameter() { ParameterName="@Szerver",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@Adatbazis",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output }

            };



            try

            {

                _ExecuteSP(GetConnectionAzure(), "GetKontrollFrissitoEleres", prms, 0);

                server = prms[0].Value.ToString();

                adatbazis = prms[1].Value.ToString();

                if (s.FrissitoSzerver != server || s.FrissitoAdatbazis != adatbazis)

                {

                    s.FrissitoSzerver = server;

                    s.FrissitoAdatbazis = adatbazis;

                }

            }

            catch

            {

                server = s.FrissitoSzerver;

                adatbazis = s.FrissitoAdatbazis;

            }

        }



        private void GetKontrollLogFromAzure(out string server, out string adatbazis)

        {

            List<SqlParameter> prms = new List<SqlParameter>

            {

                    new SqlParameter() { ParameterName="@Szerver",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output },

                    new SqlParameter() { ParameterName="@Adatbazis",SqlDbType=SqlDbType.NVarChar,Size=200,Direction=ParameterDirection.Output }

            };



            try

            {

                _ExecuteSP(GetConnectionAzure(), "GetKontrollLogEleres", prms, 0);

                server = prms[0].Value.ToString();

                adatbazis = prms[1].Value.ToString();

                if (s.KontrollLogSzerver != server || s.KontrollLogAdatbazis != adatbazis)

                {

                    s.KontrollLogSzerver = server;

                    s.KontrollLogAdatbazis = adatbazis;



                }

            }

            catch

            {

                server = s.KontrollLogSzerver;

                adatbazis = s.KontrollLogAdatbazis;

            }

        }



        private long _ExecuteSQL(SqlConnection cnAkt, string sql)

        {

            long ret = -2;

            if (cnAkt != null)

            {

                SqlCommand cmd = new SqlCommand(sql, cnAkt);

                try

                {

                    while (cnAkt.State != ConnectionState.Closed) System.Threading.Thread.Sleep(1000);

                    cnAkt.Open();

                    ret = cmd.ExecuteNonQuery();

                    cnAkt.Close();

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sql, Megjegyzes = "ExecuteSQL" });

                    if (!_silent) throw;

                    ret = -e.Number;

                }

                finally

                {

                    if (cnAkt.State == ConnectionState.Open) cnAkt.Close();

                }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return ret;

        }



        private long _ExecuteSP(SqlConnection cnAkt, string sp, List<SqlParameter> prms, long felhId)

        {

            long ret = -2;

            if (cnAkt != null)

            {

                SqlCommand cmd = new SqlCommand(sp, cnAkt)

                {

                    CommandType = CommandType.StoredProcedure

                };

                cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.Int));

                cmd.Parameters["@RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());

                if (cmd.Parameters.Contains("@UId")) cmd.Parameters["@UId"].Value = felhId;// _ABFelhasznaloId;

                try

                {

                    while (cmd.Connection.State != ConnectionState.Closed) System.Threading.Thread.Sleep(1000);

                    cmd.Connection.Open();

                    cmd.ExecuteNonQuery();

                    cmd.Connection.Close();

                    //foreach (SqlParameter prm in prms.Where(p => p.Direction.Equals(ParameterDirection.Output) || p.Direction.Equals(ParameterDirection.InputOutput)))

                    //    prm.Value = cmd.Parameters[prm.ParameterName].Value;

                    ret = Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value);

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sp, Megjegyzes = "ExecuteSP" });

                    if (!_silent) throw;

                    ret = -1;//-e.Number;

                }

                finally

                {

                    if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();

                }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return ret;

        }



        private long _ExecuteSP(SqlTransaction tran, string sp, List<SqlParameter> prms, long felhId)

        {

            long ret = -2;

            if (tran != null)

            {

                SqlCommand cmd = new SqlCommand(sp, tran.Connection, tran)

                {

                    CommandType = CommandType.StoredProcedure

                };

                cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.Int));

                cmd.Parameters["@RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());

                if (cmd.Parameters.Contains("@UId")) cmd.Parameters["@UId"].Value = felhId;// _ABFelhasznaloId;



                cmd.ExecuteNonQuery();

                ret = Convert.ToInt64(cmd.Parameters["@RETURN_VALUE"].Value);

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return ret;

        }



        private DataTable _GetTableFromSQL(SqlConnection cnAkt, string sql)

        {

            if (cnAkt != null)

            {

                try

                {

                    DataTable dt = new DataTable();

                    SqlDataAdapter da = new SqlDataAdapter(sql, cnAkt);

                    da.Fill(dt);

                    return dt;

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sql, Megjegyzes = "GetTableFromSQL" });

                    if (!_silent) throw;

                }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return null;

        }



        private DataTable _GetTableFromSQL(SqlConnection cnAkt, string sql, List<SqlParameter> prms, long felhId)

        {

            if (cnAkt != null)

            {

                SqlCommand cmd = new SqlCommand(sql, cnAkt);

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());

                if (cmd.Parameters.Contains("@UId")) cmd.Parameters["@UId"].Value = felhId;// _ABFelhasznaloId;

                try

                {

                    DataTable dt = new DataTable();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(dt);

                    return dt;

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sql, Megjegyzes = "GetTableFromSQL" });

                    if (!_silent) throw;

                }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return null;

        }



        private DataTable _GetTableFromSP(SqlConnection cnAkt, string sp, List<SqlParameter> prms, long felhId)

        {

            if (cnAkt != null)

            {

                SqlCommand cmd = new SqlCommand(sp, cnAkt);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = 3600;//360

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());

                if (cmd.Parameters.Contains("@UId")) cmd.Parameters["@UId"].Value = felhId;// _ABFelhasznaloId;

                try

                {

                    DataTable dt = new DataTable();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(dt);

                    return dt;

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sp, Megjegyzes = "GetTableFromSP" });

                    if (!_silent) throw;

                }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return null;

        }



        private DataSet _GetDataSetFromSP(SqlConnection cnAkt, string sp, List<SqlParameter> prms, long felhId)

        {

            if (cnAkt != null)

            {

                SqlCommand cmd = new SqlCommand(sp, cnAkt);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = 3600;//360

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());

                if (cmd.Parameters.Contains("@UId")) cmd.Parameters["@UId"].Value = felhId;// _ABFelhasznaloId;

                try

                {

                    DataSet ds = new DataSet();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    da.Fill(ds);

                    return ds;

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sp, Megjegyzes = "GetDataSetFromSP" });

                    if (!_silent) throw;

                }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return null;

        }



        private bool _GetXMLFromSP(string fn, SqlConnection cnAkt, string sp, List<SqlParameter> prms)

        {

            bool ret = false;

            if (cnAkt != null)

            {

                SqlCommand cmd = new SqlCommand(sp, cnAkt);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandTimeout = 600;

                cmd.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.Int));

                cmd.Parameters["@RETURN_VALUE"].Direction = ParameterDirection.ReturnValue;

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());

                try

                {

                    while (cnAkt.State != ConnectionState.Closed) System.Threading.Thread.Sleep(1000);

                    cnAkt.Open();

                    XmlReader reader = cmd.ExecuteXmlReader();

                    using (XmlTextWriter writer = new XmlTextWriter(fn, null))

                    {

                        writer.Formatting = Formatting.Indented;

                        writer.WriteNode(reader, false);

                        writer.Close();

                    }

                    reader.Close();

                    ret = true;

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sp, Megjegyzes = "GetXMLFromSP" });

                    if (!_silent) throw;

                }

                finally { if (cnAkt.State == ConnectionState.Open) cnAkt.Close(); }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return ret;

        }



        public void SetAdatbazisId(long id)

        {

            if (_ABId != id)

            {

                if (cnAB != null)

                {

                    if (cnAB.State != ConnectionState.Closed)

                        cnAB.Close();

                    cnAB.Dispose();

                    cnAB = null;

                }

                GetClientConnection(id, _FelhasznaloId);

            }

        }

        public DataTable GetTableFromSP(string sp, List<SqlParameter> prms)

        { return _GetTableFromSP(GetConnection(), sp, prms, _FelhasznaloId); }



        public DataTable GetTableFromSPVezerlo(string sp, List<SqlParameter> prms)

        { return _GetTableFromSP(GetConnectionKontrollVezerloHelyi(), sp, prms, _FelhasznaloId); }

        public DataTable GetTableFromSPFrissito(string sp, List<SqlParameter> prms)

        { return _GetTableFromSP(GetConnectionFrissito(), sp, prms, _FelhasznaloId); }

        public DataTable GetTableFromSPLog(string sp, List<SqlParameter> prms)

        { return _GetTableFromSP(GetConnectionLog(), sp, prms, _FelhasznaloId); }



        public DataTable GetTableFromSP(string sp, List<SqlParameter> prms, long adatbazisId)

        { return _GetTableFromSP(GetClientConnection(adatbazisId, FelhasznaloId), sp, prms, _ABFelhasznaloId); }



        public DataTable GetTableFromSP(string sp, List<SqlParameter> prms, long felhasznaloId, long adatbazisId)

        { return _GetTableFromSP(GetClientConnection(adatbazisId, felhasznaloId), sp, prms, _ABFelhasznaloId); }



        public DataTable GetTableFromSPAB(string sp, List<SqlParameter> prms)

        { return _GetTableFromSP(GetClientConnection(_ABId, FelhasznaloId), sp, prms, _ABFelhasznaloId); }



        public DataSet GetDataSetFromSPAB(string sp, List<SqlParameter> prms)

        { return _GetDataSetFromSP(GetClientConnection(_ABId, FelhasznaloId), sp, prms, _ABFelhasznaloId); }



        public DataTable GetTableFromSQL(string sp)

        { return _GetTableFromSQL(GetConnection(), sp); }



        public DataTable GetTableFromSQL(string sp, long adatbazisId)

        { return _GetTableFromSQL(GetClientConnection(adatbazisId, FelhasznaloId), sp); }



        public DataTable GetTableFromSQL(string sp, long felhasznaloId, long adatbazisId)

        { return _GetTableFromSQL(GetClientConnection(adatbazisId, felhasznaloId), sp); }



        public DataTable GetTableFromSQL(string sp, List<SqlParameter> prms, long felhasznaloId, long adatbazisId)

        { return _GetTableFromSQL(GetClientConnection(adatbazisId, felhasznaloId), sp, prms, _ABFelhasznaloId); }



        public DataTable GetTableFromSQLAB(string sp)

        { return _GetTableFromSQL(GetClientConnection(_ABId, FelhasznaloId), sp); }



        public bool GetXMLFromSPAB(string fn, string sp, List<SqlParameter> prms)

        { return _GetXMLFromSP(fn, GetClientConnection(_ABId, FelhasznaloId), sp, prms); }



        public long ExecuteSP(string sp, List<SqlParameter> prms)

        { return _ExecuteSP(GetConnection(), sp, prms, _FelhasznaloId); }



        public long ExecuteSPVezerlo(string sp, List<SqlParameter> prms)

        { return _ExecuteSP(GetConnectionKontrollVezerloHelyi(), sp, prms, _FelhasznaloId); }

        public long ExecuteSPFrissito(string sp, List<SqlParameter> prms)

        { return _ExecuteSP(GetConnectionFrissito(), sp, prms, _FelhasznaloId); }

        public long ExecuteSPLog(string sp, List<SqlParameter> prms)

        { return _ExecuteSP(GetConnectionLog(), sp, prms, _FelhasznaloId); }



        public long ExecuteSP(string sp, List<SqlParameter> prms, long felhasznaloId, long adatbazisId)

        { return _ExecuteSP(GetClientConnection(adatbazisId, felhasznaloId), sp, prms, _ABFelhasznaloId); }



        public long ExecuteSP(string sp, List<SqlParameter> prms, long adatbazisId)

        { return _ExecuteSP(GetClientConnection(adatbazisId, FelhasznaloId), sp, prms, _ABFelhasznaloId); }



        public long ExecuteSPAB(string sp, List<SqlParameter> prms)

        { return _ExecuteSP(GetClientConnection(_ABId, FelhasznaloId), sp, prms, _ABFelhasznaloId); }

        public long ExecuteSPAB(string sp, List<SqlParameter> prms, SqlTransaction tran)

        { return _ExecuteSP(tran, sp, prms, _ABFelhasznaloId); }



        public long ExecuteSQL(string sql)

        { return _ExecuteSQL(GetConnection(), sql); }



        public long ExecuteSQL(string sql, long felhasznaloId, long adatbazisId)

        { return _ExecuteSQL(GetClientConnection(adatbazisId, felhasznaloId), sql); }



        public long ExecuteSQLAB(string sql)

        { return _ExecuteSQL(GetClientConnection(_ABId, FelhasznaloId), sql); }



        public SqlTransaction BeginTransactionAB()

        {

            SqlConnection cnTran = GetClientConnection(_ABId, FelhasznaloId);

            cnTran.Open();

            return cnTran.BeginTransaction();

        }



        public System.IO.MemoryStream GetMemoryStream(string sp, List<SqlParameter> prms, out string sFileName)

        {

            int bufferSize = 100;

            byte[] data = new byte[bufferSize];

            long retval;

            long startIndex = 0;

            sFileName = "";

            SqlConnection cn = GetClientConnection(_ABId, FelhasznaloId);

            if (cn != null)

            {

                SqlCommand cmd = new SqlCommand(sp, cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

                if (prms != null && prms.Count > 0)

                    cmd.Parameters.AddRange(prms.ToArray());



                try

                {

                    while (cn.State != ConnectionState.Closed) System.Threading.Thread.Sleep(1000);

                    cn.Open();

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();



                    if (dr.Read())

                    {

                        sFileName = dr.GetString(1);

                        startIndex = 0;

                        retval = dr.GetBytes(2, startIndex, data, 0, bufferSize);

                        while (retval == bufferSize)

                        {

                            ms.Write(data, 0, bufferSize);

                            startIndex += bufferSize;

                            retval = dr.GetBytes(2, startIndex, data, 0, bufferSize);

                        }

                        ms.Write(data, 0, (int)retval);

                    }

                    dr.Close();

                    cn.Close();

                    ms.Position = 0;

                    if (sFileName == "")

                        return null;

                    else

                        return ms;

                }

                catch (SqlException e)

                {

                    //if (!_silent) MessageBox.Show(e.Message);

                    OnHiba(new HibaEventArgs { Hiba = e, SQL = sp, Megjegyzes = "GetMemoryStreamFromSP" });

                    if (!_silent) throw;

                }

                finally { if (cn.State != ConnectionState.Closed) cn.Close(); }

            }

            else

                throw new Exception("Sikertelen adatbázis kapcsolódás!");

            return null;

        }



        public bool SetLoginData(DB forras)

        {

            if (forras.Connected)

            {

                _alkalmazasKod = forras._alkalmazasKod;

                _securePasswordStore = forras._securePasswordStore;

                _passThrough = forras._passThrough;

                _felhasznaloNev = forras._felhasznaloNev;

                _jelszo = forras._jelszo;

                _FelhasznaloId = forras.FelhasznaloId;

                _ABId = forras._ABId;

                _ABFelhasznaloId = forras._ABFelhasznaloId;

            }

            else

            {

                _alkalmazasKod = forras._alkalmazasKod;

                _securePasswordStore = forras._securePasswordStore;

                _passThrough = forras._passThrough;

                _felhasznaloNev = forras._felhasznaloNev;

                _jelszo = forras._jelszo;

                _FelhasznaloId = 0;

                _ABId = 0;

                _ABFelhasznaloId = 0;

            }

            return Connected;

        }



        public long Login(string FelhasznaloiNev, string Jelszo)

        {

            _passThrough = false;

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() { ParameterName="@FelhasznaloiNev", SqlDbType=SqlDbType.NVarChar, Size=50, Value=FelhasznaloiNev },

                new SqlParameter() { ParameterName="@Jelszo", SqlDbType=SqlDbType.NVarChar, Size=50, Value=Jelszo },

                new SqlParameter() { ParameterName="@AlkalmazasKod", SqlDbType=SqlDbType.NVarChar, Size=50, Value=_alkalmazasKod }

            };

            _FelhasznaloId = _ExecuteSP(GetConnection(), _securePasswordStore ? "GetFelhasznaloIdUj" : "GetFelhasznaloId", prms, 0);

            return _FelhasznaloId;

        }



        public long LoginPassThrough(string FelhasznaloiNev, String Jelszo)

        {

            _passThrough = true;

            _felhasznaloNev = FelhasznaloiNev;

            using (System.Security.SecureString pwd = new System.Security.SecureString())

            {

                foreach (char c in Jelszo.ToCharArray()) pwd.AppendChar(c);

                pwd.MakeReadOnly();



                _jelszo = pwd.Copy();

                _jelszo.MakeReadOnly();

            }



            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() { ParameterName="@FelhasznaloiNev", SqlDbType=SqlDbType.NVarChar, Size=50, Value=FelhasznaloiNev },

                new SqlParameter() { ParameterName="@Jelszo", SqlDbType=SqlDbType.NVarChar, Size=50, Value=Jelszo },

                new SqlParameter() { ParameterName="@AlkalmazasKod", SqlDbType=SqlDbType.NVarChar, Size=50, Value=_alkalmazasKod }

            };



            if (!_azureKapcsolat) _azureKapcsolat = AzureEllenorzes();



            _FelhasznaloId = _ExecuteSP(GetConnection(), _securePasswordStore ? "GetFelhasznaloIdUj" : "GetFelhasznaloId", prms, 0);

            return _FelhasznaloId;

        }



        public bool Logout()

        {

            _felhasznaloNev = "";

            _jelszo.Dispose();

            _jelszo = new System.Security.SecureString();

            _FelhasznaloId = 0;

            _ABFelhasznaloId = 0;

            _ABId = 0;

            if (cnAB != null)

            {

                if (cnAB.State != ConnectionState.Closed)

                    cnAB.Close();

                cnAB = null;

            }

            if (cn != null)

            {

                if (cn.State != ConnectionState.Closed)

                    cn.Close();

                cn = null;

            }

            return true;

        }



        public long FelhasznaloId { get { return _FelhasznaloId; } }



        public bool Connected { get { return (_FelhasznaloId > 0); } }



        public bool ConnectedAB { get => _FelhasznaloId > 0 && _ABId > 0; }



        public bool ConnectedAzure { get => _azureKapcsolat; }



        public bool SecurePasswordStore { get { return _securePasswordStore; } set { _securePasswordStore = value; } }



        public string GetControlUtolsoErtek(bool Adatbazis, bool Felhasznalo, string ControlNev, string AlapErtek)

        {

            string ret;

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() { ParameterName="@Felhasznalo_ID", SqlDbType=SqlDbType.Int, Value=Felhasznalo ? _FelhasznaloId : 0 },

                new SqlParameter() { ParameterName="@Adatbazis_ID", SqlDbType=SqlDbType.Int, Value=Adatbazis ? _ABId : 0 },

                new SqlParameter() { ParameterName="@Control_Nev", SqlDbType=SqlDbType.NVarChar, Size=50, Value=ControlNev },

                new SqlParameter() { ParameterName="@Ertek", SqlDbType=SqlDbType.NVarChar, Size=50, Direction=ParameterDirection.Output },

                new SqlParameter() { ParameterName="@UserId", SqlDbType=SqlDbType.Int, Value=_FelhasznaloId},

                new SqlParameter() { ParameterName="@Alkalmazas_Kod", SqlDbType=SqlDbType.NVarChar, Size=50, Value=_alkalmazasKod }

            };

            if (_ExecuteSP(GetConnection(), "GetControlUtolsoErtek", prms, _FelhasznaloId) == 0)

                ret = AlapErtek;

            else

                ret = prms[3].Value.ToString();

            return ret;

        }



        public long GetControlUtolsoErtekLong(bool Adatbazis, bool Felhasznalo, string ControlNev, string AlapErtek)

        {

            string ret = GetControlUtolsoErtek(Adatbazis, Felhasznalo, ControlNev, AlapErtek);

            long lret;

            try { lret = Convert.ToInt64(ret); } catch { lret = 0; }

            return lret;

        }



        public void SetControlUtolsoErtek(bool Adatbazis, bool Felhasznalo, string ControlNev, string Ertek)

        {

            if (_ABId > 0 && _ABFelhasznaloId == 0)

                GetClientConnection(_ABId, FelhasznaloId);

            if (_ABFelhasznaloId > 0 && _ABId > 0)

            {

                List<SqlParameter> prms = new List<SqlParameter>()

                    {

                        new SqlParameter() { ParameterName="@Felhasznalo_ID", SqlDbType=SqlDbType.Int, Value=Felhasznalo ? _FelhasznaloId : 0 },

                        new SqlParameter() { ParameterName="@Adatbazis_ID", SqlDbType=SqlDbType.Int, Value=Adatbazis ? _ABId : 0 },

                        new SqlParameter() { ParameterName="@Control_Nev", SqlDbType=SqlDbType.NVarChar, Size=50, Value=ControlNev },

                        new SqlParameter() { ParameterName="@Ertek", SqlDbType=SqlDbType.NVarChar, Size=50, Value=Ertek },

                        new SqlParameter() { ParameterName="@UserId", SqlDbType=SqlDbType.Int, Value=_FelhasznaloId },

                        new SqlParameter() { ParameterName="@Alkalmazas_Kod", SqlDbType=SqlDbType.NVarChar, Size=50, Value=_alkalmazasKod }

                    };

                _ExecuteSP(GetConnection(), "SetControlUtolsoErtek", prms, _FelhasznaloId);

            }

        }



        public string GetTeljesNev()

        {

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() {ParameterName="@FelhasznaloId",SqlDbType=SqlDbType.Int,Value=_FelhasznaloId },

                new SqlParameter() {ParameterName="@TeljesNev",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output }

            };

            _ExecuteSP(GetConnection(), "GetTeljesNev", prms, _FelhasznaloId);

            return prms[1].Value.ToString();

        }



        public string GetAdatbazisNev()

        {

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() {ParameterName="@AdatbazisId",SqlDbType=SqlDbType.Int,Value=_ABId },

                new SqlParameter() {ParameterName="@FelhasznaloId",SqlDbType=SqlDbType.Int,Value=_FelhasznaloId },

                new SqlParameter() {ParameterName="@AdatbazisNev",SqlDbType=SqlDbType.NVarChar,Size=50,Direction=ParameterDirection.Output }

            };

            _ExecuteSP(GetConnection(), "GetAdatbazisNev", prms, _FelhasznaloId);

            return prms[2].Value.ToString();

        }



        public bool JogosultAB(Int32 jogAz)

        {

            if (_ABFelhasznaloId == 0 && GetClientConnection(_ABId, _FelhasznaloId) == null)

                return false;

            return ExecuteSPAB("GetJogFelh"

                , new List<SqlParameter>()

                {

                    new SqlParameter() { ParameterName = "@JogAz", SqlDbType = SqlDbType.Int, Value = jogAz }

                    ,new SqlParameter() { ParameterName = "@FelhAz", SqlDbType = SqlDbType.Int, Value = _ABFelhasznaloId }

                }

                ) > 0;

        }



        public DataTable GetMenupontok() { return GetMenupontok(0); }

        public DataTable GetMenupontok(long SzuloMenuId)

        {

            return _GetTableFromSP(GetConnection()

                        , "GetMenupontok"

                        , new List<SqlParameter>()

                        {

                            new SqlParameter() {ParameterName="@FelhasznaloId",SqlDbType=SqlDbType.Int,Value=_FelhasznaloId },

                            new SqlParameter() {ParameterName="@SzuloMenuId",SqlDbType=SqlDbType.Int,Value=SzuloMenuId }

                        }

                       , _FelhasznaloId

                        );

        }



        public DataTable GetAdatbazisok()

        {

            return _GetTableFromSP(GetConnection()

                    , "GetAdatbazisLista"

                    , new List<SqlParameter>()

                    {

                        new SqlParameter() { ParameterName = "FelhasznaloId", SqlDbType = SqlDbType.Int, Value = _FelhasznaloId }

                    }

                    , _FelhasznaloId

                    );

        }



        public void Dispose()

        {

            pwd?.Dispose();

            pwdAB?.Dispose();

            cn?.Dispose();

            cnAB?.Dispose();

            _jelszo?.Dispose();

        }



        #region Kiegészítő függvények...

        public class lbItem

        {

            public Int32 Id { get; set; }

            public string Text { get; set; }

        }

        /*

        public void FillCombo(ComboBox cb, string SQL, string azonosito, string megnevezes)

        {

            cb.DataSource = GetTableFromSQLAB(SQL);

            cb.ValueMember = azonosito;

            cb.DisplayMember = megnevezes;

        }

 

        public void FillList(ListBox lb, string SQL, string azonosito, string megnevezes)

        {

            foreach (DataRow dr in GetTableFromSQLAB(SQL).Rows)

            {

                lb.Items.Add(new lbItem() { Id = Convert.ToInt32(dr[azonosito]), Text = dr[megnevezes].ToString() });

            }

            lb.ValueMember = "Id";

            lb.DisplayMember = "Text";

        }

 

        public void FillCombo(ComboBox cb, string SQL, string azonosito, string megnevezes, string UresSorSzoveg)

        {

            DataTable dt = GetTableFromSQLAB(SQL);

            if (dt != null)

            {

                DataRow dr = dt.NewRow();

                dr[azonosito] = 0;

                dr[megnevezes] = UresSorSzoveg;

                dt.Rows.Add(dr);

                cb.DataSource = dt;

                cb.ValueMember = azonosito;

                cb.DisplayMember = megnevezes;

                cb.SelectedValue = 0;

            }

            else

            {

                cb.DataSource = null;

                cb.Items.Add(new lbItem() { Id = 0, Text = UresSorSzoveg });

                cb.ValueMember = "Id";

                cb.DisplayMember = "Text";

                cb.Text = UresSorSzoveg;

            }

        }

 

        public void FillCombo(ComboBox cb, string SP, List<SqlParameter> prms, string azonosito, string megnevezes)

        {

            cb.DataSource = GetTableFromSPAB(SP, prms);

            cb.ValueMember = azonosito;

            cb.DisplayMember = megnevezes;

        }

 

        public void FillCombo(ComboBox cb, string SP, List<SqlParameter> prms, string azonosito, string megnevezes, string UresSorSzoveg)

        {

            DataTable dt = GetTableFromSPAB(SP, prms);

            if (dt != null)

            {

                DataRow dr = dt.NewRow();

                dr[azonosito] = 0;

                dr[megnevezes] = UresSorSzoveg;

                dt.Rows.Add(dr);

                cb.DataSource = dt;

                cb.ValueMember = azonosito;

                cb.DisplayMember = megnevezes;

                cb.SelectedValue = 0;

            }

            else

            {

                cb.DataSource = null;

                cb.Items.Add(new lbItem() { Id = 0, Text = UresSorSzoveg });

                cb.ValueMember = "Id";

               cb.DisplayMember = "Text";

                cb.Text = UresSorSzoveg;

            }

        }

 

        public void FillList(ListBox lb, string SP, List<SqlParameter> prms, string azonosito, string megnevezes)

        {

            foreach (DataRow dr in GetTableFromSPAB(SP, prms).Rows)

            {

                lb.Items.Add(new lbItem() { Id = Convert.ToInt32(dr[azonosito]), Text = dr[megnevezes].ToString() });

            }

            lb.ValueMember = "Id";

            lb.DisplayMember = "Text";

        }

 

        public void FillCheckedList(CheckedListBox clb, string SP, List<SqlParameter> prms, string azonosito, string megnevezes, string kivalasztva)

        {

            clb.ValueMember = "Id";

            clb.DisplayMember = "Text";

            Int32 newIndex;

            foreach (DataRow dr in GetTableFromSPAB(SP, prms).Rows)

            {

                newIndex = clb.Items.Add(new lbItem() { Id = Convert.ToInt32(dr[azonosito]), Text = dr[megnevezes].ToString() });

                if (kivalasztva!="" && Convert.ToBoolean(dr[kivalasztva])) clb.SetItemChecked(newIndex, true);

            }

       }

 

        public void FillComboBoxColumn(DataGridViewComboBoxColumn col, string SP, List<SqlParameter> prms, string azonosito, string megnevezes)

        {

            foreach (DataRow dr in GetTableFromSPAB(SP, prms).Rows)

            {

                col.Items.Add(new lbItem() { Id = Convert.ToInt32(dr[azonosito]), Text = dr[megnevezes].ToString() });

            }

            col.ValueMember = "Id";

            col.DisplayMember = "Text";

        }

 

        public void SetControlValue(TextBox tb, SqlParameter p)

        {

            if (tb != null) tb.Text = (p.Value == DBNull.Value ? "" : p.Value.ToString());

        }

 

        public void SetControlValue(Label lb, SqlParameter p)

        {

            if (lb != null) lb.Text = (p.Value == DBNull.Value ? "" : p.Value.ToString());

        }

 

        public void SetControlValue(NumericUpDown nud, SqlParameter p)

        {

            if (nud != null) nud.Value = (p.Value == DBNull.Value ? 0 : Convert.ToInt32(p.Value));

        }

 

        public void SetControlValue(ComboBox cb, SqlParameter p)

        {

            if (cb != null)

                cb.SelectedValue = (p.Value == DBNull.Value ? 0 : p.Value);

        }

 

        public void SetControlText(ComboBox cb, SqlParameter p)

        {

            if (cb != null)

                cb.Text = (p.Value == DBNull.Value ? "" : p.Value.ToString());

        }

 

        public void SetControlValue(DateTimePicker dtp, SqlParameter p)

        {

            if (dtp != null)

            {

                if (p.Value != DBNull.Value)

                {

                    dtp.Value = Convert.ToDateTime(p.Value);

                    if (dtp.ShowCheckBox) dtp.Checked = true;

                }

                else if (dtp.ShowCheckBox)

                {

                    dtp.Value = DateTime.Now.Date;

                    dtp.Checked = false;

                }

            }

        }

 

        /// <summary>

        /// DataGridView látható oszlopainak szélességét tárolja.

        /// </summary>

        /// <param name="dgv">A DataGridView objektum</param>

        /// <param name="adatbazisra">Ha igaz, akkor adatbázisonként tárolja az értékeket</param>

        /// <param name="felhasznalora">Ha igaz, akkor felhasználónként tárolja az értékeket</param>

        /// <param name="prefix">szöveges előtag, amivel azonosítható a DataGridView</param>

        public void GridSaveColumnWidth(DataGridView dgv, bool adatbazisra, bool felhasznalora, string prefix)

        {

            foreach (DataGridViewColumn c in dgv.Columns)

            {

                if (c.Visible) SetControlUtolsoErtek(adatbazisra, felhasznalora, prefix + c.Name + "_Width", c.Width.ToString());

            }

        }

 

        /// <summary>

        /// DataGridView látható oszlopainak szélességét olvassa vissza.

        /// </summary>

        /// <param name="dgv">A DataGridView objektum</param>

        /// <param name="adatbazisra">Ha igaz, akkor adatbázisonként keresi az oszlopokat</param>

        /// <param name="felhasznalora">Ha igaz, akkor felhasználónként keresi az oszlopokat</param>

        /// <param name="prefix">szöveges előtag, amivel azonosítható a DataGridView</param>

        public void GridSetColumnWidth(DataGridView dgv, bool adatbazisra, bool felhasznalora, string prefix)

        {

            foreach (DataGridViewColumn c in dgv.Columns)

            {

                if (c.Visible) c.Width = (int)GetControlUtolsoErtekLong(adatbazisra, felhasznalora, prefix + c.Name + "_Width", c.Width.ToString());

            }

        }*/

        #endregion



        #region NAVOnline

        /*

        public bool CheckNAVKuldendo(Int32 BizAz)

        {

            return (ExecuteSPAB("NAV_CheckBizKuldendo",

                new List<SqlParameter>()

                {

                    new SqlParameter() { ParameterName="@BizAz", SqlDbType=SqlDbType.Int, Direction=ParameterDirection.Input, Value=BizAz}

                }

                ) > 0);

        }

       

        public bool NAVSzamlaKuldes(Int32 BizAz)

        {

            Int32 requestAz = GetNAVSorszam(BizAz, out string requestId);

 

            NAVOnline nAVOnline = new NAVOnline(

                GetNAVConfig(

                    "invoice.xml"

                    , Application.LocalUserAppDataPath

                    , "ManageInvoice"

                    , requestId

                    , requestAz

                    , ""

                    , false

                ));

           

            nAVOnline.SetConnection(cnAB);

 

            File.WriteAllBytes(nAVOnline.Path + "\\" + nAVOnline.InvoiceFileName, Encoding.UTF8.GetBytes(GetNAVInvoiceXML(BizAz)));

 

            string tranId = nAVOnline.SzamlaKuldes();

            if (tranId=="")

            {

                RedMessageBox.Show(null, "Nem sikerült a számla beküldése!!!", "NAV Online küldés", MessageBoxDefaultButton.Button1);

                return false;

            }

            else

            {

                nAVOnline.RequestId = nAVOnline.RequestId + "q";

                nAVOnline.TransactionId = tranId;

                nAVOnline.SzamlaLekerdezes();

            }

            return true;

        }

 

        internal string GetNAVInvoiceXML(Int32 bizAz)

        {

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() { ParameterName="@BizAz", SqlDbType=SqlDbType.Int, Direction=ParameterDirection.Input, Value=bizAz }

            };

            DataTable dt = GetTableFromSPAB("NAV_GetBizXML", prms);

 

            return dt.Rows[0]["Adat"].ToString();

        }

 

        internal Int32 GetNAVSorszam(Int32 bizAz, out string requestId)

        {

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() { ParameterName="@Tipus", SqlDbType=SqlDbType.Int, Direction=ParameterDirection.Input, Value=0 }

                ,new SqlParameter() { ParameterName="@BizAz", SqlDbType=SqlDbType.Int, Direction=ParameterDirection.Input, Value=bizAz }

                ,new SqlParameter() { ParameterName="@RequestId", SqlDbType=SqlDbType.NVarChar, Size=50, Direction=ParameterDirection.Output }

            };

            Int32 ret = (Int32)ExecuteSPAB("NAV_GetSorszam", prms);

            requestId = prms[2].Value.ToString();

            return ret;

        }

 

        internal NAVOnlineConfig GetNAVConfig(

            string invoiceFileName

            ,string path

            ,string muvelet

            ,string requestId

            ,Int32 requestAz

            ,string transactionId

            ,bool modify

            )

        {

            List<SqlParameter> prms = new List<SqlParameter>()

            {

                new SqlParameter() { ParameterName="@InvoiceFileName", SqlDbType=SqlDbType.NVarChar, Size=2000, Direction=ParameterDirection.Input, Value=invoiceFileName }

                ,new SqlParameter() { ParameterName="@Path", SqlDbType=SqlDbType.NVarChar, Size=2000, Direction=ParameterDirection.Input, Value=path }

                ,new SqlParameter() { ParameterName="@Muvelet", SqlDbType=SqlDbType.NVarChar, Size=50, Direction=ParameterDirection.Input, Value=muvelet }

                ,new SqlParameter() { ParameterName="@RequestId", SqlDbType=SqlDbType.NVarChar, Size=50, Direction=ParameterDirection.Input, Value=requestId }

                ,new SqlParameter() { ParameterName="@RequestAz", SqlDbType=SqlDbType.Int, Direction=ParameterDirection.Input, Value=requestAz }

                ,new SqlParameter() { ParameterName="@TransactionId", SqlDbType=SqlDbType.NVarChar, Size=50, Direction=ParameterDirection.Input, Value=transactionId }

                ,new SqlParameter() { ParameterName="@Modify", SqlDbType=SqlDbType.Bit, Direction=ParameterDirection.Input, Value=modify }

            };

            DataTable dt = GetTableFromSPAB("NAV_GetConfigXML", prms);

            string adat = dt.Rows[0]["Adat"].ToString();

 

            XmlSerializer serializer = new XmlSerializer(typeof(Kontroll2015_NAVOnlineDll.NAVOnlineConfig));

            System.IO.MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(adat));

            NAVOnlineConfig conf = (NAVOnlineConfig)serializer.Deserialize(ms);

            ms.Close();

            return conf;

        }*/

        #endregion

    }

    public class Properties

    {

        public string HelyiKontrollVezerloSzerver;

        public string HelyiKontrollVezerloAdatbazis;

        public string FrissitoSzerver;

        public string FrissitoAdatbazis;

        public string KontrollLogAdatbazis;

        public string KontrollLogSzerver;

        public Properties()

        {

            HelyiKontrollVezerloSzerver = "";

            HelyiKontrollVezerloAdatbazis = "";

            FrissitoSzerver = "";

            FrissitoAdatbazis = "";

            KontrollLogAdatbazis = "";

            KontrollLogSzerver = "";

        }

    }

}
