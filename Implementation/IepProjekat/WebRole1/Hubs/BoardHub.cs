using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebRole1.Hubs
{
    public class BoardHub : Hub
    {
        public void hello1()
        {
            Clients.All.hello();
        }
    }
}