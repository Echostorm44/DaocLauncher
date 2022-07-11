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

namespace DaocLauncher
{
    /// <summary>
    /// Interaction logic for KeyPrompt.xaml
    /// </summary>
    public partial class KeyPrompt : Window
    {
        public string LabelText { get; set; }
        public char TargetCharacter { get; set; }
        public Key KeyResult { get; set; }

        public KeyPrompt()
        {
            LabelText = "Press your / key:";
            TargetCharacter = '/';
            InitializeComponent();
            this.DataContext = this;
            ResponseTextBox.Focus();
        }

        public KeyPrompt(string labelText,  char targetCharacter)
        {
            LabelText = labelText;
            TargetCharacter = targetCharacter;
            InitializeComponent();
            this.DataContext = this;
            ResponseTextBox.Focus();
        }

        private void btnTryAgain_Click(object sender, RoutedEventArgs e)
        {
            btnTryAgain.IsEnabled = false;
            btnOK.IsEnabled = false;
            ResponseTextBox.Text = "";
            ResponseTextBox.IsEnabled = true;            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ResponseTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResponseTextBox.Text) && ResponseTextBox.Text.ToCharArray()[0] == TargetCharacter)
            {
                KeyResult = e.Key;
                ResponseTextBox.IsEnabled = false;
                btnOK.IsEnabled = true;
                btnTryAgain.IsEnabled = true;
            }
            else
            {
                ResponseTextBox.Text = "";
            }
        }
    }
}
