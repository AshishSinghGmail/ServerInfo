using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerInfo.Models
{
    public class ServerClass
    {
        /// <summary>
        /// Making default constructor Private
        /// </summary>
        private ServerClass() { }

        public string Name { get;  set; }// Name of the Server
        public bool IsAvailable { get;  set; } // Is the server available or not. For future use
        public Guid HardwareID { get; private set; } // Unique Hardware ID of the Server
        
        // List of attributes for server This can increase if requirements increase. 
        public List<ServerAttributes> attributesList { get; private set; }


        /// <summary>
        /// Private Server class, GetServer function will return the instance of the Server
        /// </summary>
        /// <param name="szName">Name of the Server</param>
        public ServerClass(string szName)
        {
            this.HardwareID = Guid.NewGuid();// assign new GUID for Hardware ID, Not use d anywhere right now
            this.Name = szName;
            this.IsAvailable = true;

            attributesList = new List<ServerAttributes>();
            // Add Default Attributes for CPU Load and RAM Load
            ServerAttributes cpuLoad = new ServerAttributes(Attrib.CPULoad);
            ServerAttributes ramLoad = new ServerAttributes(Attrib.RAMLoad);
            attributesList.Add(cpuLoad);
            attributesList.Add(ramLoad);
        }


    }
}