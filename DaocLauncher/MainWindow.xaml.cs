using DaocLauncher.Helpers;
using DaocLauncher.Models;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
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

namespace DaocLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static GeneralSettings? GenSettings { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            GenSettings = GeneralHelpers.LoadGeneralSettingsFromDisk();
            if(GenSettings.IsFirstTime)
            {
                MessageBox.Show("Please select the path to your game.dll file");
                var foo = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, Filter = "All|game.dll" };
                var gameLocDialog = foo.ShowDialog();
                if(gameLocDialog.HasValue)
                {
                    GenSettings.PathToGameDll = foo.FileName;
                }
                MessageBox.Show("Please select the path to a folder to store symlinks & settings");
                var loo = new System.Windows.Forms.FolderBrowserDialog();
                var symFolderDialog = loo.ShowDialog();
                if(symFolderDialog == System.Windows.Forms.DialogResult.OK)
                {
                    GenSettings.PathToSymbolicLinks = loo.SelectedPath;
                }
                MessageBox.Show(@"Please select the path to your DAOC user Settings.  It should be something like:\r\nc:\Users\username\AppData\Roaming\Electronic Arts\Dark Age Of Camelot\LotM");
                var goo = new System.Windows.Forms.FolderBrowserDialog();
                var userFolderDialog = goo.ShowDialog();
                if(userFolderDialog == System.Windows.Forms.DialogResult.OK)
                {
                    GenSettings.PathToUserSettings = goo.SelectedPath;
                }

                GenSettings.IsFirstTime = false;
                GeneralHelpers.SaveGeneralSettingsToDisk(GenSettings);
            }
            // Make sure we've got the game path
            while(string.IsNullOrEmpty(GenSettings.PathToGameDll))
            {
                MessageBox.Show("Please select the path to your game.dll file");
                var foo = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, Filter = "All|game.dll" };
                var gameLoc = foo.ShowDialog();
                if(gameLoc.HasValue)
                {
                    GenSettings.PathToGameDll = foo.FileName;
                    GeneralHelpers.SaveGeneralSettingsToDisk(GenSettings);
                }
            }
            while(string.IsNullOrEmpty(GenSettings.PathToSymbolicLinks))
            {
                MessageBox.Show("Please select the path to a folder to store symlinks & settings");
                var foo = new System.Windows.Forms.FolderBrowserDialog();
                var gameLoc = foo.ShowDialog();
                if(gameLoc == System.Windows.Forms.DialogResult.OK)
                {
                    GenSettings.PathToGameDll = foo.SelectedPath;
                    GeneralHelpers.SaveGeneralSettingsToDisk(GenSettings);
                }
            }
            while(string.IsNullOrEmpty(GenSettings.PathToUserSettings))
            {
                MessageBox.Show(@"Please select the path to your DAOC user Settings.  It should be something like:\r\nc:\Users\username\AppData\Roaming\Electronic Arts\Dark Age Of Camelot\LotM");
                var goo = new System.Windows.Forms.FolderBrowserDialog();
                var userFolderDialog = goo.ShowDialog();
                if(userFolderDialog == System.Windows.Forms.DialogResult.OK)
                {
                    GenSettings.PathToUserSettings = goo.SelectedPath;
                    GeneralHelpers.SaveGeneralSettingsToDisk(GenSettings);
                }
            }
        }

        private void ClickedEditAccounts(object sender, RoutedEventArgs e)
        {
            var targetUC = new EditAccountsControl();
            mainContent.Content = targetUC;
        }

        private void ClickedEditCharacters(object sender, RoutedEventArgs e)
        {
            var targetUC = new EditCharacters();
            mainContent.Content = targetUC;
        }

        private void ClickedHome(object sender, RoutedEventArgs e)
        {
            var targetUC = new Dashboard(mmenu, this);
            mainContent.Content = targetUC;
        }

        private void ClickedMacros(object sender, RoutedEventArgs e)
        {
            var targetUC = new ManageMacros();
            mainContent.Content = targetUC;
        }

        private void ClickedQuit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ClickedViewGithub(object sender, RoutedEventArgs e)
        {
            var myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = "https://github.com/Echostorm44/DaocLauncher";
            myProcess.Start();
        }

        private void ClickedViewHelp(object sender, RoutedEventArgs e)
        {
            var myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = "https://github.com/Echostorm44/DaocLauncher/wiki";
            myProcess.Start();
        }

        private void ClickedAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DAOC Launcher by Echostorm (c) 2022");
        }

        private void ClickedImportToons(object sender, RoutedEventArgs e)
        {
            var targetUC = new ImportToons();
            mainContent.Content = targetUC;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var targetUC = new Dashboard(mmenu, this);
            mainContent.Content = targetUC;
        }
    }
}
