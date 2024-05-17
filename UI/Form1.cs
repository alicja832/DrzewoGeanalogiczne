using System.Data.SqlTypes;
using UtilityLibraries;
namespace DrzewoGenealogiczne
{
    public partial class Form1 : Form
    {
        private GDrzewo drzewo;
        public Form1()
        {
            InitializeComponent();
            checkboxes = new List<CheckBox>(); 
            drzewo = null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                drzewo = new GDrzewo();
            }
            catch (Exception ex)
            {
                label1.Text = "Nie uda³o siê utworzyæ drzewa";
                label1.Visible = true;
                return;
            }
            panel1.Visible = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SqlString imie = String.IsNullOrEmpty(textBox1.Text) ? SqlString.Null : textBox1.Text;
            SqlString nazwisko = String.IsNullOrEmpty(textBox2.Text) ? SqlString.Null : textBox2.Text;
            SqlString imierodzica = String.IsNullOrEmpty(textBox7.Text) ? SqlString.Null : textBox7.Text;
            SqlString nazwiskorodzica = String.IsNullOrEmpty(textBox8.Text) ? SqlString.Null : textBox8.Text;
            SqlString imiemalzonka = String.IsNullOrEmpty(textBox3.Text) ? SqlString.Null : textBox3.Text;
            SqlString nazwiskomalzonka = String.IsNullOrEmpty(textBox4.Text) ? SqlString.Null : textBox4.Text;
            SqlString dataurodzenia = String.IsNullOrEmpty(textBox6.Text) ? SqlString.Null : textBox6.Text;
            SqlString datasmierci = String.IsNullOrEmpty(textBox5.Text) ? SqlString.Null : textBox5.Text;

            try
            {
                //  public void DodajCzlonka(SqlString ImieRodzica, SqlString NazwiskoRodzica, SqlString ImieMalzonka, SqlString NazwiskoMalzonka, SqlString Imie, SqlString Nazwisko, SqlString DataUrodzenia, SqlString DataSmierci)

                drzewo.DodajCzlonka(imierodzica, nazwiskorodzica, imiemalzonka, nazwiskomalzonka, imie, nazwisko, dataurodzenia, datasmierci);
            }
            catch (Exception ex)
            {
                label11.Text = "Nie uda³o siê dodaæ u¿ytkownika:" +ex.Message;
            }
            finally
            {
                button2.Visible = true;
                label11.Visible = true;
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = null;
                textBox6.Text = null;
                textBox7.Text = null;
                textBox8.Text = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            List<Dictionary<string, string>> lista = drzewo.ListaCzlonkow();
            int size = checkboxes.Count;


            for (int i = size; i < lista.Count; i++)
            {
                
                checkboxes.Add(new CheckBox());
                checkboxes[i].AutoSize = true;
                checkboxes[i].Location = new Point(100, 100 + 20 * i);
                checkboxes[i].Name = "checkBox1";
                checkboxes[i].Size = new Size(100, 29);
                checkboxes[i].TabIndex = 0;
                checkboxes[i].Name = lista[i]["Id"];
                checkboxes[i].Text = lista[i]["Imie"] + " " + lista[i]["Nazwisko"];
                if (lista[i].ContainsKey("wiek"))
                    checkboxes[i].Text += "  wiek: " + lista[i]["wiek"];
                if (lista[i].ContainsKey("lata"))
                    checkboxes[i].Text += " lata ¿ycia: " + lista[i]["lata"];


                checkboxes[i].BackColor = SystemColors.ButtonShadow;
            }
            for (int i = 0; i < checkboxes.Count; i++)
            {
                panel2.Controls.Add(checkboxes[i]);
            }

            panel2.Visible = true;
            button4.Visible = true;
            button2.Visible = false;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            label12.Text = "Usuniêto";
            try
            {
                foreach (CheckBox i in checkboxes)
                {
                    if (i.Checked)
                    {
                        drzewo.UsunCzlonka(i.Name);
                        i.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                label12.Text = "Nie uda³o siê usun¹æ";
            }
            finally
            {
                label12.Visible = true;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
            button2.Visible = true;
            button4.Visible = false;
        }
    }
}
