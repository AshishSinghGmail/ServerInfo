using ServerInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerInfo.Controllers
{
    public class ChangeServerStatusController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage ChangeStatusServer(string Name, bool IsAvailable)
        {
            HttpResponseMessage msg = null;
            // Authenticate if the request is from admin
            bool bVerified = true;
            if (!bVerified)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            string serverName = Name.Trim().ToUpper();

            if (ServerRepository.listServers == null)
            {
                msg = new HttpResponseMessage(HttpStatusCode.NotFound);
                msg.Content = new StringContent("There are no servers with name " + serverName);
                return msg;
            }

            // Check and change Name of the Server
            for (int i = 0; i < ServerRepository.listServers.Count; i++)
            {
                if (string.Compare(serverName, ServerRepository.listServers[i].Name) == 0)
                {
                    lock (ServerRepository.objLockServers)
                    {
                        ServerRepository.listServers[i].IsAvailable = IsAvailable;
                    }
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }

            // server not found
            msg = new HttpResponseMessage(HttpStatusCode.NotFound);
            msg.Content = new StringContent("Dude check the name of Server");
            return msg;
        }
    }
}
