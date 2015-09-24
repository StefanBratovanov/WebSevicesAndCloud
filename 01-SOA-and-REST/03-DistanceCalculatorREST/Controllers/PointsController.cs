using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _03_DistanceCalculatorREST.Controllers
{
    public class PointsController : ApiController
    {
        public IHttpActionResult GetDistance(int startX, int startY, int endX, int endY) {
            int deltaX = startX - endX;
            int deltaY = startY - endY;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return this.Ok(distance);
        }
    }
}
