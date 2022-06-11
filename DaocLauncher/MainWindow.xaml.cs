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
            // Make sure we've got the game path
            while (!File.Exists(GenSettings.PathToGameDll)) 
            { 
                var foo = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, Filter = "All|game.dll" };
                var gameLoc = foo.ShowDialog();
                if (gameLoc.HasValue)
                {
                    GenSettings.PathToGameDll = foo.FileName;
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
            var targetUC = new Dashboard();
            mainContent.Content = targetUC;
        }

        private void ClickedAbout(object sender, RoutedEventArgs e)
        {
            MutantHunter hunt = new MutantHunter();
            var results = hunt.KillMutants();
            MessageBox.Show(string.Join(",", results));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var targetUC = new Dashboard();
            mainContent.Content = targetUC;
        }
    }
    //        KeyGesture OpenCmdKeyGesture = new KeyGesture(
    //Key.B,
    //ModifierKeys.Control);

    //        ApplicationCommands.Open.InputGestures.Add(OpenCmdKeyGesture); 
}
