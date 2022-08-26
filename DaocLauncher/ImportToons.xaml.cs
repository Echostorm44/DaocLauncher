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

namespace DaocLauncher;

/// <summary>
/// Interaction logic for ImportToons.xaml
/// </summary>
public partial class ImportToons : UserControl
{
    public ObservableCollection<ImportToon> MyToons { get; set; }
    public ObservableCollection<string> AccountNames { get; set; }

    public ImportToons()
    {
        MyToons = new ObservableCollection<ImportToon>();
        var accountList = GeneralHelpers.LoadAccountListFromDisk();
        AccountNames = new ObservableCollection<string>();
        foreach(var item in accountList.MyAccounts)
        {
            AccountNames.Add(item.Name ?? "NA");
        }
        InitializeComponent();
        this.DataContext = this;
    }

    private void btnInstructions_Click(object sender, RoutedEventArgs e)
    {
        var myProcess = new System.Diagnostics.Process();
        myProcess.StartInfo.UseShellExecute = true;
        myProcess.StartInfo.FileName = "https://github.com/Echostorm44/DaocLauncher/wiki/Importing-Characters";
        myProcess.Start();
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
            if(MyToons.Count > 0 && ddlAccountNames.SelectedValue != null)
            {
                btnImportSelected.Visibility = Visibility.Visible;
            }
            else
            {
                btnImportSelected.Visibility = Visibility.Hidden;
            }
        }
    }

    private void btnImportSelected_Click(object sender, RoutedEventArgs e)
    {
        // Load existing toons to check for dupes
        if(ddlAccountNames.SelectedValue == null)
        {
            return;
        }
        var existingToons = GeneralHelpers.LoadCharactersFromDisk();
        foreach(var item in MyToons)
        {
            if(existingToons.Any(a => a.Name == item.Name) || !item.IsSelected)
            {
                continue;
            }
            var toon = new DaocCharacter();
            toon.Name = item.Name;
            toon.Server = item.ServerName;
            toon.Class = item.Class;
            toon.ParentAccountName = ddlAccountNames.SelectedValue.ToString();
            existingToons.Add(toon);
        }
        GeneralHelpers.SaveCharactersToDisk(existingToons.ToList());
        MyToons.Clear();
        MessageBox.Show("Selected Toons Added!");
    }

    private void ddlAccountNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(MyToons.Count > 0 && ddlAccountNames.SelectedValue != null)
        {
            btnImportSelected.Visibility = Visibility.Visible;
        }
        else
        {
            btnImportSelected.Visibility = Visibility.Hidden;
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
