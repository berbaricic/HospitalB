using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentAPI
{
    public class RedisStore
    {
        public static string GetRedisKey(string id)
        {
            return $"appointment:{id}";
        }

    }
}
