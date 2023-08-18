using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace Projekat
{
    /// <summary>
    /// Interaction logic for VratiBajk.xaml
    /// </summary>
    public partial class VratiBajk : Page
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public VratiBajk()
        {
            InitializeComponent();
        }



        private void Potvrdi(object sender, RoutedEventArgs e)
        {
            bool biciklOstecen = chkBiciklOstecen.IsChecked ?? false;

            try
            {
                if (string.IsNullOrWhiteSpace(txtMestoPovratka.Text))
                {
                    MessageBox.Show("Morate uneti mesto gde ostavljate bicikl!");
                    return;
                }
                if (biciklOstecen)
                {
                    PrijaviOstecenje p = new PrijaviOstecenje();
                    p.Show();
                    NavigationService.Navigate(new User());
                    NavigationService.Content = null;
                }
                else
                {

                    string ulogovaniUsername = GlobaliniPodaci.UlogovaniKorisnik;

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

                        string updateStatusQuery = "DELETE FROM Iznajmljivanja WHERE UserName = @UserName AND Status = 'iznajmljen'";

                        using (SqlCommand updateStatusCmd = new SqlCommand(updateStatusQuery, conn))
                        {
                            updateStatusCmd.Parameters.AddWithValue("@UserName", ulogovaniUsername);
                            updateStatusCmd.ExecuteNonQuery();
                        }

                        string updateBiciklStatusQuery = "UPDATE Bicikli SET Status = 'dostupan' WHERE BajsId = @IdBajsa AND Status = 'iznajmljen'";

                        using (SqlCommand updateBiciklStatusCmd = new SqlCommand(updateBiciklStatusQuery, conn))
                        {
                            updateBiciklStatusCmd.Parameters.AddWithValue("@IdBajsa", idBajsa);
                            updateBiciklStatusCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Bicikl je uspešno vraćen.");

        
                        NavigationService.Navigate(new User());
                        NavigationService.Content = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
        }


    }
}
