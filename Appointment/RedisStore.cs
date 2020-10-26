using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentLibrary
{
    public class RedisStore
    {
        public static string GetRedisKey(string id)
        {
            return $"appointment:{id}";
        }
    }
}
