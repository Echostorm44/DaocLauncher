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
        public MacroSet? CurrentSet
        {
            get => currentSet;
            set
            {
                currentSet = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> MacroSetNames { get; set; }
        public ObservableCollection<MacroSet> MacroSets { get; set; }
        public List<string> GroupCategories { get; set; }

        public ManageMacros()
        {
            if(MainWindow.GenSettings != null && (MainWindow.GenSettings.WackKey == null || MainWindow.GenSettings.SingleQuoteKey == null 
                || MainWindow.GenSettings.WackKey == Key.None || MainWindow.GenSettings.SingleQuoteKey == Key.None))
            {
                // These two keys are hardcoded to open your chat box but they can be different on different keyboards so I want to get them before we start
                // making macros so that I know when to pause listening so you can type in chat without setting off a bunch of nukes && heals
                KeyPrompt keyPrompt = new KeyPrompt("Press your / key", '/');
                if(keyPrompt.ShowDialog() == true)
                {
                    MainWindow.GenSettings.WackKey = keyPrompt.KeyResult;
                }
                keyPrompt = new KeyPrompt("Press your ' key", "'".ToCharArray()[0]);
                if(keyPrompt.ShowDialog() == true)
                {
                    MainWindow.GenSettings.SingleQuoteKey = keyPrompt.KeyResult;
                }
                GeneralHelpers.SaveGeneralSettingsToDisk(MainWindow.GenSettings);
            }
            MacroSets = new ObservableCollection<MacroSet>();
            GroupCategories = GeneralHelpers.LoadMacroGroupCategoriesListFromDisk();
            MacroSetNames = new ObservableCollection<string>();
            GeneralHelpers.LoadMacroSetsFromDisk().ForEach(a => MacroSets.Add(a));
            MacroSets.ToList().ForEach(b => MacroSetNames.Add(b.Name));
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnNewMacroSet_Click(object sender, RoutedEventArgs e)
        {
            var name = "";
            var prompt = new TextPrompt("Enter a name for the new macro set:");
            prompt.Owner = Application.Current.MainWindow;
            if(prompt.ShowDialog() == true)
            {
                name = prompt.ResponseText;
                if(!string.IsNullOrEmpty(name))
                {
                    if(MacroSets.Any(a => a.Name.ToLower() == name.ToLower()))
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
                        new ObservableCollection<HotKey>());
            foreach(string groupCategory in GroupCategories)
            {
                set.CategoryGroups.Add(groupCategory, new ObservableCollection<string>() { "CharacterName", "AnotherName" });
            }
            // set some defaults to deal with chat box being active to pause macros

            set.HotKeyCollection.Add(new HotKey(Key.Enter, KeyModifier.None, "Toggle hotkeys on entering and leaving chat", 
                new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.ToggleAllHotkeysOnOff, 1) }));
            set.HotKeyCollection.Add(new HotKey(Key.Escape, KeyModifier.None, "Enable hotkeys in case leaving chat",
                new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.EnableAllHotkeys, 2) }));
            // Just for testing
            //set.HotKeyCollection.Add(new HotKey(Key.D2, KeyModifier.None, "PBAOE Nuke",
            //    new ObservableCollection<HotKeyAction>() 
            //    { 
            //        new HotKeyAction("PBAOE", null, VirtualKeyCode.VK_2, null, null, HotkeyActionType.GroupKeyCommand, 1),
            //        new HotKeyAction("Melee", null, null, null, null, HotkeyActionType.AssistActiveWindow, 2),
            //        new HotKeyAction("Blocker", null, null, null, null, HotkeyActionType.AssistActiveWindow, 3),
            //        new HotKeyAction(null, 150, null, null, null, HotkeyActionType.PauseScript, 4),
            //        new HotKeyAction("Melee", null, null, null, "/stick", HotkeyActionType.SlashCommand, 5),
            //        new HotKeyAction("Melee", null, VirtualKeyCode.VK_4, VirtualKeyCode.ALT, null, HotkeyActionType.GroupKeyCommand, 6),
            //        new HotKeyAction("Blocker", null, null, null, "/face", HotkeyActionType.SlashCommand, 7),
            //    }));

            set.HotKeyCollection.Add(new HotKey(Key.R, KeyModifier.None, "Disable hotkeys for chat reply",
                new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.DisableAllHotkeys, 1) }));

            if(MainWindow.GenSettings != null && MainWindow.GenSettings.WackKey != null && MainWindow.GenSettings.SingleQuoteKey != null)
            {
                set.HotKeyCollection.Add(new HotKey(MainWindow.GenSettings.WackKey.Value, KeyModifier.None, "Disable hotkeys when hitting the / key which starts chat",
                    new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.DisableAllHotkeys, 1) }));
                set.HotKeyCollection.Add(new HotKey(MainWindow.GenSettings.SingleQuoteKey.Value, KeyModifier.None, "Disable hotkeys when hitting the ' key which starts chat",
                    new ObservableCollection<HotKeyAction>() { new HotKeyAction(null, null, null, null, null, HotkeyActionType.DisableAllHotkeys, 1) }));
            }

            MacroSets.Add(set);
            GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
            CurrentSet = set;
            MacroSetNames.Add(name);
            ddlExistingSets.SelectedValue = name;
        }

        private void btnDeleteMacroSet_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete the macro set?", "Really Delete Set?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if(ddlExistingSets.SelectedValue == null)
                {
                    return;
                }
                CurrentSet = null;
                string setName = ddlExistingSets.SelectedValue.ToString();
                MacroSets.Remove(MacroSets.Single(a => a.Name == setName));
                MacroSetNames.Remove(setName);
                ddlExistingSets.SelectedValue = null;
                GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
            }
        }

        private void ddlExistingSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ddlExistingSets.SelectedValue != null)
            {
                btnDeleteMacroSet.Visibility = Visibility.Visible;
                btnAddHotkeyToSet.Visibility = Visibility.Visible;
                expanderGroups.Visibility = Visibility.Visible;
                CurrentSet = MacroSets.Single(a => a.Name == ddlExistingSets.SelectedValue.ToString());
            }
            else
            {
                expanderGroups.Visibility = Visibility.Collapsed;
                btnAddHotkeyToSet.Visibility = Visibility.Collapsed;
                btnDeleteMacroSet.Visibility = Visibility.Collapsed;
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
            UpsertHotkey hotWindow = new UpsertHotkey(roo, GroupCategories);
            hotWindow.Owner = Application.Current.MainWindow;
            hotWindow.ShowDialog();
            // We're going to do this in a more defensive way because they could change the hotkey while they are editing
            var existingHotKey = CurrentSet.HotKeyCollection.SingleOrDefault(a => a.Id == hotWindow.TheHotKey.Id);
            if(existingHotKey != null)
            {
                CurrentSet.HotKeyCollection.Remove(existingHotKey);// so the binding updates
                CurrentSet.HotKeyCollection.Add(hotWindow.TheHotKey);
                var macroEntry = MacroSets.Single(a => a.Name == CurrentSet.Name).HotKeyCollection.Single(a => a.Id == hotWindow.TheHotKey.Id);
                macroEntry = hotWindow.TheHotKey;
            }
            else
            {
                CurrentSet.HotKeyCollection.Add(hotWindow.TheHotKey);
                //MacroSets.Single(a => a.Name == CurrentSet.Name).HotKeyCollection.Add(hotWindow.TheHotKey);
            }
            if(hotWindow.DeleteMe)
            {
                CurrentSet.HotKeyCollection.Remove(hotWindow.TheHotKey);
            }
            GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
        }

        private void btnAddHotkeyToSet_Click(object sender, RoutedEventArgs e)
        {
            KeyPrompt keyPrompt = new KeyPrompt("Press a key", null);
            if(keyPrompt.ShowDialog() == true)
            {
                if(CurrentSet.HotKeyCollection.Any(a => a.Key == keyPrompt.KeyResult && a.KeyModifiers == keyPrompt.KeyModifier))
                {
                    MessageBox.Show("That key combination is already defined. You forgot didn't you? You forget a lot of things lately but you don't forget that you have to pause to remember things a few times a day now. You should go see a doctor but that would make it real and what would be the point? Someday it will have all faded away and you'll be left sitting in a white room in a nightmare of confusion, barely able to eat and pissing yourself. Don't worry, you won't remember your shame around the nurses that have to clean you up, you're just a goldfish, eating, shitting and staring without comprehension at a world eager to forget you in the same way you have forgotten the hotkeys you've already entered here dummy.", "Keys Already Defined");
                    return;
                }
                else
                {
                    HotKey hk = new HotKey(keyPrompt.KeyResult, keyPrompt.KeyModifier, "NA", new ObservableCollection<HotKeyAction>());

                    UpsertHotkey hotWindow = new UpsertHotkey(hk, GroupCategories);
                    hotWindow.Owner = Application.Current.MainWindow;
                    hotWindow.ShowDialog();
                    var existingHotKey = CurrentSet.HotKeyCollection.SingleOrDefault(a => a.Id == hotWindow.TheHotKey.Id);
                    if(existingHotKey != null)
                    {
                        CurrentSet.HotKeyCollection.Remove(existingHotKey);// so the binding updates
                        CurrentSet.HotKeyCollection.Add(hotWindow.TheHotKey);
                        var macroEntry = MacroSets.Single(a => a.Name == CurrentSet.Name).HotKeyCollection.Single(a => a.Id == hotWindow.TheHotKey.Id);
                        macroEntry = hotWindow.TheHotKey;
                    }
                    else
                    {
                        CurrentSet.HotKeyCollection.Add(hotWindow.TheHotKey);
                        //MacroSets.Single(a => a.Name == CurrentSet.Name).HotKeyCollection.Add(hotWindow.TheHotKey);
                    }
                    if(hotWindow.DeleteMe)
                    {
                        CurrentSet.HotKeyCollection.Remove(hotWindow.TheHotKey);
                    }
                    GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
                }
            }
        }

        private void lstSetGroups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lstSetGroups.SelectedItem == null)
            {
                return;
            }
            dynamic roo = lstSetGroups.SelectedItem;
            string preloadNames = "";
            if(CurrentSet.CategoryGroups.ContainsKey(roo.Key))
            {
                preloadNames = string.Join(",", CurrentSet.CategoryGroups[(string)roo.Key].ToList());
            }

            TextPrompt tp = new TextPrompt("Enter group names separated by commas. eg Frank,Barney,Ben", preloadNames);
            tp.ShowDialog();
            if(!string.IsNullOrEmpty(tp.ResponseText))
            {
                var splitNames = tp.ResponseText.Split(',');
                string groupKey = roo.Key;
                CurrentSet.CategoryGroups[groupKey].Clear();
                foreach(string item in splitNames)
                {
                    string nameToAdd = item?.Trim() ?? "";
                    if(!string.IsNullOrEmpty(nameToAdd))
                    {
                        CurrentSet.CategoryGroups[groupKey].Add(nameToAdd);
                    }
                }
                GeneralHelpers.SaveMacrosToDisk(MacroSets.ToList());
            }
        }
    }
}
