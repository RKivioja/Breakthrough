using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lauta
{
    /// <summary>
    /// Interaction logic for Lauta.xaml
    /// </summary>
    public partial class Lauta : UserControl
    {
        private static Nappula.Nappula valittuNappula;
        private static Nappula.Nappula poistettavaNappula;
        private static bool mustanVuoro;
        private static bool peliOhi;

        private static int siirrettavacolumn;
        private static int siirrettavarow;
        private static int kohderuutucolumn;
        private static int kohderuuturow;

        private static int mustiaNappuloita;
        private static int punaisiaNappuloita;


        /// <summary>
        /// Konstruktori.
        /// </summary>
        public Lauta()
        {
            InitializeComponent();

            mustanVuoro = false;
        }

        /// <summary>
        /// Ottaa talteen nappulan, jonka kohdalla on painettu hiiren näppäintä.
        /// </summary>
        /// <param name="sender">Eventin laukaissut nappula</param>
        /// <param name="e">Sisältää tietoja eventistä</param>
        private void nappula_OnNappulaMouseDown(object sender, RoutedEventArgs e)
        {
            if (valittuNappula != null && poistettavaNappula == null)
            {
                poistettavaNappula = (Nappula.Nappula)sender;
                syoNappula(valittuNappula, poistettavaNappula);
                poistettavaNappula = null;
                valittuNappula = null;
            }
            else if (valittuNappula == null) valittuNappula = (Nappula.Nappula)sender;
        }

        /// <summary>
        /// Gridin latautumisen jälkeen luodaan sen pohjalle pelikenttä ja asetellaan siihen nappulat.
        /// </summary>
        /// <param name="sender">Eventin laukaissut grid</param>
        /// <param name="e">Sisältää tietoja eventistä</param>
        private void gridPelialue_Loaded(object sender, RoutedEventArgs e)
        {
            luoGrid();
            asetaNappulat();
        }

        /// <summary>
        /// Muodostaa pelikentän täyttämällä gridin labeleillä.
        /// </summary>
        private void luoGrid()
        {
            for (int gridinkoko = 0; gridinkoko < 8; gridinkoko++)
            {
                gridPelialue.ColumnDefinitions.Add(new ColumnDefinition());
                gridPelialue.RowDefinitions.Add(new RowDefinition());
            }

            for (int rivi = 0; rivi < gridPelialue.RowDefinitions.Count; rivi++)
            {
                for (int sarake = 0; sarake < gridPelialue.ColumnDefinitions.Count; sarake++)
                {
                    Label label = new Label();

                    label.VerticalAlignment = VerticalAlignment.Stretch;
                    label.HorizontalAlignment = HorizontalAlignment.Stretch;
                    label.BorderBrush = Brushes.Black;
                    label.BorderThickness = new Thickness(1, 1, 1, 1);
                    label.SetValue(Grid.ColumnProperty, sarake);
                    label.SetValue(Grid.RowProperty, rivi);
                    label.AllowDrop = true;
                    label.Height = 25;
                    label.Width = 25;

                    label.AddHandler(Label.MouseLeftButtonDownEvent, new RoutedEventHandler(label_Click));

                    gridPelialue.Children.Add(label);
                }
            }
        }

        /// <summary>
        /// Asettaa nappulat pelikentälle.
        /// </summary>
        /// <remarks>
        /// Aliohjelmassa on aika paljon toistoa, mutta toisaalta se on tässä tapauksessa kannattavaa 
        /// myöhempää laajentamista ajatellen.
        /// </remarks>
        private void asetaNappulat()
        {

            for (int rivi = 0; rivi < 2; rivi++)
            {
                for (int sarake = 0; sarake < gridPelialue.ColumnDefinitions.Count; sarake++)
                {
                    Nappula.Nappula nappula = new Nappula.Nappula();

                    nappula.SetValue(Grid.ColumnProperty, sarake);
                    nappula.SetValue(Grid.RowProperty, rivi);

                    nappula.Background = Brushes.Black;
                    nappula.Musta = true;
                    nappula.HorizontalAlignment = HorizontalAlignment.Stretch;
                    nappula.VerticalAlignment = VerticalAlignment.Stretch;

                    nappula.OnNappulaMouseDown += nappula_OnNappulaMouseDown; //

                    mustiaNappuloita++;
                    
                    gridPelialue.Children.Add(nappula);
                }
            }

            for (int rivi = 6; rivi < gridPelialue.RowDefinitions.Count; rivi++)
            {
                for (int sarake = 0; sarake < gridPelialue.ColumnDefinitions.Count; sarake++)
                {
                    Nappula.Nappula nappula = new Nappula.Nappula();

                    nappula.SetValue(Grid.ColumnProperty, sarake);
                    nappula.SetValue(Grid.RowProperty, rivi);

                    nappula.Background = Brushes.Red;
                    nappula.Musta = false;
                    nappula.HorizontalAlignment = HorizontalAlignment.Stretch;
                    nappula.VerticalAlignment = VerticalAlignment.Stretch;

                    nappula.OnNappulaMouseDown += nappula_OnNappulaMouseDown;

                    punaisiaNappuloita++;

                    gridPelialue.Children.Add(nappula);
                }
            }
        }

        /// <summary>
        /// Aliohjelma, jonka pääasiallinen tehtävä on hoitaa nappuloiden liikuttelu tyhjissä ruuduissa. Kutsutaan aina kun pelaaja klikkaa jotakin ruutua.
        /// </summary>
        /// <param name="sender">Eventin laukaissut ruutu(label)</param>
        /// <param name="e">Sisältää tietoja eventistä</param>
        private void label_Click(object sender, RoutedEventArgs e)
        {
            Label kohderuutu = sender as Label;
            Nappula.Nappula siirrettava = valittuNappula as Nappula.Nappula;

            try
            {
                siirrettavacolumn = Grid.GetColumn(siirrettava);
                siirrettavarow = Grid.GetRow(siirrettava);

                kohderuutucolumn = Grid.GetColumn(kohderuutu);
                kohderuuturow = Grid.GetRow(kohderuutu);
            }
            catch
            {
            }

            if (valittuNappula != null && peliOhi == false)
            {
                if (Math.Abs(siirrettavacolumn - kohderuutucolumn) < 2 && Math.Abs(siirrettavarow - kohderuuturow) < 2 && siirrettavarow < kohderuuturow && siirrettava.Musta == true && mustanVuoro == true)
                {
                    siirraNappulaa(siirrettava, kohderuutucolumn, kohderuuturow);

                    tarkistaVoitto(siirrettava);
                }

                if (Math.Abs(siirrettavacolumn - kohderuutucolumn) < 2 && Math.Abs(siirrettavarow - kohderuuturow) < 2 && siirrettavarow > kohderuuturow && siirrettava.Musta == false && mustanVuoro == false)
                {

                    siirraNappulaa(siirrettava, kohderuutucolumn, kohderuuturow);

                    tarkistaVoitto(siirrettava);
                }
            }
        }

        /// <summary>
        /// Siirtää nappulaa.
        /// </summary>
        /// <param name="siirrettava">Siirrettävä nappula</param>
        /// <param name="kohdecolumn">Kohderuudun sarake</param>
        /// <param name="kohderow">Kohderuudun rivi</param>
        private static void siirraNappulaa(Nappula.Nappula siirrettava, int kohdecolumn, int kohderow)
        {
            siirrettava.SetValue(Grid.ColumnProperty, kohdecolumn);
            siirrettava.SetValue(Grid.RowProperty, kohderow);

            if (mustanVuoro == true) mustanVuoro = false;
            else mustanVuoro = true;
            
            valittuNappula = null;
        }

        /// <summary>
        /// Toteuttaa nappulan syömisen.
        /// </summary>
        /// <param name="syoja">Liikutettava nappula</param>
        /// <param name="syotava">Syötävä nappula</param>
        private void syoNappula(Nappula.Nappula syoja, Nappula.Nappula syotava)
        {
            if ((syoja.Musta == true && syotava.Musta == false && mustanVuoro == true) || (syoja.Musta == false && syotava.Musta == true && mustanVuoro == false))
            {
                
                int syojacolumn = (int)syoja.GetValue(Grid.ColumnProperty);
                int syojarow = (int)syoja.GetValue(Grid.RowProperty);

                int syotavacolumn = (int)syotava.GetValue(Grid.ColumnProperty);
                int syotavarow = (int)syotava.GetValue(Grid.RowProperty);

                if (Math.Abs(syojacolumn - syotavacolumn) < 2 && Math.Abs(syojarow - syotavarow) < 2 && syojacolumn != syotavacolumn)
                {
                    if ((syoja.Musta == true && (syojarow - syotavarow) < 0) || (syoja.Musta == false && (syojarow - syotavarow) > 0))
                    {
                        gridPelialue.Children.Remove(syotava);

                        if (syotava.Musta == true) mustiaNappuloita--;
                        else punaisiaNappuloita--;

                        siirraNappulaa(syoja, syotavacolumn, syotavarow);

                        valittuNappula = null;
                    }
                }
            }
            tarkistaVoitto(syoja);
        }

        /// <summary>
        /// Tarkistaa voittoehtojen toteutumista.
        /// </summary>
        /// <param name="siirretty">Nappula, jonka siirtämisen jälkeen voittoehtojen toteutumisen tarkistus on tarpeellista.</param>
        private void tarkistaVoitto(Nappula.Nappula siirretty)
        {
            if (Grid.GetRow(siirretty) == 0 && siirretty.Musta == false)
            {
                siirretty.Background = Brushes.Green;
                Voitto();
            }

            if (Grid.GetRow(siirretty) == gridPelialue.RowDefinitions.Count - 1 && siirretty.Musta == true)
            {
                siirretty.Background = Brushes.Green;
                Voitto();
            }

            if (punaisiaNappuloita == 0 || mustiaNappuloita == 0)
            {
                siirretty.Background = Brushes.Green;
                Voitto();
            }
        }

        /// <summary>
        /// Aliohjelma, jossa käsitellään erilaiset voittoon liittyvät tapahtumat.
        /// </summary>
        /// <remarks>
        /// Voisi kenties olla event?
        /// </remarks>
        private static void Voitto()
        {
            peliOhi = true;
        }
    }
}
