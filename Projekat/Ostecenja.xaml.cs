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
    /// Interaction logic for Ostecenja.xaml
    /// </summary>
    public partial class Ostecenja : Page
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";
        public Ostecenja()
        {
            InitializeComponent();
            UcitajOstecenja();

        }


        private void UcitajOstecenja()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Ostecenja";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                TextBlock porukaTextBlock = new TextBlock
                                {
                                    Text = "Trenutno nije ostecen nijedan bicikl",
                                    FontSize = 16,
                                    Foreground = Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                                wrapPanelOstecenja.Children.Add(porukaTextBlock);
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    int id = Convert.ToInt32(reader["IdBajsa"]);
                                    string ime = reader["Ime"].ToString();
                                    string prezime = reader["Prezime"].ToString();
                                    string Vstaostecenja = reader["VrstaOstecenja"].ToString();
                                    string opis = reader["Opis"].ToString();
                                    string status = reader["Status"].ToString();

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

                                    
                                    TextBlock KorisnikTextBlock = new TextBlock
                                    {

                                        Text = "Korisnik koji je vozio bicikl",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(KorisnikTextBlock);

                                 
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

                                  
                                    TextBlock vrstaOstecenjaTextBlock = new TextBlock
                                    {
                                        Text = $"Vrsta oštećenja: {Vstaostecenja}",
                                        FontSize = 14,
                                        TextWrapping = TextWrapping.Wrap,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(vrstaOstecenjaTextBlock);

                                    TextBlock opisTextBlock = new TextBlock
                                    {
                                        Text = $"Opis: {opis}",
                                        FontSize = 14,
                                        TextWrapping = TextWrapping.Wrap,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(opisTextBlock);

                                    TextBlock statusTextBlock = new TextBlock
                                    {
                                        Text = $"Status: {status}",
                                        FontSize = 14,
                                        Margin = new Thickness(5)
                                    };
                                    stackPanel.Children.Add(statusTextBlock);

                                    Button platiButton = new Button
                                    {
                                        Content = "Popravi",
                                        FontSize = 14,
                                        Width = 100,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Bottom,
                                        Margin = new Thickness(5),
                                    };
                                    platiButton.Click += PopraviButton_Click;
                                    platiButton.Tag = id; 
                                    stackPanel.Children.Add(platiButton);


          
                                    border.Child = stackPanel;

                            
                                    wrapPanelOstecenja.Children.Add(border);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške prilikom učitavanja bicikla: " + ex.Message);
            }
        }

        private void PopraviButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button popraviButton = sender as Button;
                if (popraviButton == null)
                    return;

                int idBajsa = (int)popraviButton.Tag; 

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    
                    string deleteOstecenjaQuery = "DELETE FROM Ostecenja WHERE IdBajsa = @IdBajsa";

                    using (SqlCommand deleteOstecenjaCmd = new SqlCommand(deleteOstecenjaQuery, conn))
                    {
                        deleteOstecenjaCmd.Parameters.AddWithValue("@IdBajsa", idBajsa);
                        deleteOstecenjaCmd.ExecuteNonQuery();
                    }

                  
                    string updateBiciklStatusQuery = "UPDATE Bicikli SET Status = 'dostupan' WHERE BajsId = @IdBajsa";

                    using (SqlCommand updateBiciklStatusCmd = new SqlCommand(updateBiciklStatusQuery, conn))
                    {
                        updateBiciklStatusCmd.Parameters.AddWithValue("@IdBajsa", idBajsa);
                        updateBiciklStatusCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Bicikl je popravljen i vraćen u stanje 'dostupno'.");
                    wrapPanelOstecenja.Children.Clear();
                    UcitajOstecenja();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške prilikom popravke bicikla: " + ex.Message);
            }
        }

    }
}
