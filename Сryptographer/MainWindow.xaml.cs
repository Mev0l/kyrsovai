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
using Сryptographer.models;

namespace Сryptographer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyInitialize();
        }

        private Cryptographer cryptographer = new();
        private void MyInitialize() 
        {
            this.DataContext = cryptographer;
            
        }

        public void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
         
            



        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RightComboBox == null || leftComboBox == null) return;
            if (((ComboBox)sender).Name == "RightComboBox")
            {
                if (RightComboBox.SelectedIndex == 0) { leftComboBox.SelectedIndex = 1; cryptographer.isLeftOriginal = false; }
                if (RightComboBox.SelectedIndex == 1) { leftComboBox.SelectedIndex = 0; cryptographer.isLeftOriginal = true; }
            }
            else
            {
                if (leftComboBox.SelectedIndex == 0) { RightComboBox.SelectedIndex = 1; cryptographer.isLeftOriginal = true; }
                if (leftComboBox.SelectedIndex == 1) { RightComboBox.SelectedIndex = 0; cryptographer.isLeftOriginal = false; }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (leftComboBox.SelectedIndex)
            {
                case 0:
                    leftComboBox.SelectedIndex = 1;
                    break;
                case 1:
                    leftComboBox.SelectedIndex = 0;
                    break;
            }
            cryptographer.SwitchTexts();
        }

        private void cypherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (shiftSelect != null)
            switch (cryptographer.CryptoMode) 
            {
                case 0:
                    shiftSelect.IsEnabled = true;
                        AESSettings.IsEnabled = false;
                    break;
                case 1:
                    shiftSelect.IsEnabled = false;
                        AESSettings.IsEnabled = true;
                    break;
            }



        }

        private void ButtonRandom_Click(object sender, RoutedEventArgs e)
        {
            cryptographer.AESGenerateRandomKey();
        }
    }
}
