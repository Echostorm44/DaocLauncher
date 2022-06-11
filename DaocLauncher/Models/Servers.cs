using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaocLauncher.Models
{
    public class Server
    {
        public string ID { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public bool IsOfficial { get; set; }
    }

    public class ServerList
    {
        public List<Server> Servers { get; set; }
        public ServerList()
        {
            Servers = new List<Server>();
        }
    }
}
