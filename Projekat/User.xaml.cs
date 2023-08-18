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
using System.Windows.Shapes;

namespace Projekat
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Window
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public User()
        {
            InitializeComponent();
        }

        private void RentBike(object sender, RoutedEventArgs e)
        { 
            mainFrame.Content = new PrikazBajsovaUser();                
        }


        private void Gasenje(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void VratiBike(object sender, RoutedEventArgs e)
        {
            string ulogovaniUsername = GlobaliniPodaci.UlogovaniKorisnik;

            if (ImaIznajmljenBicikl(ulogovaniUsername))
            {
                mainFrame.Content = new VratiBajk();
            }
            else
            {
                MessageBox.Show("Ne možete da vratite bicikl jer ga niste iznajmili.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool ImaIznajmljenBicikl(string username)
        {
            bool imaIznajmljenBicikl = false;

           
            string query = $"SELECT COUNT(*) FROM IznajmljivanjA WHERE Username = @Username AND Status = 'iznajmljen';";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    int rowCount = (int)command.ExecuteScalar();
                    if (rowCount > 0)
                    {
                        imaIznajmljenBicikl = true;
                    }
                }
            }

            return imaIznajmljenBicikl;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            m.Show();
            Close();
        }
    }
}
