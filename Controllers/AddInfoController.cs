using ServerInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerInfo.Controllers
{
    public class AddInfoController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage AddInfo(string Name, double CPULoad, double RAMLoad)
        {
            DateTime dtnow = DateTime.Now;
            HttpResponseMessage msg = null;
            string serverName = Name.Trim().ToUpper();
            if (ServerRepository.listServers == null)
            {
                msg = new HttpResponseMessage(HttpStatusCode.NotFound);
                msg.Content = new StringContent("There are no servers with name " + serverName);
                return msg;
            }

            ServerClass server = null;
            // Check and change Name of the Server
            for (int i = 0; i < ServerRepository.listServers.Count; i++)
            {
                if (string.Compare(serverName, ServerRepository.listServers[i].Name) == 0)
                {
                    // found server
                    server = ServerRepository.listServers[i];
                    break;
                }
            }
            // Check if the server is found
            if (server == null)
            {
                msg = new HttpResponseMessage(HttpStatusCode.NotFound);
                msg.Content = new StringContent("There are no servers with name " + serverName);
                return msg;
            }

            for (int i = 0; i < server.attributesList.Count; i++)
            {
                if (server.attributesList[i].Attribute == Attrib.CPULoad)
                {
                    AddtoHourQueue(server.attributesList[i],dtnow, CPULoad);
                    AddToDayQueue(server.attributesList[i], dtnow, CPULoad);
                }
                else if (server.attributesList[i].Attribute == Attrib.RAMLoad)
                {
                    AddtoHourQueue(server.attributesList[i], dtnow, RAMLoad);
                    AddToDayQueue(server.attributesList[i], dtnow, CPULoad);
                }
            }

            msg = new HttpResponseMessage(HttpStatusCode.OK);
            msg.Content = new StringContent( serverName + " Values updated.");
            return msg;
            
        }

        /// <summary>
        /// Add value to Hour Queue
        /// </summary>
        /// <param name="attrib">Attribute for which value is to be added</param>
        /// <param name="dtNow">Date time of request</param>
        /// <param name="val">Value to be added</param>
        private void AddtoHourQueue(ServerAttributes attrib, DateTime dtNow , double val)
        {
            TimeSpan duration = DateTime.Now - attrib.dtLastValue;
            // check is there was no data since last 1 hour
            if (duration.TotalMinutes > 60)
            {
                lock (attrib.lockHour)
                {
                    attrib.lastHourQueue.Clear();
                    attrib.lastHourQueue.Add(val);
                    attrib.dtLastValue = dtNow;

                }
                return;
            }

            // if last data entered was less than 1 min ago then calculate the average and store value
            if (duration.TotalMinutes < 1)
            {
                lock (attrib.lockHour)
                {
                    double initialVal = attrib.lastHourQueue[0];
                    attrib.lastHourQueue.RemoveAt(0);
                    double newVal = initialVal + (val - initialVal) / 2;
                    attrib.lastHourQueue.Insert(0, newVal);
                    attrib.dtLastValue = dtNow;
                }
                return;
            }

            // if the last value was more than 1 minute ago but less than 60 minutes
            lock (attrib.lockHour)
            {
                // Add 0 for all the values corresponding to minutes where there was no data
                for (int i = 0; i < duration.TotalMinutes - 1; i++)
                {
                    attrib.lastHourQueue.Insert(0, 0);
                }
                // add the new value
                attrib.lastHourQueue.Insert(0, val);

                // make sure the queue is less than 60 minutes
                while (attrib.lastHourQueue.Count > 60)
                {
                    attrib.lastHourQueue.RemoveAt(60);
                }
                attrib.dtLastValue = dtNow;
            }

        }


        /// <summary>
        /// Add value to Day Queue
        /// </summary>
        /// <param name="attrib">Attribute for which value is to be added</param>
        /// <param name="dtNow">Date time of request</param>
        /// <param name="val">Value to be added</param>
        private void AddToDayQueue(ServerAttributes attrib, DateTime dtNow, double val)
        {
            TimeSpan duration = DateTime.Now - attrib.dtLastValue;
            // check is there was no data since last Day
            if (duration.TotalHours > 24)
            {
                lock (attrib.lockDay)
                {
                    attrib.lastDayQueue.Clear();
                    attrib.lastDayQueue.Add(val);
                    attrib.dtLastValue = dtNow;
                }
                return;
            }

            // if last data entered was less than 1 day ago then calculate the average and store value
            if (duration.TotalHours < 1)
            {
                lock (attrib.lockDay)
                {
                    double initialVal = attrib.lastDayQueue[0];
                    attrib.lastDayQueue.RemoveAt(0);
                    double newVal = initialVal + (val - initialVal) / 2;
                    attrib.lastDayQueue.Insert(0, newVal);
                    attrib.dtLastValue = dtNow;
                }
                return;
            }

            // if the last value was more than 1 hour ago but less than 24 hours 
            lock (attrib.lockDay)
            {
                // Add 0 for all the values corresponding to minutes where there was no data
                for (int i = 0; i < duration.TotalHours - 1; i++)
                {
                    attrib.lastDayQueue.Insert(0, 0);
                }
                // add the new value
                attrib.lastDayQueue.Insert(0, val);

                // make sure the queue is less than 60 minutes
                while (attrib.lastDayQueue.Count > 60)
                {
                    attrib.lastDayQueue.RemoveAt(60);
                }
                attrib.dtLastValue = dtNow;
            }
        }
    }
}
