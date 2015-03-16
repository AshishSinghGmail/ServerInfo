using ServerInfo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerInfo.Controllers
{
    /// <summary>
    ///  Controller for adding a new Server
    /// </summary>
    public class AddServerController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage AddServer(string Name)
        {
            // Authenticate if the request is from admin
            bool bVerified = true;
            if (!bVerified)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            string serverName = Name.Trim().ToUpper();

            if (ServerRepository.listServers == null)
            {
                ServerRepository.listServers = new List<ServerClass>(); // if List of servers is null initialize list 
            }

            if (ServerRepository.listServers.Count >= ServerRepository.MAX_SERVERS)
            {
                Debug.WriteLine("Dude you want to add more than 1000 servers??? Pay more $$$'s");
                return new HttpResponseMessage(HttpStatusCode.PaymentRequired);
            }

            // Check if the Server already exists
            for (int i = 0; i < ServerRepository.listServers.Count; i++)
            {
                if (string.Compare(serverName, ServerRepository.listServers[i].Name) == 0)
                {
                    Debug.WriteLine("Server " + serverName + " already exists");
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                }
            }

            try
            {
                // for multi threaded environment; this function is not expected to be called a lot 
                // thus locking should not cause that much delay 
                lock (ServerRepository.objLockServers)
                {
                    ServerClass server = new ServerClass(serverName);
                    ServerRepository.listServers.Add(server);
                }
            }
            catch (Exception ex)
            {
                // log the exception
                Debug.WriteLine("Exception: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }

            // Wohooo !!! new server created and is added in list of servers.
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }
}
