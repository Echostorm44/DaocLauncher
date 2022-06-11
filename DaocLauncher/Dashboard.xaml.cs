using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DaocLauncher
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public List<DaocCharacter> AllCharacters { get; set; }
        public List<Server> Servers { get; set; }
        public List<string> ServerNames { get; set; }
        public List<DaocAccount> Accounts { get; set; }
        public ICommand LaunchChar { get; private set; }
        public ICommand LaunchMyAcct { get; private set; }
        public Dashboard()
        {
            AllCharacters = GeneralHelpers.LoadCharactersFromDisk() ?? new List<DaocCharacter>();
            Servers = GeneralHelpers.LoadServerListFromDisk()?.Servers ?? new List<Server>();
            ServerNames = Servers.Select(a => a.Name).ToList();
            Accounts = GeneralHelpers.LoadAccountListFromDisk()?.MyAccounts ?? new List<DaocAccount>();
            LaunchChar = new RelayCommand(a => LaunchCharClicked(a));
            LaunchMyAcct = new RelayCommand(a => LaunchMyAccountClicked(a));
            InitializeComponent();            
            this.DataContext = this;    
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    Game game = button.DataContext as Game;
        //    int id = game.ID;
        //    // ...
        //}

        void Test(object param)
        {
            MessageBox.Show("HA!");
        }

        void LaunchCharClicked(object choice)
        {            
            if (choice == null)
            {
                return;
            }
            var selectedChar = (DaocCharacter)choice;
            if (string.IsNullOrEmpty(selectedChar.ParentAccountName) || string.IsNullOrEmpty(selectedChar.Name) || 
                string.IsNullOrEmpty(selectedChar.Server) || string.IsNullOrEmpty(selectedChar.Class))
            {
                return;
            }
            var accountData = Accounts.FirstOrDefault(a => a.Name == selectedChar.ParentAccountName);
            if (accountData == null)
            {
                return;
            }
            var serverData = Servers.FirstOrDefault(a => a.Name == selectedChar.Server);
            if (serverData == null)
            {
                return;
            }
            var classLookup = GeneralHelpers.GetAllCharacterClasses();
            if (!classLookup.ContainsKey(selectedChar.Class))
            {
                return;
            }
            var realmNumber = classLookup[selectedChar.Class];
            GeneralHelpers.LaunchDaoc(selectedChar.Name, accountData.Name, accountData.Password, serverData.IP, serverData.ID, selectedChar.Name, realmNumber);
        }

        void LaunchMyAccountClicked(object choice)
        {
            if (choice == null)
            {
                return;
            }
            var selectedAccount = (DaocAccount)choice;
            if (string.IsNullOrEmpty(selectedAccount.DefaultServer))
            {
                return;
            }
            var serverData = Servers.FirstOrDefault(a => a.Name == selectedAccount.DefaultServer);
            if (serverData == null)
            {
                return;
            }
            GeneralHelpers.SaveAccountListToDisk(Accounts);// Save the default server if it changed
            GeneralHelpers.LaunchDaoc(selectedAccount.DefaultTag, selectedAccount.Name, selectedAccount.Password, serverData.IP, serverData.ID, "", "");
        }
    }
}
