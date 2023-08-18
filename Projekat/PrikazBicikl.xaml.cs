using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Projekat
{
    public partial class PrikazBicikl : Page
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";

        public PrikazBicikl()
        {
            InitializeComponent();
            UcitajBicikle();
        }

        private void UcitajBicikle()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT BajsId, Opis, Cena, Slika FROM Bicikli";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                               
                                TextBlock porukaTextBlock = new TextBlock
                                {
                                    Text = "Trenutno nema bicikl",
                                    FontSize = 16,
                                    Foreground = Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                                wrapPanelBajkovi.Children.Add(porukaTextBlock);
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

                                   
                                    RowDefinition row1 = new RowDefinition { Height = new GridLength(40, GridUnitType.Star) }; 
                                    RowDefinition row2 = new RowDefinition { Height = new GridLength(30, GridUnitType.Star) }; 
                                    RowDefinition row3 = new RowDefinition { Height = new GridLength(30, GridUnitType.Star) }; 
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
                                    Button deleteButton = new Button
                                    {
                                        Content = "Obriši",
                                        Width = 100,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Margin = new Thickness(5),
                                        Tag = id
                                    };
                                    deleteButton.Click += DeleteButton_Click;
                                    cenaIDugmePanel.Children.Add(cenaTextBlock);
                                    cenaIDugmePanel.Children.Add(deleteButton);
                                    Grid.SetRow(cenaIDugmePanel, 2);
                                    grid.Children.Add(cenaIDugmePanel);

                                    
                                    border.Child = grid;

                                   
                                    wrapPanelBajkovi.Children.Add(border);
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






        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                MessageBoxResult result = MessageBox.Show("Da li ste sigurni da želite da izbrišete ovaj bicikl?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string deleteQuery = "DELETE FROM Bicikli WHERE BajsId = @Id";
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(ConnectionString))
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                      
                        wrapPanelBajkovi.Children.Clear();
                        UcitajBicikle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Došlo je do greške prilikom brisanja bajka: " + ex.Message);
                    }
                }
            }
        }

    }
}
