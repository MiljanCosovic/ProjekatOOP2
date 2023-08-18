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

namespace Projekat
{
    /// <summary>
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void Gasenje(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DodajBajs(object sender, RoutedEventArgs e)
        {
           mainFrame.Content = new DodajBajs();
        }

        private void PregledBajs(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new PrikazBicikl();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new Korisnici();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new Ostecenja();
        }

        private void Iznamjljeno(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new Iznajmljeno();
        }

        private void Odjava(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            m.Show();
            Close();
        }
    }
}
