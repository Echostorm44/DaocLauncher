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

namespace DaocLauncher;

partial class TextPrompt : Window
{
    public string LabelText { get; set; }

    public TextPrompt()
    {
        LabelText = "Enter your command:";
        InitializeComponent();
        this.DataContext = this;
        ResponseTextBox.Focus();
    }

    public TextPrompt(string labelText, string preloadText = "")
    {
        LabelText = labelText;
        InitializeComponent();
        if(!string.IsNullOrEmpty(preloadText))
        {
            ResponseText = preloadText;
        }
        this.DataContext = this;
        this.Activate();
        ResponseTextBox.Focus();
        ResponseTextBox.CaretIndex = ResponseTextBox.Text.Length;
    }

    public string ResponseText { get { return ResponseTextBox.Text; } set { ResponseTextBox.Text = value; } }

    private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void ResponseTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if(e.Key == Key.Return)
        {
            OKButton_Click(sender, e);
        }
    }
}
