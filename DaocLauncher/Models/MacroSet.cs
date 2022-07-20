using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaocLauncher.Helpers;
namespace DaocLauncher.Models
{
    public class MacroSet : INotifyPropertyChanged
    {
        string name;
        public string Name
        {
            get => name;
            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        // eg PBAOE: [ "Harry", "Sally", "Paul" ], Melee: ["Blocker1"]
        Dictionary<string, ObservableCollection<string>> categoryGroups;
        public Dictionary<string, ObservableCollection<string>> CategoryGroups
        {
            get => categoryGroups;
            set
            {
                if (categoryGroups == value)
                {
                    return;
                }

                categoryGroups = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryGroups)));
            }
        }
        Dictionary<HotKey, ObservableCollection<HotKeyAction>> hotKeyCollection;
        public Dictionary<HotKey, ObservableCollection<HotKeyAction>> HotKeyCollection
        {
            get => hotKeyCollection;
            set
            {
                if (hotKeyCollection == value)
                {
                    return;
                }

                hotKeyCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HotKeyCollection)));
            }
        } // 2: assist melee, slash command face, hotkey 2, pause 2000, hotkey 3

        public MacroSet(string name, Dictionary<string, ObservableCollection<string>> categoryGroups, Dictionary<HotKey, ObservableCollection<HotKeyAction>> hotKeyCollection)
        {
            Name = name;
            CategoryGroups = categoryGroups;
            HotKeyCollection = hotKeyCollection;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
