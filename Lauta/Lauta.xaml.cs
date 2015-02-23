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
        private static bool nappulaValittu;
        private static bool mustanVuoro;
        private static bool peliOhi;


        private static int siirrettavacolumn;
        private static int siirrettavarow;
        private static int kohderuutucolumn;
        private static int kohderuuturow;


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
            if (valittuNappula == null)
            {
                valittuNappula = (Nappula.Nappula)sender; // 1. Nappula valitaan aktiiviseksi (click)
            }
            else
            {
                poistettavaNappula = (Nappula.Nappula)sender;
            }
            //5. Jos klikataan omaa nappulaa niin vaihdetaan aktiviseksi nappulaksi klikattu oma nappula eli palataan kohtaan 1.
            if (nappulaValittu == false)
            {
                nappulaValittu = true; // 2. Laitetaan talteen tieto aktiivisesti nappulasta
            }
            else
            {
                //3. Jos seuraavaksi klikataan vastustajan nappulaa niin tarkistetaan olisiko se tallessa olevalle nappulalle kelvollinen syötävä
                syoNappula(poistettavaNappula);
            }
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

                    nappula.OnNappulaMouseDown += nappula_OnNappulaMouseDown; //

                    gridPelialue.Children.Add(nappula);
                }
            }
        }

        /// <summary>
        /// Aliohjelma, jonka pääasiallinen tehtävä on hoitaa pelin sisäinen logiikka. Kutsutaan aina kun pelaaja klikkaa jotakin ruutua.
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
                    //4. Jos seuraavaksi klikataankin tyhjää ruutua niin tarkistetaan olisiko se tallessa olevalle nappulalle kelvollinen siirto
                    /*
                    if (poistettavaNappula != null && poistettavacolumn == kohderuutucolumn && poistettavarow == kohderuuturow)
                    {
                        gridPelialue.Children.Remove(poistettavaNappula);
                    }

                    siirrettava.SetValue(Grid.ColumnProperty, kohderuutucolumn); //4.1 Jos on niin siirretään nappula tähän ruutuun
                    siirrettava.SetValue(Grid.RowProperty, kohderuuturow);
                    */

                    siirraNappulaa(siirrettava, kohderuutucolumn, kohderuuturow);
                    
                    nappulaValittu = false;
                    
                    valittuNappula = null;
                    
                    mustanVuoro = false;

                    if (Grid.GetRow(siirrettava) == gridPelialue.RowDefinitions.Count - 1)
                    {
                        siirrettava.Background = Brushes.Green;
                        Voitto();
                    }
                }

                if (Math.Abs(siirrettavacolumn - kohderuutucolumn) < 2 && Math.Abs(siirrettavarow - kohderuuturow) < 2 && siirrettavarow > kohderuuturow && siirrettava.Musta == false && mustanVuoro == false)
                {

                    siirraNappulaa(siirrettava, kohderuutucolumn, kohderuuturow);
                    
                    nappulaValittu = false;

                    valittuNappula = null;

                    mustanVuoro = true;

                    if (Grid.GetRow(siirrettava) == 0)
                    {
                        siirrettava.Background = Brushes.Green;
                        Voitto();
                    }
                }
            }
            //4.1 Jos ei niin ei tehdä mitään

        }

        private static void siirraNappulaa(Nappula.Nappula siirrettava, int kohdecolumn, int kohderow)
        {
            siirrettava.SetValue(Grid.ColumnProperty, kohdecolumn); //4.1 Jos on niin siirretään nappula tähän ruutuun
            siirrettava.SetValue(Grid.RowProperty, kohderow);
        }

        private void syoNappula(Nappula.Nappula siirrettava)
        {
            int kohdecolumn = (int)poistettavaNappula.GetValue(Grid.ColumnProperty);
            int kohderow = (int)poistettavaNappula.GetValue(Grid.ColumnProperty);
            
            gridPelialue.Children.Remove(poistettavaNappula);
            
            siirraNappulaa(siirrettava, kohdecolumn, kohderow);
            //3.1 jos on niin tehdään syönti
            //3.2 jos ei niin ei tehdä mitään eli valittuna pysyy edelleen se oikea nappula
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
