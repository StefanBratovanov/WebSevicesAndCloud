using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using _03_DistanceCalculatorREST;

namespace _04_DistanceCalculatorRESTClient
{
    class DistanceCalculatorRESTClient
    {
        static void Main()
        {
            var client = new RestClient("http://localhost:58297");

            var request = new RestRequest("api/points", Method.GET);

            request.AddParameter("startX", 0);
            request.AddParameter("startY", 0);
            request.AddParameter("endX", 9);
            request.AddParameter("endY", 9);


            var response = client.Execute(request);
   
            var content = response.Content;

            Console.WriteLine("Distance: {0}", content);
        }
    }
}
