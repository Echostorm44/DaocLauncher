using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
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
/// Interaction logic for EditCharacters.xaml
/// </summary>
public partial class EditCharacters : UserControl
{
    public ObservableCollection<DaocCharacter> AllCharacters { get; set; }
    public ObservableCollection<string> Servers { get; set; }
    public ObservableCollection<string> AccountNames { get; set; }
    public ObservableCollection<string> CharacterClasses { get; set; }
    Helpers.Debouncer debouncer = new Debouncer(500);

    public EditCharacters()
    {
        InitializeComponent();
        var classList = GeneralHelpers.GetAllCharacterClasses();
        CharacterClasses = new ObservableCollection<string>();
        foreach(var item in classList)
        {
            CharacterClasses.Add(item.Key);
        }

        var characterList = GeneralHelpers.LoadCharactersFromDisk();
        AllCharacters = new ObservableCollection<DaocCharacter>();
        foreach(var item in characterList)
        {
            AllCharacters.Add(item);
        }
        var serverList = GeneralHelpers.LoadServerListFromDisk();
        Servers = new ObservableCollection<string>();
        foreach(var item in serverList.Servers)
        {
            Servers.Add(item.Name);
        }
        var accountList = GeneralHelpers.LoadAccountListFromDisk();
        AccountNames = new ObservableCollection<string>();
        foreach(var item in accountList.MyAccounts)
        {
            AccountNames.Add(item.Name ?? "NA");
        }

        this.DataContext = this;
    }

    private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        debouncer.Debounce(() =>
        {
            GeneralHelpers.SaveCharactersToDisk(AllCharacters.ToList());
        });
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if(gridChars.SelectedItem != null)
        {
            AllCharacters.Remove((DaocCharacter)gridChars.SelectedItem);
            GeneralHelpers.SaveCharactersToDisk(AllCharacters.ToList());
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

    private void EditWindowSettings(object sender, RoutedEventArgs e)
    {
        if(e.Source != null)
        {
            var targetCharacter = (DaocCharacter)((Button)e.Source).DataContext;
            WindowStatsPrompt ws = new WindowStatsPrompt(targetCharacter);
            ws.ShowDialog();
            targetCharacter.WindowFullScreen = ws.IsFullScreen;
            targetCharacter.WindowFullScreenWindowed = ws.IsFullScreenWindowed;
            targetCharacter.WindowX = ws.ResponseX;
            targetCharacter.WindowY = ws.ResponseY;
            targetCharacter.WindowHeight = ws.ResponseHeight;
            targetCharacter.WindowWidth = ws.ResponseWidth;
            if(ws.CurrentScreenName.Contains("DISPLAY"))
            {
                Regex regNum = new Regex(".?DISPLAY([0-9]+)");
                var regResult = regNum.Match(ws.CurrentScreenName);
                if(regResult.Success && regResult.Groups.Count > 1)
                {
                    var monitorNumTemp = regResult.Groups[1].Value;
                    if(int.TryParse(monitorNumTemp, out var monitorNum))
                    {
                        targetCharacter.MonitorNumber = (monitorNum - 1);// These seem to start from 1 but DAOC is zero based
                    }
                }
            }

            GeneralHelpers.SaveCharactersToDisk(AllCharacters.ToList());

            var genSettings = GeneralHelpers.LoadGeneralSettingsFromDisk();
            string gameFolder = genSettings.PathToGameDll.Replace("game.dll", "");
            var charSymLinkFolder = $@"{genSettings.PathToSymbolicLinks}\{targetCharacter.Name}\";
            var characterUserSettingsFolder = $@"{charSymLinkFolder}copiedUserSettings\";

            if(!Directory.Exists(charSymLinkFolder))
            {
                Directory.CreateDirectory(charSymLinkFolder);
            }
            if(Directory.EnumerateFiles(charSymLinkFolder).Count() == 0)
            {// Folder isn't initialized yet, lets do that now.
                var filesToLink = Directory.GetFiles(gameFolder, "*.*", SearchOption.TopDirectoryOnly);
                var foldersToLink = Directory.GetDirectories(gameFolder, "*.*", SearchOption.TopDirectoryOnly);
                foreach(var item in filesToLink)
                {
                    var currentFileName = System.IO.Path.GetFileName(item);
                    File.CreateSymbolicLink(charSymLinkFolder + currentFileName, item);
                }
                foreach(var item in foldersToLink)
                {
                    var currentFolderName = item.Replace(gameFolder, "");
                    Directory.CreateSymbolicLink(charSymLinkFolder + currentFolderName, item);
                }
                if(!Directory.Exists(characterUserSettingsFolder))
                {
                    Directory.CreateDirectory(characterUserSettingsFolder);
                }
                var userFiles = Directory.GetFiles(genSettings.PathToUserSettings);
                foreach(var item in userFiles)
                {
                    var currentFileName = System.IO.Path.GetFileName(item);
                    if(currentFileName == "user.dat")
                    {
                        File.Copy(item, characterUserSettingsFolder + "oldUser.dat.old", true);
                    }
                    File.Copy(item, characterUserSettingsFolder + currentFileName, true);
                }
                // change user.dat filename then iterate through old user.dat writing lines && occasionally changing a value for resolution && windowed writing to a new user.dat
                UpdateUserDat(targetCharacter, characterUserSettingsFolder);
            }
            else
            {
                File.Copy(characterUserSettingsFolder + "user.dat", characterUserSettingsFolder + "oldUser.dat.old", true);
                UpdateUserDat(targetCharacter, characterUserSettingsFolder);
            }
            // Ok, the Symlinks are ready, we just need to add a Paths file && a new folder to hold out user.dat && other stuff like char inis && edit the user.dat with our resolutionsettings 
            using(TextWriter tw = new StreamWriter(charSymLinkFolder + "paths.dat", false))
            {
                tw.WriteLine("[paths]");
                tw.WriteLine($@"settings={characterUserSettingsFolder}");
            }
        }
    }

    private static void UpdateUserDat(DaocCharacter targetCharacter, string characterUserSettingsFolder)
    {
        using(TextReader tr = new StreamReader(characterUserSettingsFolder + "oldUser.dat.old"))
        {
            using(TextWriter tw = new StreamWriter(characterUserSettingsFolder + "user.dat", false))
            {
                bool inPerformaceSection = false;
                while(tr.Peek() > 0)
                {
                    var currentLine = tr.ReadLine() ?? "";
                    if(currentLine.StartsWith("windowed="))
                    {
                        currentLine = "windowed=" + (targetCharacter.WindowFullScreen ? "0" : "1");
                    }
                    if(currentLine.StartsWith("screen_height="))
                    {
                        currentLine = "screen_height=" + targetCharacter.WindowHeight;
                    }
                    if(currentLine.StartsWith("screen_width="))
                    {
                        currentLine = "screen_width=" + targetCharacter.WindowWidth;
                    }
                    if(currentLine.StartsWith("fullscreen_windowed="))
                    {
                        currentLine = "fullscreen_windowed=" + (targetCharacter.WindowFullScreenWindowed ? "1" : "0");
                    }
                    if(currentLine.StartsWith("[performance]"))
                    {
                        inPerformaceSection = true;
                    }
                    if(inPerformaceSection && currentLine.StartsWith("item10="))
                    {// This is where full screen window sets the monitor on my system at least.  0 is monitor 1, 1 is monitor 2, will need more testing
                        currentLine = "item10=" + targetCharacter.MonitorNumber;
                    }
                    if(currentLine.StartsWith("["))
                    {
                        inPerformaceSection = false;
                    }
                    tw.WriteLine(currentLine);
                }
            }
        }
    }
}
