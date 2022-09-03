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
/// Interaction logic for EditAccountsControl.xaml
/// </summary>
public partial class EditAccountsControl : UserControl
{
    public ObservableCollection<DaocAccount> AccountData { get; set; }
    Helpers.Debouncer debouncer = new Debouncer(500);

    public EditAccountsControl()
    {
        InitializeComponent();
        var accountList = GeneralHelpers.LoadAccountListFromDisk();
        AccountData = new ObservableCollection<DaocAccount>();
        foreach(var item in accountList.MyAccounts)
        {
            AccountData.Add(item);
        }
        this.DataContext = this;
    }

    private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        debouncer.Debounce(() =>
        {
            GeneralHelpers.SaveAccountListToDisk(AccountData.ToList());
        });
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if(gridAccounts.SelectedItem != null)
        {
            AccountData.Remove((DaocAccount)gridAccounts.SelectedItem);
            GeneralHelpers.SaveAccountListToDisk(AccountData.ToList());
        }
    }

    private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
    {
        if(e.OriginalSource.GetType() == typeof(DataGridCell))
        {
            DataGrid grid = (DataGrid)sender;
            grid.BeginEdit(e);
        }
    }
}
