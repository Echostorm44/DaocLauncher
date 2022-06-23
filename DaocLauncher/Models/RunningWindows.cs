using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaocLauncher.Models
{
    public class RunningWindows
    {
        public IntPtr Handle { get; set; }
        public string Name { get; set; }


        public RunningWindows()
        {          
        }
    }
}
