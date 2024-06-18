using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Numerics;
using System.Security.Principal;
using UtilityLibraries;
namespace GDrzewoTests
{
    [TestClass]
    public class GDrzewoTest
    {
        /** Metoda sprawdzaj¹ca czy polaczenie z baza danych siê powiodlo
        */
        [TestMethod]
        public void GDrzewoconnectionTest()
        {
            GDrzewo drzewo = new GDrzewo();
            SqlConnection sqlConnection = drzewo.Connection;
            Assert.IsNotNull(sqlConnection, "Connection is not established");
            Assert.AreEqual(ConnectionState.Open, sqlConnection.State, "Connection is not open");

        }
        /** Metoda sprawdzaj¹ca czy dodawanie czlonka siê udalo
        */
        [TestMethod]
        public void DodajCzlonkaTest()
        {
            GDrzewo drzewo = new GDrzewo();
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990/12/01", "2013-12-22");
            Assert.ThrowsException<System.ArgumentException>(() => drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Zurawski",
               "1990-12-22", SqlString.Null));
            
            List<Dictionary<string, string>> lista = drzewo.ListaCzlonkow();
            Assert.IsTrue(lista.Any(), "Dane Ÿle wprowadzono");
            
            int intwar = 23;
            Assert.AreEqual(intwar.ToString(), lista[0]["lata"], "Dane Ÿle wprowadzono");
            Assert.ThrowsException<System.ArgumentException>(() => drzewo.DodajCzlonka("Adam", "Kowalski", SqlString.Null, SqlString.Null, "Adam", "Zurawski",
               "1990-11-22", SqlString.Null));
        }
        /**metoda sprawdzajaca wypisywanie wszystkich czlonkow, ktorzy byli dodani
         */
        [TestMethod]
         public void ListaCzlonkowTest()
         {
            GDrzewo drzewo = new GDrzewo();
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990-12-22", SqlString.Null);
            drzewo.DodajCzlonka("Adam", "Kowalski", SqlString.Null, SqlString.Null, "Piotr", "Kowalski", "2012-12-22", SqlString.Null);
            List<Dictionary<string, string>> lista = drzewo.ListaCzlonkow();
            Assert.IsTrue(lista.Any(czlon => czlon["Imie"] == "Adam" && czlon["Nazwisko"] == "Kowalski" && czlon["wiek"] == "34"), "Dane Ÿle wprowadzono");
            Assert.IsTrue(lista.Any(czlon => czlon["Imie"] == "Piotr" && czlon["Nazwisko"] == "Kowalski" && czlon["wiek"] == "12" && czlon["rodzic"]=="/1/"), "Dane Ÿle wprowadzono");
           
        }
        /**metoda sprawdzajaca wypisywanie wszystkich czlonkow, ktorzy byli dodani
       */
        [TestMethod]
        public void RaportTest()
        {
            GDrzewo drzewo = new GDrzewo();
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990-12-22", "2013-12-22");
            drzewo.DodajCzlonka("Adam", "Kowalski", SqlString.Null, SqlString.Null, "Piotr", "Kowalski", "2012-12-22", SqlString.Null);
            List<Dictionary<string, string>> lista = drzewo.Raport("/1/");
            Assert.IsTrue(lista.Any(czlon => czlon["Imie"] == "Piotr" && czlon["Nazwisko"] == "Kowalski" ), "Nieprawid³owy raport");
          
        }
       
       
       
        /**
         * metoda testujaca czy usuwanie czlonka powiod³o siê
         */
        [TestMethod]
        public void usuwanieCzlonkowTest()
        {
            GDrzewo drzewo = new GDrzewo();
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990/12/01", "2013-12-22");
            drzewo.UsunCzlonka("/1/");
            List<Dictionary<String, String>> lista = drzewo.ListaCzlonkow();
            Assert.AreEqual(false , lista.Any(), "Danych nie usunieto");

        }
    }

}