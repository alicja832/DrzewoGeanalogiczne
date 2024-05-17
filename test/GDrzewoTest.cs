using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990/12/01", SqlString.Null);
            List<Dictionary<string, string>> lista = drzewo.ListaCzlonkow();
            Assert.IsTrue(lista.Any(czlon => czlon["Imie"] == "Adam" && czlon["Nazwisko"] == "Kowalski"), "Dane Ÿle wprowadzono");
        }
        /**metoda sprawdzajaca wypisywanie wszystkich czlonkow, ktorzy byli dodani
         */
        [TestMethod]
         public void ListaCzlonkowTest()
         {
             GDrzewo drzewo = new GDrzewo();
             drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990-12-22", SqlString.Null);
             List<Dictionary<string, string>> lista = drzewo.ListaCzlonkow();
             Assert.IsTrue(lista.Any(czlon => czlon["Imie"] == "Adam" && czlon["Nazwisko"] == "Kowalski" && czlon["wiek"] == "34"), "Dane Ÿle wprowadzono");
         }
        /**
         * Sprawdzanie czy dodanie czlonka, ktory nie jest z nikim spokrewniony spowoduje wyrzucenie wyjatku
         */
        [TestMethod]
        public void DodajCzlonka_niespokrewnionego_Test()
        {
            GDrzewo drzewo = new GDrzewo();
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990-12-22", SqlString.Null);
            Assert.ThrowsException<System.ArgumentException>(() => drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Zurawski",
                "1990-12-22", SqlString.Null)); 
        }
        /**
         * metoda testujaca czy usuwanie czlonka powiod³o siê
         */
        [TestMethod]
        public void usuwanieCzlonkowTest()
        {
            GDrzewo drzewo = new GDrzewo();
            drzewo.DodajCzlonka(SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, "Adam", "Kowalski", "1990/12/01", SqlString.Null);
            drzewo.UsunCzlonka("/1/");
            List<Dictionary<String, String>> lista = drzewo.ListaCzlonkow();
            Assert.AreEqual(false , lista.Any(), "Danych nie usunieto");

        }
    }

}