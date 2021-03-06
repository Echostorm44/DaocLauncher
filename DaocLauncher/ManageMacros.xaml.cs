using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class ManageMacros : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        MacroSet? currentSet;
        public MacroSet? CurrentSet { get => currentSet; set { currentSet = value; OnPropertyChanged(); } }

        public ObservableCollection<string> MacroSetNames { get; set; }
        public ObservableCollection<MacroSet> MacroSets { get; set; }
        public List<string> GroupCategories { get; set; }        

        public ManageMacros()
        {
            MacroSets = new ObservableCollection<MacroSet>();
            GroupCategories = GeneralHelpers.LoadMacroGroupCategoriesListFromDisk();
            MacroSetNames = new ObservableCollection<string>();
            GeneralHelpers.LoadMacroSetsFromDisk().ForEach(a => MacroSets.Add(a));
            MacroSets.ToList().ForEach(b => MacroSetNames.Add(b.Name));
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
            prompt.Owner = Application.Current.MainWindow; 
            if (prompt.ShowDialog() == true)
            {
                name = prompt.ResponseText;
                if (!string.IsNullOrEmpty(name))
                {
                    if (MacroSets.Any(a => a.Name.ToLower() == name.ToLower()))
                    {
                        MessageBox.Show("There is already a set with that name, try again.");
                        return;
                    }
                }
            }
            else
            {
                return;
            }
            MacroSet set = new MacroSet(name, new Dictionary<string, ObservableCollection<string>>(),
                        new Dictionary<HotKey, ObservableCollection<HotKeyAction>>());
            set.CategoryGroups.Add(GroupCategories.First(), new ObservableCollection<string>() { "CharacterName" });
            // set some defaults to deal with chat box being active to pause macros

            set.HotKeyCollection.Add(new HotKey(Key.Enter, KeyModifier.None, "Toggle hotkeys on entering and leaving chat"),
                new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.ToggleAllHotkeysOnOff) });
            set.HotKeyCollection.Add(new HotKey(Key.Escape, KeyModifier.None, "Enable hotkeys in case leaving chat"),
                new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.EnableAllHotkeys) });
            
            set.HotKeyCollection.Add(new HotKey(Key.D2, KeyModifier.None, "PBAOE Nuke"),
                new ObservableCollection<HotKeyAction>() 
                { 
                    new HotKeyAction("PBAOE", null, VirtualKeyCode.VK_2, null, null, HotkeyActionType.GroupKeyCommand),
                    new HotKeyAction("Melee", null, null, null, null, HotkeyActionType.AssistActiveWindow),
                    new HotKeyAction("Blocker", null, null, null, null, HotkeyActionType.AssistActiveWindow),
                    new HotKeyAction(null, 150, null, null, null, HotkeyActionType.PauseScript),
                    new HotKeyAction("Melee", null, null, null, "/stick", HotkeyActionType.SlashCommand),
                    new HotKeyAction("Melee", null, VirtualKeyCode.VK_4, VirtualKeyCode.ALT, null, HotkeyActionType.GroupKeyCommand),
                    new HotKeyAction("Blocker", null, null, null, "/face", HotkeyActionType.SlashCommand),
                });

            set.HotKeyCollection.Add(new HotKey(Key.R, KeyModifier.None, "Disable hotkeys for chat reply"),
                new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.DisableAllHotkeys) });

            //KeyPrompt keyPrompt = new KeyPrompt("Press your / key", '/');  // Add one for reply as well since key isn't hard coded
            //if (keyPrompt.ShowDialog() == true)
            //{
            //    set.HotKeyCollection.Add(new HotKey(keyPrompt.KeyResult, KeyModifier.None, "Disable hotkeys while typing in chat"),
            //    new List<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.Disable) });
            //}
            //keyPrompt = new KeyPrompt("Press your ' key", "'".ToCharArray()[0]);
            //if (keyPrompt.ShowDialog() == true)
            //{
            //    set.HotKeyCollection.Add(new HotKey(keyPrompt.KeyResult, KeyModifier.None, "Disable hotkeys while typing in chat"),
            //    new List<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.Disable) });
            //}
            MacroSets.Add(set);
            //GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
            CurrentSet = set;
            MacroSetNames.Add(name);
            ddlExistingSets.SelectedValue = name;
        }

        private void btnDeleteMacroSet_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the macro set?", "Really Delete Set?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (ddlExistingSets.SelectedValue == null)
                {
                    return;
                }
                CurrentSet = null;
                string setName = ddlExistingSets.SelectedValue.ToString();
                MacroSets.Remove(MacroSets.Single(a => a.Name == setName));
                MacroSetNames.Remove(setName);
                ddlExistingSets.SelectedValue = null;
                //GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());                
            }
        }

        private void ddlExistingSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddlExistingSets.SelectedValue != null)
            {
                btnDeleteMacroSet.Visibility = Visibility.Visible;
                CurrentSet = MacroSets.Single(a => a.Name == ddlExistingSets.SelectedValue.ToString());
            }
            else
            {
                btnDeleteMacroSet.Visibility = Visibility.Hidden;
                CurrentSet = null;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void lstHotkeys_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {            
            dynamic roo = lstHotkeys.SelectedItem;            
            UpsertHotkey hotWindow = new UpsertHotkey(roo.Key, roo.Value, GroupCategories);
            hotWindow.Owner = Application.Current.MainWindow;
            hotWindow.ShowDialog();

            if (CurrentSet.HotKeyCollection.ContainsKey(hotWindow.TheHotKey))
            {
                CurrentSet.HotKeyCollection[hotWindow.TheHotKey] = hotWindow.AllActions;
                MacroSets.Single(a => a.Name == CurrentSet.Name).HotKeyCollection[hotWindow.TheHotKey] = hotWindow.AllActions;
            }
            else
            {
                CurrentSet.HotKeyCollection.Add(hotWindow.TheHotKey, hotWindow.AllActions);
                MacroSets.Single(a => a.Name == CurrentSet.Name).HotKeyCollection.Add(hotWindow.TheHotKey, hotWindow.AllActions);
            }
            GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
        }
    }
}
