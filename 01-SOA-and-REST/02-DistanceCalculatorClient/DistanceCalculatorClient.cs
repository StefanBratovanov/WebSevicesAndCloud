using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02_DistanceCalculatorClient.DistanceService;

namespace _02_DistanceCalculatorClient
{
    class DistanceCalculatorClient
    {
        static void Main()
        {
            var client = new ServiceCalculatorClient();

            var startP = new Point { X = 17, Y = 17 };
            var endP = new Point { X = 0, Y = -0 };

            var distance = client.CalculateDistance(startP, endP);
            Console.WriteLine("Distance: {0:F3}", distance);
            
        }
    }
}
