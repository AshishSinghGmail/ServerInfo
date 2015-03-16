using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerInfo.Models
{
    public class ServerRepository
    {
        public  const int MAX_SERVERS = 1000;
        // List of Servers, not used currently but might get used later
        public static List<ServerClass> listServers = null;

        public static object objLockServers = new object();

    }
}