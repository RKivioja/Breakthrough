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

namespace Breakthrough
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Konstruktori.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Poistuu ohjelmasta.
        /// </summary>
        /// <param name="sender">Tapahtuman aktivoinut objekti</param>
        /// <param name="e">Sisältää tietoja tapahtumasta</param>
        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Avaa About-ikkunan.
        /// </summary>
        /// <param name="sender">Tapahtuman aktivoinut objekti</param>
        /// <param name="e">Sisältää tietoja tapahtumasta</param>
        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBoxBreakthrough aboutlomake = new AboutBoxBreakthrough();
            aboutlomake.ShowDialog();
        }

        /// <summary>
        /// Poistaa vanhan pelin ja luo uuden.
        /// </summary>
        /// <param name="sender">Tapahtuman aktivoinut objekti</param>
        /// <param name="e">Sisältää tietoja tapahtumasta</param>
        private void buttonUusiPeli_Click(object sender, RoutedEventArgs e)
        {
            Lauta.Lauta lautapeliuusi = new Lauta.Lauta();
            
            lautapeliuusi.HorizontalAlignment = HorizontalAlignment.Center; 
            lautapeliuusi.VerticalAlignment = VerticalAlignment.Center; 
            lautapeliuusi.Margin = new Thickness(40);
            lautapeliuusi.Name = "lautaPelialue";


            gridPelialue.Children.RemoveAt(0);
            gridPelialue.Children.Add(lautapeliuusi);
        }

        /// <summary>
        /// Avaa ohjelman avustuksen.
        /// </summary>
        /// <param name="sender">Tapahtuman aktivoinut objekti</param>
        /// <param name="e">Sisältää tietoja tapahtumasta</param>
        private void menuItemWebHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://users.jyu.fi/~rosakivi/GKO.html");
        }
    }
}
