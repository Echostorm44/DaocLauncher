using DaocLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace DaocLauncher.Models;

public class MacroSet : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public const int WmHotKey = 0x0312;

    string name;
    public string Name
    {
        get => name;
        set
        {
            if(name == value)
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
            if(categoryGroups == value)
            {
                return;
            }

            categoryGroups = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CategoryGroups)));
        }
    }
    public ObservableCollection<HotKey> HotKeyCollection { get; set; }
    public bool ListeningForHotkeys { get; set; }

    public MacroSet(string name, Dictionary<string, ObservableCollection<string>> categoryGroups, ObservableCollection<HotKey> hotKeyCollection)
    {
        Name = name;
        CategoryGroups = categoryGroups;
        HotKeyCollection = hotKeyCollection;
        ListeningForHotkeys = false;
    }

    public void StartMeUp()
    {
        ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
        ListeningForHotkeys = true;
    }

    private void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
    {
        if(ListeningForHotkeys)
        {
            if(!handled)
            {
                if(msg.message == WmHotKey)
                {
                    var handledKey = (int)msg.wParam;
                    var targetKey = HotKeyCollection.SingleOrDefault(a => a.Id == handledKey);
                    if(targetKey != null)
                    {
                        if(targetKey.HotKeyActionEvent != null)
                        {
                            handled = true;
                            targetKey.HotKeyActionEvent.Invoke(targetKey);
                        }
                    }
                }
            }
        }
    }

    public MacroSet DeepCopyMe()
    {
        var catGroups = new Dictionary<string, ObservableCollection<string>>();
        var hotKeys = new ObservableCollection<HotKey>();
        foreach(var cat in CategoryGroups)
        {
            catGroups.Add(cat.Key, cat.Value);
        }
        foreach(var hot in HotKeyCollection)
        {
            var triggerActs = new ObservableCollection<HotKeyAction>();
            foreach(var act in hot.TriggeredActions)
            {
                triggerActs.Add(act);
            }
            var hk = new HotKey(hot.Key, hot.KeyModifiers, hot.Description, triggerActs);
            hotKeys.Add(hk);
        }
        var result = new MacroSet(Name, catGroups, hotKeys);
        return result;
    }

    public void Dispose()
    {
        ListeningForHotkeys = false;
        ComponentDispatcher.ThreadFilterMessage -= new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
        foreach(var item in this.HotKeyCollection)
        {
            item.Dispose();
        }
        this.HotKeyCollection.Clear();
    }
}
