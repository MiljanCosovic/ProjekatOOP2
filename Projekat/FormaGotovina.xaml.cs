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
using System.Windows.Shapes;

namespace Projekat
{
    /// <summary>
    /// Interaction logic for FormaGotovina.xaml
    /// </summary>
    public partial class FormaGotovina : Window
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public int BajsId { get; set; }
        public float UkupnaCena { get; set; }
        public string BrojTelefona { get; set; }
        public string NacinPlacanja { get; set; }
        public float TrajanjeSati { get; set; }
        public FormaGotovina()
        {
            InitializeComponent();
        }

        private void PlatiButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {



                string ulogovaniUsername = GlobaliniPodaci.UlogovaniKorisnik;

                string ime = GetImeKorisnika();
                string prezime = GetPrezimeKorisnika();

                if (!float.TryParse(txtBrojGotovine.Text, out float unetiIznos))
                {
                    MessageBox.Show("Unesite validan iznos u polje za gotovinu.");
                    return;
                }

                if (unetiIznos < UkupnaCena)
                {
                    MessageBox.Show("Uneti iznos ne može biti manji od ukupne cene.");
                    return;
                }
                float kusur = unetiIznos - UkupnaCena;


                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();


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

                    string insertQuery = "INSERT INTO Iznajmljivanja (IdBajsa, UserName, Ime, Prezime, BrojTelefona, TrajanjeSati, NacinPlacanja, Cena, Status,DatumVremeIznajmljivanja,Slika ) " +
                                         "VALUES (@IdBajsa, @UserName, @Ime, @Prezime, @BrojTelefona, @TrajanjeSati, @NacinPlacanja, @Cena, @Status,@DatumVremeIznajmljivanja, @Slika)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@IdBajsa", BajsId);
                        insertCmd.Parameters.AddWithValue("@UserName", ulogovaniUsername);
                        insertCmd.Parameters.AddWithValue("@Ime", ime);
                        insertCmd.Parameters.AddWithValue("@Prezime", prezime);
                        insertCmd.Parameters.AddWithValue("@BrojTelefona", BrojTelefona);
                        insertCmd.Parameters.AddWithValue("@TrajanjeSati", TrajanjeSati);
                        insertCmd.Parameters.AddWithValue("@NacinPlacanja", NacinPlacanja);
                        insertCmd.Parameters.AddWithValue("@Cena", UkupnaCena);
                        insertCmd.Parameters.AddWithValue("@Status", "iznajmljen");
                        insertCmd.Parameters.AddWithValue("@DatumVremeIznajmljivanja", DateTime.Now); 
                        insertCmd.Parameters.AddWithValue("@Slika", imageBytes);


                        int rowsAffected = insertCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string updateStatusQuery = "UPDATE Bicikli SET Status = 'iznajmljen' WHERE BajsId = @IdBajsa";

                            using (SqlCommand updateStatusCmd = new SqlCommand(updateStatusQuery, conn))
                            {
                                updateStatusCmd.Parameters.AddWithValue("@IdBajsa", BajsId);
                                updateStatusCmd.ExecuteNonQuery();
                            }
                            if (kusur > 0)
                            {
                                MessageBox.Show($"Uspešno ste platili. Vaš kusur je {kusur:F2} RSD.");
                            }
                            else
                            {
                                MessageBox.Show($"Uspešno ste platili.");
                            }

                            PrikaziPotvrduIznajmljivanja();
                            this.Close();
                            MessageBox.Show($"Uspešno ste iznajmili bicikl! Na kartici vrati bajs mozete vratiti bicikl!");
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
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }

        private string GetImeKorisnika()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string query = "SELECT Ime FROM Registracija WHERE UsernameR = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", GlobaliniPodaci.UlogovaniKorisnik);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }

                    return "Nepoznato ime";
                }
            }
        }

        private string GetPrezimeKorisnika()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string query = "SELECT Prezime FROM Registracija WHERE UsernameR = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", GlobaliniPodaci.UlogovaniKorisnik);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }

                    return "Nepoznato prezime";
                }
            }
        }

        private void PrikaziPotvrduIznajmljivanja()
        {
            Potvrda potvrdaWindow = new Potvrda();

            
            potvrdaWindow.DataContext = new
            {
                Korisnik = $"Korisnik: {GlobaliniPodaci.UlogovaniKorisnik}",
                Ime = $"Ime: {GetImeKorisnika()}",
                Prezime = $"Prezime: {GetPrezimeKorisnika()}",
                DatumVreme = $"Datum i vreme iznajmljivanja: {DateTime.Now}",
                Trajanje = $"Trajanje: {TrajanjeSati} sati",
                UkupnaCena = $"Ukupna cena: {UkupnaCena} RSD",
                DetaljiBicikla = $"Detalji o biciklu: Bicikl {BajsId}",
                VremeIznajmljivanja = $"Vreme iznajmljivanja: {DateTime.Now}",
                Napomena = $"Napomena: Korisnik je duzan da vati Bicikl posle  {TrajanjeSati} sata od preuzimanja."
            };

            potvrdaWindow.ShowDialog();
        }
    }
}
