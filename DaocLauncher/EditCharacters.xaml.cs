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
    /// Interaction logic for EditCharacters.xaml
    /// </summary>
    public partial class EditCharacters : UserControl
    {
        public ObservableCollection<DaocCharacter> AllCharacters { get; set; }
        public ObservableCollection<string> Servers { get; set; }
        public ObservableCollection<string> AccountNames { get; set; }
        public ObservableCollection<string> CharacterClasses { get; set; }
        Helpers.Debouncer debouncer = new Debouncer(500);

        public EditCharacters()
        {
            InitializeComponent();
            var classList = GeneralHelpers.GetAllCharacterClasses();
            CharacterClasses = new ObservableCollection<string>();
            foreach (var item in classList)
            {
                CharacterClasses.Add(item.Key);
            }

            var characterList = GeneralHelpers.LoadCharactersFromDisk();
            AllCharacters = new ObservableCollection<DaocCharacter>();
            foreach (var item in characterList)
            {
                AllCharacters.Add(item);
            }
            var serverList = GeneralHelpers.LoadServerListFromDisk();
            Servers = new ObservableCollection<string>();
            foreach (var item in serverList.Servers)
            {
                Servers.Add(item.Name);
            }
            var accountList = GeneralHelpers.LoadAccountListFromDisk();
            AccountNames = new ObservableCollection<string>();
            foreach (var item in accountList.MyAccounts)
            {
                AccountNames.Add(item.Name ?? "NA");
            }

            this.DataContext = this;
        }
        
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            debouncer.Debounce(() =>
            {
                GeneralHelpers.SaveCharactersToDisk(AllCharacters.ToList());
            });
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridChars.SelectedItem != null)
            {
                AllCharacters.Remove((DaocCharacter)gridChars.SelectedItem);
                GeneralHelpers.SaveCharactersToDisk(AllCharacters.ToList());
            }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grid = (DataGrid)sender;
                grid.BeginEdit(e);
            }
        }
    }
}
