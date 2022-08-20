using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace DaocLauncher;

/// <summary>
/// Interaction logic for WindowStatsPrompt.xaml
/// </summary>
public partial class WindowStatsPrompt : Window, INotifyPropertyChanged
{
    public int ResponseWidth { get; set; }
    public int ResponseHeight { get; set; }
    public int ResponseX { get; set; }
    public int ResponseY { get; set; }

    ScreenResolution selectedResolution;
    public ScreenResolution SelectedResolution
    {
        get => selectedResolution;
        set
        {
            if(selectedResolution == value)
            {
                return;
            }

            selectedResolution = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedResolution)));
        }
    }
    bool isFullScreen;
    public bool IsFullScreen
    {
        get => isFullScreen;
        set
        {
            if(isFullScreen == value)
            {
                return;
            }

            isFullScreen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFullScreen)));
        }
    }
    bool isFullScreenWindowed;
    public bool IsFullScreenWindowed
    {
        get => isFullScreenWindowed;
        set
        {
            if(isFullScreenWindowed == value)
            {
                return;
            }

            isFullScreenWindowed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFullScreenWindowed)));
        }
    }

    public ObservableCollection<ScreenResolution> PossibleResolutions { get; set; }
    public string CurrentScreenName { get; set; }

    Dictionary<string, List<ScreenResolution>>? ScreenResolutionDictionary;

    public WindowStatsPrompt(DaocCharacter characterIn)
    {
        PossibleResolutions = new ObservableCollection<ScreenResolution>();
        ScreenResolutionDictionary = new Dictionary<string, List<ScreenResolution>>();
        var allScreens = System.Windows.Forms.Screen.AllScreens;
        foreach(System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
        {
            var modes = NativeMethods.GetDeviceModes(screen.DeviceName);
            var finalModes = (from a in modes
                select new ScreenResolution
                {
                    Height = a.dmPelsHeight,
                    Width = a.dmPelsWidth,
                    Text = (a.dmPelsWidth + " x " + a.dmPelsHeight)
                }).ToList().Distinct();
            ScreenResolutionDictionary.Add(screen.DeviceName, finalModes.OrderBy(a => (a.Width + a.Height)).ThenBy(a => a.Width).Distinct().ToList());
        }
        InitializeComponent();
        this.DataContext = this;
        IsFullScreen = characterIn.WindowFullScreen;
        IsFullScreenWindowed = characterIn.WindowFullScreenWindowed;
        if(characterIn.WindowHeight > 0 && characterIn.WindowWidth > 0)
        {
            this.Width = characterIn.WindowWidth;
            this.Height = characterIn.WindowHeight;
            this.Left = characterIn.WindowX;
            this.Top = characterIn.WindowY;
        }
        LoadResolutionDropdown();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
        //var gas = NativeMethods.GetGraphicsAdapters();
        //var mons = NativeMethods.GetMonitors(gas.First().DeviceName);
        //var modes2 = NativeMethods.GetDeviceModes(gas.First().DeviceName);        
        //var point = new System.Drawing.Point((int)this.Left, (int)this.Top);
        //var p2s = PointToScreen(new Point(point.X, point.Y));
        //var scr = System.Windows.Forms.Screen.FromPoint(point);
        //var too = scr.Bounds.Contains(point) ? scr : null;
        SetReturnAndClose();
        DialogResult = true;
    }

    private void SetReturnAndClose()
    {
        var point = new System.Drawing.Point((int)this.Left, (int)this.Top);
        ResponseHeight = (int)this.ActualHeight;
        ResponseWidth = (int)this.ActualWidth;
        ResponseX = point.X;
        ResponseY = point.Y;
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
        var point = new System.Drawing.Point((int)this.Left, (int)this.Top);
        ResponseX = point.X;
        ResponseY = point.Y;
        LoadResolutionDropdown();
    }

    private void LoadResolutionDropdown()
    {
        var moo = this;
        var mommy = Application.Current.MainWindow;
        var point = new System.Drawing.Point((int)mommy.Left, (int)mommy.Top);
        var scr = System.Windows.Forms.Screen.FromPoint(point);
        if(scr != null && ScreenResolutionDictionary.ContainsKey(scr.DeviceName) && CurrentScreenName != scr.DeviceName)
        {
            CurrentScreenName = scr.DeviceName;
            var comboItems = ScreenResolutionDictionary[scr.DeviceName];
            PossibleResolutions.Clear();
            foreach(var res in comboItems)
            {
                PossibleResolutions.Add(res);
            }
        }
    }

    private void ddlResolutions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(SelectedResolution != null)
        {
            this.Width = SelectedResolution.Width;
            this.Height = selectedResolution.Height;
            ResponseHeight = selectedResolution.Height;
            ResponseWidth = SelectedResolution.Width;
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        SetReturnAndClose();
    }
}

public class ScreenResolution
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Text { get; set; }

    public ScreenResolution()
    {
    }

    public override bool Equals(object? obj)
    {
        if(obj == null || obj.GetType() != typeof(ScreenResolution))
        {
            return false;
        }
        return Text.Equals(((ScreenResolution)obj).Text);
    }

    public override int GetHashCode()
    {
        return Text.GetHashCode();
    }
}