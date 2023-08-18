using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Projekat
{
    /// <summary>
    /// Interaction logic for FormaZaRent.xaml
    /// </summary>
    public partial class FormaZaRent : Window
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        private int BajsId { get; set; }
        private float CenaPoSatu { get; set; }
        public FormaZaRent(int bajsId)
        {
            InitializeComponent();
            BajsId = bajsId;
        }

        private void IzracunajButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtBrojTelefona.Text))
            {
                MessageBox.Show("Unesite broj telefona.");
                return;
            }
            else if (!IsValidPhoneNumber(txtBrojTelefona.Text))
            {
                MessageBox.Show("Unesite validan broj telefona (9 ili 10 cifara).");
                return;
            }
            if (float.TryParse(txtTrajanje.Text, out float trajanje) && trajanje > 0)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();

                        string query = "SELECT Cena FROM Bicikli WHERE BajsId = @BajsId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@BajsId", BajsId);
                            object cenaObj = cmd.ExecuteScalar();

                            if (cenaObj != null && float.TryParse(cenaObj.ToString(), out float cenaPoSatu))
                            {
                                CenaPoSatu = cenaPoSatu;
                                float ukupnaCena = CenaPoSatu * trajanje;
                                txtUkupnaCena.Text = ukupnaCena.ToString("F2") + " RSD";
                                txtNacinPlacanja.Visibility = Visibility.Visible;
                                btnIznajmi.Visibility = Visibility.Visible;
                                cmbNacinPlacanja.Visibility = Visibility.Visible;
                                stackOstecenje.Visibility = Visibility.Visible; 
                            }
                            else
                            {
                                MessageBox.Show("Nije moguće dobiti cenu po satu za izabrani bicikl.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Došlo je do greške prilikom izračuna cene: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Unesite validan broj sati trajanja iznajmljivanja.");
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {

            int phoneNumberLength = phoneNumber.Length;
            return (phoneNumberLength == 9 || phoneNumberLength == 10) && phoneNumber.All(char.IsDigit);
        }

        private void IznajmiButton_Click(object sender, RoutedEventArgs e)
        {
            if (chkOstecenje.IsChecked == true)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();

                        string updateQuery = "UPDATE Bicikli SET Status = 'Oštećeno' WHERE BajsId = @BajsId";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@BajsId", BajsId);
                            updateCmd.ExecuteNonQuery();
                        }


                        string selectImageQuery = $"SELECT Slika FROM Bicikli WHERE BajsId = @IdBajsa";
                        byte[] imageBytes;

                        using (SqlCommand command = new SqlCommand(selectImageQuery, conn))
                        {
                            command.Parameters.AddWithValue("@IdBajsa", BajsId);
                            imageBytes = (byte[])command.ExecuteScalar();
                        }

                        
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(imageBytes))
                        {
                            stream.Position = 0;
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();
                        }

                        string insertOstecenjaQuery = $"INSERT INTO Ostecenja (IdBajsa, Username, Ime, Prezime, VrstaOstecenja, Opis,Status, Slika) VALUES (@IdBajsa, @Username, @Ime,@Prezime,@VrtsaOstecenja, @Opis, @Status, @Slika)";

                        using (SqlCommand command = new SqlCommand(insertOstecenjaQuery, conn))
                        {
                            command.Parameters.AddWithValue("@IdBajsa", BajsId);
                            command.Parameters.AddWithValue("@Username", "Nepoznato");
                            command.Parameters.AddWithValue("@Ime", "Nepoznato");
                            command.Parameters.AddWithValue("@Prezime", "Nepoznato");
                            command.Parameters.AddWithValue("@VrtsaOstecenja", "Nepoznato");
                            command.Parameters.AddWithValue("@Opis", "Nepoznato");
                            command.Parameters.AddWithValue("@Status", "nije placeno");
                            command.Parameters.AddWithValue("@Slika", imageBytes);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {

                                MessageBox.Show("Prijavili ste da bicikl ima oštećenje. Molimo vas da izaberete drugi bicikl.");


                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Došlo je do greške prilikom upisivanja podataka.");
                            }
                            }
                        }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Došlo je do greške prilikom ažuriranja statusa bicikla: " + ex.Message);
                }
                return;
            }

            if (cmbNacinPlacanja.SelectedItem is ComboBoxItem selectedPlacanje)
            {
                string placanje = selectedPlacanje.Content.ToString();

                if (placanje == "Kartica")
                {
                    FormaKartica formaZaKarticu = new FormaKartica();
                    formaZaKarticu.BajsId = BajsId; 
                    formaZaKarticu.UkupnaCena = float.Parse(txtUkupnaCena.Text.Replace(" RSD", ""));
                    formaZaKarticu.BrojTelefona = txtBrojTelefona.Text;
                    formaZaKarticu.NacinPlacanja = placanje;
                    formaZaKarticu.TrajanjeSati = float.Parse(txtTrajanje.Text);
                    formaZaKarticu.ShowDialog();
                    this.Close();
                }
                else if (placanje == "Gotovina")
                {
                    FormaGotovina formaZaGotovinu = new FormaGotovina();
                    formaZaGotovinu.BajsId = BajsId; 
                    formaZaGotovinu.UkupnaCena = float.Parse(txtUkupnaCena.Text.Replace(" RSD", ""));
                    formaZaGotovinu.BrojTelefona = txtBrojTelefona.Text;
                    formaZaGotovinu.NacinPlacanja = placanje;
                    formaZaGotovinu.TrajanjeSati = float.Parse(txtTrajanje.Text);  
                    formaZaGotovinu.ShowDialog();
                    this.Close();
                }
                else if (placanje == "Mobilna plaćanja")
                {
                    FormaMobApp formaZaApp = new FormaMobApp();
                    formaZaApp.BajsId = BajsId;  
                    formaZaApp.UkupnaCena = float.Parse(txtUkupnaCena.Text.Replace(" RSD", ""));
                    formaZaApp.BrojTelefona = txtBrojTelefona.Text;
                    formaZaApp.NacinPlacanja = placanje;
                    formaZaApp.TrajanjeSati = float.Parse(txtTrajanje.Text);  
                    formaZaApp.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Molimo vas izaberite način plaćanja (gotovina, kartica, mobilno placanje).");
                }
            }
            else
            {
                MessageBox.Show("Molimo vas izaberite način plaćanja (gotovina, kartica, mobilno placanje).");
            }
        }




    }
}
