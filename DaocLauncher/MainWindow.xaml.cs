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

        private void ClickedMacros(object sender, RoutedEventArgs e)
        {
            var soo = (uint)VirtualKeyCode.VK_A;
            var aoo = (uint)VirtualKeyCode.VK_A;
            var loo = (uint)'A';
            UInt16 eoo = 'A';
            var rooo = "";
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {

            var roo = (uint)e.Key;
            var foo = (uint)Key.D2;
            var soo = (uint)VirtualKeyCode.VK_2;
            var yoo = e.KeyStates;
        }

        HotKey hotKey;
        private void ClickedAbout(object sender, RoutedEventArgs e)
        {
            hotKey = new HotKey(Key.F9, KeyModifier.None);
            hotKey.Register(OnHotKeyHandler);

        }
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private void OnHotKeyHandler(HotKey hotKey)
        {
            IntPtr WindowToFind = FindWindow("DAoCMWC", "Buddyblocker");
            IntPtr WindowToReturnTo = FindWindow("DAoCMWC", "Elliiee");
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            SendKeysTo keySender = new SendKeysTo();
            var dialog = new TextPrompt();
            if (dialog.ShowDialog() == true)
            {
                keySender.SendChatCommand(WindowToFind, dialog.ResponseText, WindowToReturnTo);
            }


            //keySender.SendThoseKeysSucka(WindowToFind, VirtualKeyCode.VK_1, null, WindowToReturnTo);
            //keySender.SendChatCommand(WindowToFind, "/assist frickk", WindowToReturnTo);
            //Thread.Sleep(2000);
            //keySender.SendChatCommand(WindowToFind, "/wave", WindowToReturnTo);

            //keySender.SendThoseKeysSucka(WindowToFind, VirtualKeyCode.RETURN, null, WindowToReturnTo);
            //keySender.SendThoseKeysSucka(WindowToFind, "/assist frickk", WindowToReturnTo);
            //keySender.SendThoseKeysSucka(WindowToFind, VirtualKeyCode.RETURN, null, WindowToReturnTo);
            //keySender.SendThoseKeysSucka(WindowToFind, VirtualKeyCode.VK_3, VirtualKeyCode.SHIFT, WindowToReturnTo);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var targetUC = new Dashboard();
            mainContent.Content = targetUC;
        }
    }
}
