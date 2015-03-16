using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerInfo.Models
{
    /// <summary>
    /// Enum for attributes for the Server. Add more attributes as the requirements increase. 
    /// Each Server will have list of attributes. 
    /// </summary>
    public enum Attrib{ CPULoad, RAMLoad};
    public class ServerAttributes
    {
        /// <summary>
        /// making default constructor private
        /// </summary>
        private ServerAttributes(){}
        public Attrib Attribute { get; private set; }// Name of the attribute
        public DateTime dtLastValue; // Last time when the value was updated
        
        // Ideally I would have wanted to use Double ended queue or circular array of double values
        // for the sake of time I am using list
        public List<double> lastHourQueue { get; set; }// Queue to save the last hour average values
        public List<double> lastDayQueue { get; set; } // Queue to save last day's average values

       
        public object lockHour; // lock for adding data to queue for last hour
        public object lockDay; // lock for adding data to queue for last day

        /// <summary>
        /// public constructor 
        /// </summary>
        /// <param name="szName">Name of the Attribute</param>
        public ServerAttributes(Attrib attribute)
        {
            this.Attribute = attribute;
            lastHourQueue = new List<double>();
            lastDayQueue = new List<double>();
            lockDay = new object();
            lockHour = new object();
            dtLastValue = DateTime.MinValue;
        }
    }
}