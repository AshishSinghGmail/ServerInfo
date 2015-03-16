using ServerInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerInfo.Controllers
{
    public class GetLastHourDataController : ApiController
    {
        public IEnumerable<double> GetLastHourData(string Name, Attrib attribute, bool bHour)
        {
            string serverName = Name.Trim().ToUpper();

            if (ServerRepository.listServers == null)
            {
                return null;
            }
            ServerClass server = null;
            // Check and change Name of the Server
            for (int i = 0; i < ServerRepository.listServers.Count; i++)
            {
                if (string.Compare(serverName, ServerRepository.listServers[i].Name) == 0)
                {
                    server = ServerRepository.listServers[i];
                    break;
                }
            }

            if (server == null)
            {
                return null;
            }

            List<double> list = new List<double>();

            for (int i = 0; i < server.attributesList.Count; i ++)
            {
                // check which attribute is being requested
                if (server.attributesList[i].Attribute == attribute)
                {
                    if (bHour)
                    {
                        foreach (double val in server.attributesList[i].lastHourQueue)
                            list.Add(val);
                    }
                    else
                    {
                        foreach (double val in server.attributesList[i].lastDayQueue)
                            list.Add(val);
                    }

                }
            }

                return list;
        }
    }
}
