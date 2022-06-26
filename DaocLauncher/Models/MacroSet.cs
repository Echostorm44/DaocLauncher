using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaocLauncher.Helpers;
namespace DaocLauncher.Models
{
    public class MacroSet
    {
        public string Name { get; set; }
        public Dictionary<string, List<string>> CategoryGroups { get; set; }// eg PBAOE: [ "Harry", "Sally", "Paul" ], Melee: ["Blocker1"]
        public Dictionary<HotKey, List<HotKeyAction>> HotKeyCollection { get; set; } // 2: assist melee, slash command face, hotkey 2, pause 2000, hotkey 3

        public MacroSet(string name, Dictionary<string, List<string>> categoryGroups, Dictionary<HotKey, List<HotKeyAction>> hotKeyCollection)
        {
            Name = name;
            CategoryGroups = categoryGroups;
            HotKeyCollection = hotKeyCollection;
        }
    }
}
