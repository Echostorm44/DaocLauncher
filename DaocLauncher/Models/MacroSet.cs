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

        public event PropertyChangedEventHandler? PropertyChanged;

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
        public ObservableCollection<HotKey> HotKeyCollection { get; set; }
        
        public MacroSet(string name, Dictionary<string, ObservableCollection<string>> categoryGroups, ObservableCollection<HotKey> hotKeyCollection)
        {
            Name = name;
            CategoryGroups = categoryGroups;
            HotKeyCollection = hotKeyCollection;
        }

    }
}
