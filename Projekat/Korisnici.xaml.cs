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
    public partial class Korisnici : Page
    {
        private const string ConnectionString = "Data Source=DESKTOP-1D72NJG;Initial Catalog=c#projekat;Integrated Security=True";

        public Korisnici()
        {
            InitializeComponent();
            UcitajKorisnike();
        }

        private void UcitajKorisnike()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT UserId, Ime, Prezime, Email, UsernameR FROM Registracija";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                           
                                TextBlock porukaTextBlock = new TextBlock
                                {
                                    Text = "Trenutno nema nijedan korisnik",
                                    FontSize = 16,
                                    Foreground = Brushes.Red,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                                wrapPanelKorisnici.Children.Add(porukaTextBlock);
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    int id = Convert.ToInt32(reader["UserId"]);
                                    string ime = reader["Ime"].ToString();
                                    string prezime = reader["Prezime"].ToString();
                                    string email = reader["Email"].ToString();
                                    string username = reader["UsernameR"].ToString();



                             
                                    Border border = new Border
                                    {
                                        Width = 220,
                                        Height = 300,
                                        Margin = new Thickness(10),
                                        BorderBrush = Brushes.Black,
                                        BorderThickness = new Thickness(1),
                                        Background = Brushes.LightGray

                                    };

                                    // Kreiramo Grid unutar Border-a
                                    Grid grid = new Grid();
                                    grid.HorizontalAlignment = HorizontalAlignment.Center;

                                 
                                    for (int i = 0; i < 6; i++)
                                    {
                                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                    }

                                    
                                    TextBlock naslovTextBlock = new TextBlock
                                    {
                                        Text = "Korisnik",
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 18,
                                        TextAlignment = TextAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Foreground = Brushes.White, // Boja teksta naslova
                                        Margin = new Thickness(0, 5, 0, 0)

                                    };
                                    Grid.SetRow(naslovTextBlock, 0);
                                    grid.Children.Add(naslovTextBlock);

                                    TextBlock imeTextBlock = new TextBlock
                                    {
                                        Text = $"Ime: {ime}",
                                        TextAlignment = TextAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 14,
                                        Margin = new Thickness(0, 5, 0, 0)
                                    };
                                    Grid.SetRow(imeTextBlock, 1);
                                    grid.Children.Add(imeTextBlock);

                                  
                                    TextBlock prezimeTextBlock = new TextBlock
                                    {
                                        Text = $"Prezime: {prezime}",
                                        TextAlignment = TextAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 14,
                                        Margin = new Thickness(0, 5, 0, 0)
                                    };
                                    Grid.SetRow(prezimeTextBlock, 2);
                                    grid.Children.Add(prezimeTextBlock);

                                  
                                    TextBlock emailTextBlock = new TextBlock
                                    {
                                        Text = $"Email:\n{email}",
                                        TextAlignment = TextAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 14,
                                        Margin = new Thickness(0, 5, 0, 0)
                                    };
                                    Grid.SetRow(emailTextBlock, 3);
                                    grid.Children.Add(emailTextBlock);

                             
                                    TextBlock usernameTextBlock = new TextBlock
                                    {
                                        Text = $"Username: {username}",
                                        TextAlignment = TextAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 14,
                                        Margin = new Thickness(0, 5, 0, 0)
                                    };
                                    Grid.SetRow(usernameTextBlock, 4);
                                    grid.Children.Add(usernameTextBlock);

                                    border.Child = grid;

                        
                                    wrapPanelKorisnici.Children.Add(border);

                        
                                    Button deleteButton = new Button
                                    {
                                        Content = "Obriši",
                                        Width = 100,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Bottom, 
                                        Margin = new Thickness(5),
                                        Tag = username
                                    };
                                    deleteButton.Click += DeleteButton_Click;
                                    Grid.SetRow(deleteButton, 5); 
                                    grid.Children.Add(deleteButton);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške prilikom učitavanja korisnika: " + ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string username)
            {
                MessageBoxResult result = MessageBox.Show("Da li ste sigurni da želite da izbrišete korisnika?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string deleteBicikliQuery = "DELETE FROM Registracija WHERE UsernameR = @Username";
                    string deleteLoginQuery = "DELETE FROM Login WHERE Username = @Username";

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(ConnectionString))
                        {
                            conn.Open();

                            
                            using (SqlCommand deleteRegistracijaCmd = new SqlCommand(deleteBicikliQuery, conn))
                            {
                                deleteRegistracijaCmd.Parameters.AddWithValue("@Username", username);
                                deleteRegistracijaCmd.ExecuteNonQuery();
                            }

                           
                            using (SqlCommand deleteLoginCmd = new SqlCommand(deleteLoginQuery, conn))
                            {
                                deleteLoginCmd.Parameters.AddWithValue("@Username", username);
                                deleteLoginCmd.ExecuteNonQuery();
                            }

                       
                            wrapPanelKorisnici.Children.Clear();
                            UcitajKorisnike();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Došlo je do greške prilikom brisanja korisnika: " + ex.Message);
                    }
                }
            }
        }



    }
}
