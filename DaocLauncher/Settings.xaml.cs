using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
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

namespace DaocLauncher;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : UserControl, INotifyPropertyChanged
{
    string dllPath;
    public string DllPath
    {
        get => dllPath;
        set
        {
            if(dllPath == value)
            {
                return;
            }

            dllPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DllPath)));
        }
    }
    string userDataPath;
    public string UserDataPath
    {
        get => userDataPath;
        set
        {
            if(userDataPath == value)
            {
                return;
            }

            userDataPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserDataPath)));
        }
    }
    string symLinkPath;
    public string SymLinkPath
    {
        get => symLinkPath;
        set
        {
            if(symLinkPath == value)
            {
                return;
            }

            symLinkPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SymLinkPath)));
        }
    }
    string wackKeyMap;
    public string WackKeyMap
    {
        get => wackKeyMap;
        set
        {
            if(wackKeyMap == value)
            {
                return;
            }

            wackKeyMap = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WackKeyMap)));
        }
    }
    string singleQuoteKeyMap;
    public string SingleQuoteKeyMap
    {
        get => singleQuoteKeyMap;
        set
        {
            if(singleQuoteKeyMap == value)
            {
                return;
            }

            singleQuoteKeyMap = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SingleQuoteKeyMap)));
        }
    }

    public Settings()
    {
        var genSets = GeneralHelpers.LoadGeneralSettingsFromDisk();
        DllPath = genSets.PathToGameDll;
        UserDataPath = genSets.PathToUserSettings;
        SymLinkPath = genSets.PathToSymbolicLinks;
        WackKeyMap = genSets.WackKey?.ToString() ?? "Not Set";
        SingleQuoteKeyMap = genSets.SingleQuoteKey?.ToString() ?? "Not Set";
        this.DataContext = this;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetDllPathButtonClicked(object sender, RoutedEventArgs e)
    {
        do
        {
            MessageBox.Show("Please select the path to your game.dll file");
            var foo = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, Filter = "All|game.dll" };
            var gameLoc = foo.ShowDialog();
            if(gameLoc.HasValue)
            {
                MainWindow.GenSettings!.PathToGameDll = foo.FileName;
                DllPath = foo.FileName;
                GeneralHelpers.SaveGeneralSettingsToDisk(MainWindow.GenSettings);
            }
        }
        while (string.IsNullOrEmpty(DllPath));
    }

    private void SetUserDataPathButtonClicked(object sender, RoutedEventArgs e)
    {
        do
        {
            MessageBox.Show(@"Please select the path to your DAOC user Settings.  It should be something like:\r\nc:\Users\username\AppData\Roaming\Electronic Arts\Dark Age Of Camelot\LotM");
            var goo = new System.Windows.Forms.FolderBrowserDialog();
            var userFolderDialog = goo.ShowDialog();
            if(userFolderDialog == System.Windows.Forms.DialogResult.OK)
            {
                MainWindow.GenSettings!.PathToUserSettings = goo.SelectedPath;
                UserDataPath = goo.SelectedPath;
                GeneralHelpers.SaveGeneralSettingsToDisk(MainWindow.GenSettings);
            }
        }
        while (string.IsNullOrEmpty(UserDataPath));
    }

    private void SetSymLinkPathButtonClicked(object sender, RoutedEventArgs e)
    {
        do
        {
            MessageBox.Show("Please select the path to a folder to store symlinks & settings");
            var foo = new System.Windows.Forms.FolderBrowserDialog();
            var gameLoc = foo.ShowDialog();
            if(gameLoc == System.Windows.Forms.DialogResult.OK)
            {
                MainWindow.GenSettings!.PathToSymbolicLinks = foo.SelectedPath;
                SymLinkPath = foo.SelectedPath;
                GeneralHelpers.SaveGeneralSettingsToDisk(MainWindow.GenSettings);
            }
        }
        while (string.IsNullOrEmpty(SymLinkPath));
    }

    private void SetWackKeyButtonClicked(object sender, RoutedEventArgs e)
    {
        var keyPrompt = new KeyPrompt("Press your / key", '/');
        if(keyPrompt.ShowDialog() == true)
        {
            MainWindow.GenSettings!.WackKey = keyPrompt.KeyResult;
            WackKeyMap = keyPrompt.KeyResult.ToString();
            GeneralHelpers.SaveGeneralSettingsToDisk(MainWindow.GenSettings);
        }
    }

    private void SetSingleQuoteButtonClicked(object sender, RoutedEventArgs e)
    {
        var keyPrompt = new KeyPrompt("Press your ' key", "'".ToCharArray()[0]);
        if(keyPrompt.ShowDialog() == true)
        {
            MainWindow.GenSettings!.SingleQuoteKey = keyPrompt.KeyResult;
            SingleQuoteKeyMap = keyPrompt.KeyResult.ToString();
            GeneralHelpers.SaveGeneralSettingsToDisk(MainWindow.GenSettings);
        }
    }
}
