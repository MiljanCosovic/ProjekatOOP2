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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Projekat
{
    /// <summary>
    /// </summary>
    public partial class registracija : Window
    {
        private Korisnik korisnik = new Korisnik();
        public registracija()
        {
            InitializeComponent();
        }


        private void potvrdi_Click(object sender, RoutedEventArgs e)
        {
            string ime = imeR.Text;
            string prezime = prezimeR.Text;
            string email = emailR.Text;
            string username = usernameR.Text;
            string lozinka = lozinkaR.Password;
            string potvrdaLozinke = potvrdaLozR.Password;

            Korisnik korisnik = new Korisnik();
            string rezultatRegistracije = korisnik.Registracija(ime, prezime, email, username, lozinka, potvrdaLozinke);

            MessageBox.Show(rezultatRegistracije);

            if (rezultatRegistracije == "Uspešno ste se registrovali! Možete se sada ulogovati i koristiti aplikaciju!")
            {
                MainWindow login = new MainWindow();
                login.Show();
                this.Close();
            }

        }

        private void Izlaz_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NazadNaLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }  
}
    

