using DaocLauncher.Helpers;
using DaocLauncher.Models;
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

namespace DaocLauncher;

partial class TextPrompt : Window, INotifyPropertyChanged
{
    public string LabelText { get; set; }
    public List<QuickSayShortcut> QuickShortcuts { get; set; }

    public TextPrompt()
    {
        LabelText = "Enter your command:";
        QuickShortcuts = GeneralHelpers.LoadQuickSayShortcutsFromDisk();
        InitializeComponent();
        this.DataContext = this;
        ResponseTextBox.Focus();
    }

    public TextPrompt(string labelText, string preloadText = "")
    {
        QuickShortcuts = GeneralHelpers.LoadQuickSayShortcutsFromDisk();
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

    public string ResponseText
    {
        get { return ResponseTextBox.Text; }
        set
        {
            if(ResponseTextBox.Text == value)
            {
                return;
            }
            ResponseTextBox.Text = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResponseText)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    private void ShortcutButtonClicked(object sender, RoutedEventArgs e)
    {
        if(e.Source != null)
        {
            var cut = (QuickSayShortcut)((Button)e.Source).DataContext;
            ResponseText = cut.Text;
            OKButton_Click(sender, e);
        }
    }
}
