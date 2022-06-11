using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaocLauncher.Models
{
    public class DaocAccount
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string DefaultTag { get; set; }
        public string? DefaultServer { get; set; }
    }
    public class AllDaocAccounts
    {
        public List<DaocAccount> MyAccounts { get; set; }

        public AllDaocAccounts()
        {
            MyAccounts = new List<DaocAccount>();            
        }
    }
}
