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

namespace Nappula
{
    /// <summary>
    /// Interaction logic for Nappula.xaml
    /// </summary>
    public partial class Nappula : UserControl
    {
        private RadioButton _Nappula;
        private bool musta;

        /// <summary>
        /// Get- ja set-metodit puolen tunnistamista varten.
        /// </summary>
        public bool Musta
        {
            get { return musta; }
            set { musta = value; }
        }

        /// <summary>
        /// Konstruktori.
        /// </summary>
        public Nappula()
        {
            InitializeComponent();

        }

        public static readonly RoutedEvent OnNappulaMouseDownEvent =
            EventManager.RegisterRoutedEvent("OnNappulaMouseDown", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Nappula)); //

        /// <summary>
        /// Muokataan templatea, jotta ulkopuolelta päästään suoraan koko kontrolliin kiinni.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Nappula = FindName("radioButtonNappula") as RadioButton;

            if (_Nappula != null)
            {
                _Nappula.PreviewMouseDown += NappulaMouseDown; //
            }
        }

        /// <summary>
        /// Tapahtumankäsittelijä painallusta varten.
        /// </summary>
        public event RoutedEventHandler OnNappulaMouseDown
        {
            add { AddHandler(OnNappulaMouseDownEvent, value); }
            remove { RemoveHandler(OnNappulaMouseDownEvent, value); }
        }

        /// <summary>
        /// Painallustapahtuma.
        /// </summary>
        /// <param name="sender">Tapahtuman aktivoinut objecti</param>
        /// <param name="e">Tietoja tapahtumasta</param>
        private void NappulaMouseDown(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnNappulaMouseDownEvent));
        }
    }
}
