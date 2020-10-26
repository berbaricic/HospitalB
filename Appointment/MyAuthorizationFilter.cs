using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentLibrary
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
