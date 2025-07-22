using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Utils
{
    public class RedisHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["RedisConfiguration"];
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection => lazyConnection.Value;

        public static IDatabase Database => Connection.GetDatabase();

        // 序列化为 JSON 字符串
        private static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        // 反序列化为对象
        private static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        // 缓存 List 数据
        public static void SetList<T>(string key, List<T> list, TimeSpan expiry)
        {
            var db = Database;
            string json = SerializeObject(list);
            db.StringSet(key, json, expiry);
        }

        // 获取缓存的 List 数据
        public static List<T> GetList<T>(string key)
        {
            var db = Database;
            string json = db.StringGet(key);

            if (json == null)
                return null;

            return DeserializeObject<List<T>>(json);
        }
    }
}