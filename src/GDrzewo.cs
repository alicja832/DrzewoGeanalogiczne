using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
namespace UtilityLibraries;

    public class GDrzewo
    {
        private string nazwa;
        private SqlConnection connection { get;set; }

    /**
     * konsruktor z domyslną nazwą drzewa, inicjuje polaczenie, tworzy drzewo za pomoca gotowej procedury skladowanej 
     */
    public GDrzewo(string nazwaRodziny = "Drzewo Geanalogiczne")
        {
            nazwa = nazwaRodziny;
            string sqlconnection = @"SERVER=MSSQLServer; " + "INITIAL CATALOG = GDrzewo; INTEGRATED SECURITY = SSPI; Server = (local)";
            try
            {
                connection = new SqlConnection(sqlconnection);
                connection.Open();
                StworzDrzewo();
            }
            catch (SqlException ex)
            {
                throw new ArgumentException("Nie udało się połączyć z bazą danych:" + ex.Message);
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }
        /** Funkcja tworząca drzewo geanalogiczne
         */
        private void StworzDrzewo()
        {
            string sqlcommand = "EXEC StworzDrzewo";

            SqlCommand command = null;
            try
            {
          
                command = new SqlCommand(sqlcommand, connection);
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new ArgumentException("Nie udało się utworzyć drzewa"+ ex.Message);
            }
        }      
        /**
         * Funkcja dodajaca osobe do drzewa
         */
        public void DodajCzlonka(SqlString ImieRodzica, SqlString NazwiskoRodzica, SqlString ImieMalzonka, SqlString NazwiskoMalzonka, SqlString Imie, SqlString Nazwisko, SqlString DataUrodzenia, SqlString DataSmierci)
        {
            string sqlcommand = "EXEC DodajCzlonka  @ImieRodzica, @NazwiskoRodzica" +
                ", @ImieMalzonka , @NazwiskoMalzonka ,@Imie,@Nazwisko ,@DataUrodzenia, @DataSmierci";
            SqlCommand command = null;
            try
            {
                command = new SqlCommand(sqlcommand, connection);
                command.Parameters.Add("@ImieRodzica", SqlDbType.NVarChar, 20).Value = ImieRodzica;
                command.Parameters.Add("@NazwiskoRodzica", SqlDbType.NVarChar, 20).Value = NazwiskoRodzica;
                command.Parameters.Add("@ImieMalzonka", SqlDbType.NVarChar, 20).Value = ImieMalzonka;
                command.Parameters.Add("@NazwiskoMalzonka", SqlDbType.NVarChar, 20).Value = NazwiskoMalzonka;
                command.Parameters.Add("@Imie", SqlDbType.NVarChar, 20).Value = Imie;
                command.Parameters.Add("@Nazwisko", SqlDbType.NVarChar, 20).Value = Nazwisko;
                command.Parameters.Add("@DataUrodzenia", SqlDbType.Date).Value = DateTime.Parse(DataUrodzenia.Value);
                if (DataSmierci != SqlString.Null)
                    command.Parameters.Add("@DataSmierci", SqlDbType.Date).Value = DateTime.Parse(DataSmierci.Value);
                else
                    command.Parameters.Add("@DataSmierci", SqlDbType.Date).Value = SqlDateTime.Null;
               command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new ArgumentException(ex.Message, nameof(ex));

            }

        }
    public void UsunCzlonka(SqlString CzlonekId)
    {
        string sqlcommand = "DELETE FROM GDrzewo WHERE CzlonekId=CAST (  @CzlonekId AS hierarchyid )";
        SqlCommand command = null;
        try
        {
            command = new SqlCommand(sqlcommand, connection);
            command.Parameters.Add("@CzlonekId",SqlDbType.VarChar,5).Value = CzlonekId;
            command.ExecuteNonQuery();

        }catch(SqlException ex)
        {
            throw new ArgumentException(ex.Message, nameof(ex));
        }
    }
        public int ilosc()
        {
            
            return 0;
        }
        public SqlConnection Connection
        {
            get => connection;
        }
        public string Nazwa
        {
            get => nazwa;
            set => nazwa = value;
        }
        public List<Dictionary<string, string>> ListaCzlonkow()
        {
      
        List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            string sqlcommand1 = "SELECT CzlonekId.ToString() as Id,Poziom,Imie,Nazwisko,ImieMalzonka,NazwiskoMalzonka , (YEAR(GETDATE()) - YEAR(DataUrodzenia)) as wiek from GDrzewo WHERE DataSmierci is  NULL";
            string sqlcommand2 = "SELECT CzlonekId.ToString() as Id,Poziom,Imie,Nazwisko,ImieMalzonka,NazwiskoMalzonka, (YEAR(DataSmierci) - YEAR(DataUrodzenia)) as lata from GDrzewo WHERE DataSmierci is not NULL";
            SqlCommand command1 = null;
            SqlCommand command2 = null;
        try
                {
                    command1 = new SqlCommand(sqlcommand1, connection);
                    SqlDataReader datareader  = command1.ExecuteReader();
                 
                     while (datareader.Read())
                     {

                            Dictionary<string,string> tmp = new Dictionary<string,string>();

                            tmp["Id"]= datareader["Id"].ToString();
                            tmp["Poziom"] = datareader["Poziom"].ToString();
                            tmp["Imie"] = datareader["Imie"].ToString();
                            tmp["Nazwisko"] = datareader["Nazwisko"].ToString();
                            tmp["wiek"] = datareader["wiek"].ToString();

                        if (datareader["ImieMalzonka"] != null)
                            tmp["ImieMalzonka"]=datareader["ImieMalzonka"].ToString();
                        if (datareader["NazwiskoMalzonka"] != null)
                            tmp["NazwiskoMalzonka"] = datareader["NazwiskoMalzonka"].ToString();
                
                        data.Add(tmp);
                     }
                    datareader.Close();
                    command2 = new SqlCommand(sqlcommand2, connection);
                    SqlDataReader datareader2 = command2.ExecuteReader();

            
                while (datareader2.Read())
                {

                    Dictionary<string, string> tmp = new Dictionary<string, string>();

                    tmp["Id"] = datareader2["Id"].ToString();
                    tmp["Poziom"] = datareader2["Poziom"].ToString();
                    tmp["Imie"] = datareader2["Imie"].ToString();
                    tmp["Nazwisko"] = datareader2["Nazwisko"].ToString();
                    tmp["lata"] = datareader2["lata"].ToString();

                    if (datareader["ImieMalzonka"] != null)
                        tmp["ImieMalzonka"] = datareader2["ImieMalzonka"].ToString();
                    if (datareader["NazwiskoMalzonka"] != null)
                        tmp["NazwiskoMalzonka"] = datareader2["ImieMalzonka"].ToString();

                    data.Add(tmp);
                }
            datareader2.Close();

                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Wyprowadzenie danych nie powiodło się.", nameof(ex));
      
                }
                return data;
        }
        ~GDrzewo()
        {
            
                string sqlcommand = "DROP TABLE IF EXISTS GDrzewo";
                SqlCommand command = new SqlCommand(sqlcommand, connection);
                command.ExecuteNonQuery();
            
            if (connection != null)
                connection.Close();

        }
    }
