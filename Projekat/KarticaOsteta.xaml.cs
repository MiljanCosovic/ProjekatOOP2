﻿using System;
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
    /// Interaction logic for KarticaOsteta.xaml
    /// </summary>
    public partial class KarticaOsteta : Window
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        private string vrstaOstecenja;
        private int cena;
        private string opis;
        public KarticaOsteta(string vrstaOstecenja, int cena, string opis)
        {
            InitializeComponent();
            this.vrstaOstecenja = vrstaOstecenja;
            this.cena = cena;
            this.opis = opis;
            txtCena.Text = cena.ToString();
        }

        private void PlatiButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBrojKartice.Text) || !IsValidId(txtBrojKartice.Text))
                {
                    MessageBox.Show("Unesite validan broj kartice (vise od 4 cifre).");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPin.Password) || !IsValidPin(txtPin.Password))
                {
                    MessageBox.Show("Unesite validan PIN (4 cifre).");
                    return;
                }



                string ulogovaniUsername = GlobaliniPodaci.UlogovaniKorisnik;
                string ime = GetImeKorisnika();
                string prezime = GetPrezimeKorisnika();


                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string getIdBajsaQuery = "SELECT IdBajsa FROM Iznajmljivanja WHERE UserName = @UserName AND Status = 'iznajmljen'";

                    int idBajsa;

                    using (SqlCommand getIdBajsaCmd = new SqlCommand(getIdBajsaQuery, conn))
                    {
                        getIdBajsaCmd.Parameters.AddWithValue("@UserName", ulogovaniUsername);
                        idBajsa = (int)getIdBajsaCmd.ExecuteScalar();
                    }
                    string selectImageQuery = $"SELECT Slika FROM Bicikli WHERE BajsId = @IdBajsa";
                    byte[] imageBytes;

                    using (SqlCommand command = new SqlCommand(selectImageQuery, conn))
                    {
                        command.Parameters.AddWithValue("@IdBajsa", idBajsa);
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

                    string updateStatusQuery = "DELETE FROM Iznajmljivanja WHERE UserName = @UserName AND Status = 'iznajmljen'";

                    using (SqlCommand updateStatusCmd = new SqlCommand(updateStatusQuery, conn))
                    {
                        updateStatusCmd.Parameters.AddWithValue("@UserName", ulogovaniUsername);
                        updateStatusCmd.ExecuteNonQuery();
                    }

                    string updateBiciklStatusQuery = "UPDATE Bicikli SET Status = 'ostecen' WHERE BajsId = @IdBajsa AND Status = 'iznajmljen'";

                    using (SqlCommand updateBiciklStatusCmd = new SqlCommand(updateBiciklStatusQuery, conn))
                    {
                        updateBiciklStatusCmd.Parameters.AddWithValue("@IdBajsa", idBajsa);
                        updateBiciklStatusCmd.ExecuteNonQuery();
                    }

                    string insertOstecenjaQuery = $"INSERT INTO Ostecenja (IdBajsa, Username, Ime, Prezime, VrstaOstecenja, Opis,Status, Slika) VALUES (@IdBajsa, @Username, @Ime,@Prezime,@VrtsaOstecenja, @Opis, @Status, @Slika)";

                    using (SqlCommand command = new SqlCommand(insertOstecenjaQuery, conn))
                    {
                        command.Parameters.AddWithValue("@IdBajsa", idBajsa);
                        command.Parameters.AddWithValue("@Username", ulogovaniUsername);
                        command.Parameters.AddWithValue("@Ime", ime);
                        command.Parameters.AddWithValue("@Prezime", prezime);
                        command.Parameters.AddWithValue("@VrtsaOstecenja", vrstaOstecenja);
                        command.Parameters.AddWithValue("@Opis", opis);
                        command.Parameters.AddWithValue("@Status", "placeno");
                        command.Parameters.AddWithValue("@Slika", imageBytes);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Uspešno ste platili! Sa vašeg računa je skinuto {cena} RSD.");
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
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }


        private bool IsValidId(string id)
        {
          
            return id.Length > 4 && id.All(char.IsDigit);

        }

        private bool IsValidPin(string pin)
        {
         
            return pin.Length == 4 && pin.All(char.IsDigit);
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

    }
}
