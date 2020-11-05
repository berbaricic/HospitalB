using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentLibrary
{
    public class RedisStore
    {
        public static string GetRedisKey(string doctorId, string id)
        {
            return $"doctor:{doctorId}/appointment:{id}";
        }
    }
}
