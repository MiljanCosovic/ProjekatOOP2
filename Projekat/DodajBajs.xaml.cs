using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Projekat
{
    public partial class DodajBajs : Page
    {
        private Bicikl bicikl;
        private byte[] slikaBytes;

        public DodajBajs()
        {
            InitializeComponent();
            bicikl = new Bicikl();
        }

        private void btnDodajSliku_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;

                try
                {
                    // Učitavanje slike i konverzija u byte[]
                    slikaBytes = File.ReadAllBytes(selectedFileName);

                    // Prikazivanje izabrane slike u Image kontrolu
                    BitmapImage bitmapImage = new BitmapImage(new Uri(selectedFileName));
                    imgPrikazSlike.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška prilikom učitavanja slike: " + ex.Message);
                }
            }
        }

        private void btnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOpis.Text) || !float.TryParse(txtCena.Text, out float cena) || slikaBytes == null)
    {
        string errorMsg = "Morate uneti sve podatke pre nego što dodate bicikl.";

        if (!float.TryParse(txtCena.Text, out cena))
        {
            errorMsg += "\nCena mora biti uneta kao broj.";
        }

        MessageBox.Show(errorMsg, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    string opis = txtOpis.Text;
    string status = "dostupan";

    bicikl.DodajBicikl(opis, cena, slikaBytes, status);

    MessageBox.Show("Uspešno ste dodali bicikl!", "Potvrda", MessageBoxButton.OK, MessageBoxImage.Information);

    txtOpis.Clear();
    txtCena.Clear();
    imgPrikazSlike.Source = null;
    slikaBytes = null;

        }
    }
}
