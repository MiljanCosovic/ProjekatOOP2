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
    /// Interaction logic for Iznajmljeno.xaml
    /// </summary>
    public partial class Iznajmljeno : Page
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public Iznajmljeno()
        {
            InitializeComponent();
            UcitajIznajmljeno();
        }


        private void UcitajIznajmljeno()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Iznajmljivanja";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                TextBlock porukaTextBlock = new TextBlock
                                {
                                    Text = "Trenutno nije iznajmljen nijedan bicikl",
                                    FontSize = 16,
                                    Foreground = Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                                wrapPanelIznajmljeno.Children.Add(porukaTextBlock);
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    int id = Convert.ToInt32(reader["IdBajsa"]);
                                    string username = reader["UserName"].ToString();
                                    string ime = reader["Ime"].ToString();
                                    string prezime = reader["Prezime"].ToString();
                                    int brTel = Convert.ToInt32(reader["BrojTelefona"]);
                                    int trajanje = Convert.ToInt32(reader["TrajanjeSati"]);
                                    string nacinPlacanja = reader["NacinPlacanja"].ToString();
                                    int cena = Convert.ToInt32(reader["Cena"]);
                                    string status = reader["Status"].ToString();

                                    DateTime datumIznajmljivanja = Convert.ToDateTime(reader["DatumVremeIznajmljivanja"]); 

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
                                        Height = 450,
                                        Margin = new Thickness(10),
                                        BorderBrush = Brushes.Black,
                                        BorderThickness = new Thickness(1)
                                    };
                                    StackPanel stackPanel = new StackPanel();

                                    Image image = new Image { Source = slika, Width = 150, Height = 150, HorizontalAlignment = HorizontalAlignment.Center };
                                    stackPanel.Children.Add(image);

                                    TextBlock idTextBlock = new TextBlock
                                    {
                                        Text = $"Id Bajsa: {id}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(idTextBlock);

                                    TextBlock korisnikTextBlock = new TextBlock
                                    {
                                        Text = "Korisnik koji je iznajmio bicikl",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(korisnikTextBlock);

                                 
                                    TextBlock imeTextBlock = new TextBlock
                                    {
                                        Text = $"Ime: {ime}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(imeTextBlock);

                        
                                    TextBlock prezimeTextBlock = new TextBlock
                                    {
                                        Text = $"Prezime: {prezime}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(prezimeTextBlock);

                              
                                    TextBlock trajanjeTextBlock = new TextBlock
                                    {
                                        Text = $"Trajanje (sati): {trajanje}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(trajanjeTextBlock);

                                    TextBlock nacinPlacanjaTextBlock = new TextBlock
                                    {
                                        Text = $"Način plaćanja: {nacinPlacanja}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(nacinPlacanjaTextBlock);

                                    TextBlock cenaTextBlock = new TextBlock
                                    {
                                        Text = $"Cena: {cena}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(cenaTextBlock);

                                    TextBlock statusTextBlock = new TextBlock
                                    {
                                        Text = $"Status: {status}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(statusTextBlock);

                             
                                    TextBlock datumTextBlock = new TextBlock
                                    {
                                        Text = $"Datum iznajmljivanja: {datumIznajmljivanja.ToShortDateString()}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(datumTextBlock);

                               
                                    border.Child = stackPanel;

                            
                                    wrapPanelIznajmljeno.Children.Add(border);
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

    }
}
