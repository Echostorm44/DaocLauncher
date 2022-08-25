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

namespace DaocLauncher;

/// <summary>
/// Interaction logic for ImportToons.xaml
/// </summary>
public partial class ImportToons : UserControl
{
    public ObservableCollection<ImportToon> MyToons { get; set; }

    public ImportToons()
    {
        MyToons = new ObservableCollection<ImportToon>();
        InitializeComponent();
        this.DataContext = this;
    }

    private void btnInstructions_Click(object sender, RoutedEventArgs e)
    {
    }

    private void btnImport_Click(object sender, RoutedEventArgs e)
    {
        if(!string.IsNullOrEmpty(txtPasteBox.Text))
        {
            MyToons.Clear();
            var lines = txtPasteBox.Text.Split("\r\n");
            foreach(var line in lines)
            {
                var toon = new ImportToon();
                var items = line.Split(" ");
                if(items.Length != 7)
                {
                    continue;
                }
                toon.IsSelected = true;
                toon.ServerName = items[0].Trim();
                toon.Realm = items[1].Trim();
                toon.Name = items[2].Trim();
                toon.Level = items[4].Trim();
                toon.Race = items[5].Trim();
                toon.Class = items[6].Trim();
                MyToons.Add(toon);
            }
        }
    }

    private void btnImportSelected_Click(object sender, RoutedEventArgs e)
    {
        // Load existing toons to check for dupes

        foreach(var item in MyToons)
        {
        }
    }
}

public class ImportToon
{
    public bool IsSelected { get; set; }
    public string ServerName { get; set; }
    public string Realm { get; set; }
    public string Name { get; set; }
    public string Level { get; set; }
    public string Race { get; set; }
    public string Class { get; set; }

    public ImportToon()
    {
    }
}
