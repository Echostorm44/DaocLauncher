using DaocLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class KeyPrompt : Window, INotifyPropertyChanged
    {
        string labelText;
        public string LabelText
        {
            get => labelText;
            set
            {
                if (labelText == value)
                {
                    return;
                }

                labelText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelText)));
            }
        }
        public char? TargetCharacter { get; set; }
        public Key KeyResult { get; set; }
        public KeyModifier KeyModifier { get; set; }

        public KeyPrompt()
        {
            LabelText = "Press your / key:";
            TargetCharacter = '/';
            InitializeComponent();
            this.DataContext = this;
            ResponseTextBox.Focus();
        }

        public KeyPrompt(string labelText,  char? targetCharacter)
        {
            LabelText = labelText;
            TargetCharacter = targetCharacter;
            InitializeComponent();
            this.DataContext = this;
            ResponseTextBox.Focus();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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

        private void ResponseTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (TargetCharacter == null)
            {                
                return;
            }
            if (!string.IsNullOrEmpty(ResponseTextBox.Text) && TargetCharacter.HasValue && ResponseTextBox.Text.ToCharArray()[0] == TargetCharacter)
            {   
                DialogResult = true;
            }
            else
            {
                ResponseTextBox.Text = "";
            }
        }

        private void ResponseTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TargetCharacter == null)
            {
                e.Handled = true;
            }
            else
            {
                return;            
            }
            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None &&
                (key == Key.Delete || key == Key.Back || key == Key.Escape))
            {
                return;
            }

            // If no actual key was pressed - return
            if (key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear ||
                key == Key.OemClear ||
                key == Key.Apps)
            {
                return;
            }

            KeyResult = key;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                KeyModifier = KeyModifier.Shift;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                KeyModifier = KeyModifier.Ctrl;
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                KeyModifier = KeyModifier.Alt;
            }
            else
            {
                KeyModifier = KeyModifier.None;
            }

            if (TargetCharacter == null)
            {
                LabelText = KeyResult.ToString() + " + " + KeyModifier.ToString();
                btnOK.IsEnabled = true;
                btnTryAgain.IsEnabled = true;
                ResponseTextBox.Text = "";
                return;
            }
        }
    }
}
