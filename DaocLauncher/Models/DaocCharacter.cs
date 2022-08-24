using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaocLauncher.Models
{
    public class DaocCharacter
    {
        public string Name { get; set; }
        public string ParentAccountName { get; set; }
        public string Server { get; set; }
        public string Class { get; set; }
        public int WindowX { get; set; }
        public int WindowY { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public bool WindowFullScreen { get; set; }
        public bool WindowFullScreenWindowed { get; set; }
        public int MonitorNumber { get; set; }

        public DaocCharacter()
        {
            Name = "";
            ParentAccountName = "";
            Server = "";
            Class = "";
        }
    }
}
