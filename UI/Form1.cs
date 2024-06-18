using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UtilityLibraries;
namespace DrzewoGenealogiczne
{
    public partial class Form1 : Form
    {
        private GDrzewo drzewo;
        //jest pomysl na raport - ilosc wnukow/dzieci danego malzenstwa!
        public Form1()
        {
            InitializeComponent();
            checkboxes = new Dictionary<int, List<CheckBox>>();
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
                label1.Text = "Nie udało się utworzyc drzewa";
                label1.Visible = true;
                return;
            }
            panel1.Visible = true;
            panel3.Visible = false;
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

            panel1.Controls.Remove(label11);
            label11.Text = "Dodano członka";

            try
            {
                //  public void DodajCzlonka(SqlString ImieRodzica, SqlString NazwiskoRodzica, SqlString ImieMalzonka, SqlString NazwiskoMalzonka, SqlString Imie, SqlString Nazwisko, SqlString DataUrodzenia, SqlString DataSmierci)
                drzewo.DodajCzlonka(imierodzica, nazwiskorodzica, imiemalzonka, nazwiskomalzonka, imie, nazwisko, dataurodzenia, datasmierci);
            }
            catch (Exception ex)
            {
                label11.Text =  ex.Message;
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
            panel1.Controls.Add(label11);
        }
        private int sumael()
        {
            int returned = 0;
            foreach (int j in checkboxes.Keys)
            {
                returned += checkboxes[j].Count;
            }
            return returned;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            button2.Visible = false;
            panel2.Visible = true;
            List<Dictionary<string, string>> lista;
           
            try
            {
                lista = drzewo.ListaCzlonkow();
            }
            catch (Exception ex)
            {
                label12.Text = ex.Message;
                label12.Visible = true;
               
                return;
            }
            int size = sumael();

            for (int i = size; i < lista.Count; i++)
            {
                int poziom = Int32.Parse(lista[i]["Poziom"]);
                if (!checkboxes.ContainsKey(poziom))
                    checkboxes[poziom] = new List<CheckBox>();

                CheckBox foundCheckBox = checkboxes[poziom].Find(x => (x.Name == lista[i]["Id"]));
              
                if (foundCheckBox == null)
                {
                    CheckBox tmp = new CheckBox();
                    tmp.AutoSize = true;
                    tmp.Size = new Size(100, 29);
                    tmp.TabIndex = 0;
                    tmp.Name = lista[i]["Id"];
                    tmp.Text = lista[i]["Imie"] + " " + lista[i]["Nazwisko"];

                    if (lista[i].ContainsKey("wiek"))
                        tmp.Text += "  wiek: " + lista[i]["wiek"];
                    if (lista[i].ContainsKey("lata"))
                        tmp.Text += " lata życia: " + lista[i]["lata"];

                    tmp.BackColor = SystemColors.ButtonShadow;
                    string data = lista[i]["rodzic"];
                    CheckBox CheckBoxParent = null;
                    int sizer = lista[i].Count;
                
                    if (poziom==1)
                    {
                        if (i==0)
                        {
                            tmp.Location = new Point(panel2.Width / (poziom + 1) * ( 1) - 50, 47 + 20 * poziom);
                        }
                        else
                        {
                            tmp.Location = new Point(100+ panel2.Width  / (poziom + 1) - 50, 47 + 20 * poziom);
                        }
                    }
                    else
                    {
                        CheckBoxParent = checkboxes[poziom - 1].Find(x => (x.Name == data));
                        int p = CheckBoxParent.Location.X;
                        int x = p - panel2.Width / (poziom + 3);

                        CheckBox foundplace;
                        int ind = 0;
                        int count = checkboxes[poziom].Count;
                        do
                        {
                            if (ind == count - 1)
                                foundplace = null;
                            else
                                foundplace = checkboxes[poziom].GetRange(ind, count).Find(checkbox => (checkbox.Location.X == x));

                            if (foundplace != null)
                                ind = checkboxes[poziom].IndexOf(foundplace);
                            tmp.Location = new Point(x, 47 + 20 * poziom);
                            if (lista[i].Count > 3)
                            {
                                x += (panel2.Width / (sizer));
                            }
                            else
                                x += panel2.Width / (poziom + 1);

                        } while (foundplace != null);
                    }
                    checkboxes[poziom].Add(tmp);
                    panel2.Controls.Add(tmp);
                }
            }
            button4.Visible = true;
            button2.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label12.Text = "Nic nie zaznaczono";
            try
            {
                foreach (int j in checkboxes.Keys)
                {
                    foreach (CheckBox c in checkboxes[j])
                    {

                        if (c.Checked)
                        {
                            drzewo.UsunCzlonka(c.Name);
                            panel2.Controls.Remove(c);
                            checkboxes[j].Remove(c);
                            label12.Text = "Usunięto";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                label12.Text = "Nie udało się usunąć członka";
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

        private void button5_Click(object sender, EventArgs e)
        {
            int position;
            panel4.Visible = true;
            panel4.Controls.Clear();
            panel4.Controls.Add(label14);
            panel4.Controls.Add(button6);
            // panel2.Visible = false;

            button6.Visible = true;
            List<Dictionary<string, string>> lista;
            Dictionary<int, List<Label>> labels = new Dictionary<int, List<Label>>();
            SqlString arg = null;
            label14.Text = "Nie udało się wygenerować raportu";


            foreach (int j in checkboxes.Keys)
            {
                foreach (CheckBox c in checkboxes[j])
                {

                    if (c.Checked)
                    {
                        arg = c.Name;

                    }
                }
            }
            if (arg == null)
                return;

            try
            {
                lista = drzewo.Raport(arg);
            }
            catch (Exception ex)
            {
                label14.Visible = true;
                return;
            }
            label14.Text = "Raport przedstawia listę wszystkich potomków wybranego członka";
            label14.Visible = true;
            labels.Clear();
            for (int i = 0; i < lista.Count; i++)
            {
                int poziom = Int32.Parse(lista[i]["Poziom"]);
                if (!labels.ContainsKey(poziom))
                    labels[poziom] = new List<Label>();

                Label tmp = new Label();
                tmp.AutoSize = true;
                tmp.Size = new Size(100, 29);
                tmp.TabIndex = 0;
                tmp.Text = lista[i]["Imie"] + " " + lista[i]["Nazwisko"];
                tmp.BackColor = SystemColors.ButtonShadow;
                labels[poziom].Add(tmp);
            }
            //dodawanie do panelu
            foreach (int j in labels.Keys)
            {
                for (int i = 0; i < labels[j].Count; i++)
                {

                    labels[j][i].Location = new Point(10 + 200 * i, 47 + 20 * j);
                    panel4.Controls.Add(labels[j][i]);
                }
            }


        }

        private void button6_Click(object sender, EventArgs e)
        {
            //najpierw musimy wywolac odpowiednia funkcje z GDrzewa- moim zdaniem pobieramy List<Dictionary<int, string>>
            //int to poziom osoby - czyli czy to jest dziecko czy wnuk czy prawnuk... nastepnie wkladamy to do 
            // Dictionary<int, List<Label>> labels; zeby to bylo ladnie i pokolei 1-funkcja
            panel4.Visible = false;

        }
    }
}