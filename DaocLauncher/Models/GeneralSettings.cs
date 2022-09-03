using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DaocLauncher.Models;

public class GeneralSettings
{
    public string PathToGameDll { get; set; }
    public string PathToSymbolicLinks { get; set; }
    public string PathToUserSettings { get; set; }
    public bool IsFirstTime { get; set; }
    public Key? WackKey { get; set; }
    public Key? SingleQuoteKey { get; set; }
}
