using Newtonsoft.Json;

namespace esp32_client.Utils
{
    public static class Utils
    {
        public static string GetBackDirectory(string? OriginDirectory)
        {
            string backDirString = "";

            if (OriginDirectory is null) return backDirString;

            var backDir = OriginDirectory.Split('/').ToList();
            if (backDir.Count > 1)
            {
                backDirString = string.Join('/', backDir.Take(backDir.Count - 1));
            }
            return backDirString;
        }

        public static bool TryParseObjectToDictionary(object obj, out Dictionary<string, object> dict)
        {
            dict = new Dictionary<string, object>();
            try
            {
                dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(obj)) ?? new Dictionary<string, object>();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        public static object? ChangeType(object value, Type conversion)
        {
            Type? t = conversion;
            if (t is not null)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (string.IsNullOrEmpty(value?.ToString()))
                    {
                        return null;
                    }

                    t = Nullable.GetUnderlyingType(t);

                    if (t is null)
                        return null;
                }
                var rs = Convert.ChangeType(value, t);
                return rs;
            }
            return null;
        }
    }
}