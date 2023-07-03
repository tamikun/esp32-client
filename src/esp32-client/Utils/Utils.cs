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
                dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(obj));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}