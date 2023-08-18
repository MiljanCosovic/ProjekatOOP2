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
    /// Interaction logic for PrijaviOstecenje.xaml
    /// </summary>
    public partial class PrijaviOstecenje : Window
    {
        public PrijaviOstecenje()
        {
            InitializeComponent();
        }

        private void PrijaviOstecenje_Click(object sender, RoutedEventArgs e)
        {
            string vrstaOstecenja = ((ComboBoxItem)cmbVrstaOstecenja.SelectedItem).Content.ToString();
            int cena = Convert.ToInt32(((ComboBoxItem)cmbVrstaOstecenja.SelectedItem).Tag);
            string opis = txtOpisOstecenja.Text;
            string nacinPlacanja = ((ComboBoxItem)cmbNacinPlacanja.SelectedItem).Content.ToString();

            if (nacinPlacanja == "Gotovina")
            {
                GotovinaOsteta gotovinaOstetaProzor = new GotovinaOsteta(vrstaOstecenja, cena, opis);
                gotovinaOstetaProzor.Show();
            }
            else if (nacinPlacanja == "Kartica")
            {
                KarticaOsteta karticaOstetaProzor = new KarticaOsteta(vrstaOstecenja, cena, opis);
                karticaOstetaProzor.Show();
            }
            else if (nacinPlacanja == "Mobilno plaćanje")
            {
                MobilnoPlacanjeOsteta mobilnoPlacanjeOstetaProzor = new MobilnoPlacanjeOsteta(vrstaOstecenja, cena, opis);
                mobilnoPlacanjeOstetaProzor.Show();
            }

            this.Close();
        }

    }
}
