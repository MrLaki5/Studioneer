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

        public void joingroup(string groupName)
        {
            this.Groups.Add(this.Context.ConnectionId, groupName);
        }

        public void updategroup(string group, string id, string channame, string title, string text, string answers, string time) {
            Clients.Group(group).boardfresh(id, channame, title, text, answers, time);
        }
    }
}