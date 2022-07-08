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
    /// Interaction logic for ManageMacros.xaml
    /// </summary>
    public partial class ManageMacros : UserControl
    {
        public ObservableCollection<string> MacroSetNames { get; set; }
        public List<MacroSet> MacroSets { get; set; }
        public ManageMacros()
        {
            MacroSetNames = new ObservableCollection<string>();
            MacroSets = GeneralHelpers.LoadMacroSetsFromDisk();
            MacroSets.ForEach(b => MacroSetNames.Add(b.Name));
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnEditMacroSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewMacroSet_Click(object sender, RoutedEventArgs e)
        {
            var name = "";
            var prompt = new TextPrompt("Enter a name for the new macro set:");
            if (prompt.ShowDialog() == true)
            {
                name = prompt.ResponseText;
                if (!string.IsNullOrEmpty(name))
                {
                    MacroSetNames.Add(name);
                    MacroSet set = new MacroSet(name, new Dictionary<string, List<string>>(),
                        new Dictionary<HotKey, List<HotKeyAction>>());
                    // set some defaults to deal with chat box being active to pause macros

                }
            }
        }

        private void btnDeleteMacroSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ddlExistingSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
