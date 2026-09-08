using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Cate.Hubs
{
    public class NotifyPOHub : Hub
    {
        public void NotifyPayment(string billNumber)
        {
            Clients.All.paymentSuccess(billNumber);
        }
    }
}
