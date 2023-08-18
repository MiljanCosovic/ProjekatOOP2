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

namespace Projekat
{
    /// <summary>
    /// Interaction logic for PrikazBajsovaUser.xaml
    /// </summary>
    public partial class PrikazBajsovaUser : Page
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public PrikazBajsovaUser()
        {
            InitializeComponent();
            UcitajBicikleUser();
        }


        private void UcitajBicikleUser()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT BajsId, Opis, Cena, Slika FROM Bicikli where Status= 'dostupan'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                
                                TextBlock porukaTextBlock = new TextBlock
                                {
                                    Text = "Trenutno nema dostupnih bicikli",
                                    FontSize = 16,
                                    Foreground = Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                                wrapPanelBajkoviUser.Children.Add(porukaTextBlock);
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    int id = Convert.ToInt32(reader["BajsId"]);
                                    string opis = reader["Opis"].ToString();
                                    float cena = float.Parse(reader["Cena"].ToString());

                                    
                                    byte[] imageData = (byte[])reader["Slika"];
                                    BitmapImage slika = new BitmapImage();
                                    using (MemoryStream stream = new MemoryStream(imageData))
                                    {
                                        slika.BeginInit();
                                        slika.CacheOption = BitmapCacheOption.OnLoad;
                                        slika.StreamSource = stream;
                                        slika.EndInit();
                                    }

                                   
                                    Border border = new Border
                                    {
                                        Width = 220,
                                        Height = 300,
                                        Margin = new Thickness(10),
                                        BorderBrush = System.Windows.Media.Brushes.Black,
                                        BorderThickness = new Thickness(1)
                                    };

                                 
                                    Grid grid = new Grid();

                                  
                                    RowDefinition row1 = new RowDefinition { Height = new GridLength(40, GridUnitType.Star) }; // Visina prvog reda je 40% od ukupne visine
                                    RowDefinition row2 = new RowDefinition { Height = new GridLength(30, GridUnitType.Star) }; // Visina drugog reda je 30% od ukupne visine
                                    RowDefinition row3 = new RowDefinition { Height = new GridLength(30, GridUnitType.Star) }; // Visina trećeg reda je 30% od ukupne visine
                                    grid.RowDefinitions.Add(row1);
                                    grid.RowDefinitions.Add(row2);
                                    grid.RowDefinitions.Add(row3);

                                
                                    Image image = new Image { Source = slika, Width = 150, Height = 150, HorizontalAlignment = HorizontalAlignment.Center };
                                    Grid.SetRow(image, 0);
                                    grid.Children.Add(image);

                                   
                                    TextBlock opisTextBlock = new TextBlock
                                    {
                                        Text = opis,
                                        TextWrapping = TextWrapping.Wrap,
                                        TextAlignment = TextAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        HorizontalAlignment = HorizontalAlignment.Left,
                                        Margin = new Thickness(1)
                                    };
                                    Grid.SetRow(opisTextBlock, 1);
                                    grid.Children.Add(opisTextBlock);

                                 
                                    StackPanel cenaIDugmePanel = new StackPanel { Orientation = Orientation.Vertical };
                                    TextBlock cenaTextBlock = new TextBlock
                                    {
                                        Text = $"Cena po satu: {cena} RSD",
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center
                                    };
                                    Button preuzmiButton = new Button
                                    {
                                        Content = "Iznajmi",
                                        Width = 100,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5),
                                        Tag = id
                                    };
                                    preuzmiButton.Click += PreuzmiButton_Click; 
                                    cenaIDugmePanel.Children.Add(cenaTextBlock);
                                    cenaIDugmePanel.Children.Add(preuzmiButton);
                                    Grid.SetRow(cenaIDugmePanel, 2);
                                    grid.Children.Add(cenaIDugmePanel);

                                    
                                    border.Child = grid;

                                 
                                    wrapPanelBajkoviUser.Children.Add(border);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške prilikom učitavanja bajkova: " + ex.Message);
            }
        }

        private void PreuzmiButton_Click(object sender, RoutedEventArgs e)
        {
            string ulogovaniUsername = GlobaliniPodaci.UlogovaniKorisnik;
            if (sender is Button preuzmiButton && preuzmiButton.Tag is int bajsId)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();

                        string query = "SELECT Status FROM Iznajmljivanja WHERE UserName = @Korisnik";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Korisnik", ulogovaniUsername);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string status = reader["Status"].ToString();

                                    if (status == "iznajmljen")
                                    {
                                        // Korisnik ne može iznajmiti bicikl jer već ima aktivan iznajam
                                        MessageBox.Show("Ne možete iznajmiti bicikl jer već imate iznajmljen bicikl.");
                                    }
                                    else
                                    {
                                        // Korisnik može iznajmiti bicikl
                                        FormaZaRent formaZaRent = new FormaZaRent(bajsId);
                                        formaZaRent.ShowDialog(); // Otvorite formu kao dijalog
                                                                  // Zatvaranje trenutne stranice
                                        NavigationService.Navigate(new User());
                                        NavigationService.Content = null;
                                    }
                                }
                                else
                                {
                                    // Korisnik nema prethodnih iznajmljivanja, može iznajmiti bicikl
                                    FormaZaRent formaZaRent = new FormaZaRent(bajsId);
                                    formaZaRent.ShowDialog(); // Otvorite formu kao dijalog
                                                              // Zatvaranje trenutne stranice
                                    NavigationService.Navigate(new User());
                                    NavigationService.Content = null;
                                }
                            }
                        }
                    }
                   

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Došlo je do greške prilikom otvaranja forme za iznajmljivanje: " + ex.Message);
                }
            }
        }

    }
}
