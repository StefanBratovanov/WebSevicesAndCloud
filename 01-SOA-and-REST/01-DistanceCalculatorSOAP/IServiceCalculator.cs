using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace _01_DistanceCalculatorSOAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceCalculator
    {

        [OperationContract]
        double CalculateDistance(Point startPoint, Point endPoint);
    }

    [DataContract]
    public class Point
    {

        [DataMember]
        public int X { get; set; }

        [DataMember]
        public int Y { get; set; }
    }
}
