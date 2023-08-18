using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;



namespace Projekat
{
    public partial class MainWindow : Window
    {
        private Korisnik korisnik;
        public MainWindow()
        {
            InitializeComponent();
            korisnik = new Korisnik();
        }

        private void dugme_Click(object sender, RoutedEventArgs e)
        {
            if (izbor.SelectedItem is ComboBoxItem selectedItem)
            {
                string userRole = selectedItem.Content.ToString().Trim();
                string enteredUsername = username.Text;
                string password = lozinka.Password;

                Korisnik korisnik = new Korisnik();
                string uspesnaPrijava = korisnik.Prijava(enteredUsername, password, userRole);

                if (uspesnaPrijava == "Uspješno ste prijavljeni!")
                {
                    GlobaliniPodaci.UlogovaniKorisnik = enteredUsername;
                    this.Close();
                    MessageBox.Show(uspesnaPrijava);
                }
                else
                {
                    MessageBox.Show(uspesnaPrijava);
                }
            }
            else
            {
                MessageBox.Show("Molimo vas izaberite ulogu (admin ili user).");
            }
        }


        private void reg_Click(object sender, RoutedEventArgs e)
        {
            registracija r = new registracija();
            r.Show();
            this.Hide();
        }

        private void Izlaz_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

