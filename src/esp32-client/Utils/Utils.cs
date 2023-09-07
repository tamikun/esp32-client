using System.Reflection;
using esp32_client.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace esp32_client.Utils
{
    public static class Utils
    {
        public static string GetBackDirectory(string OriginDirectory)
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
        public static object ChangeType(this object value, Type conversion)
        {
            if (value is null) return null;

            Type t = conversion;
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

        public static byte[] GetBytesFromFile(IFormFile file)
        {
            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            return fileBytes;
        }

        public static string GetContentType(string fileType)
        {
            string contentType = "";
            switch (fileType?.ToLower())
            {
                case "txt":
                    contentType = "text/plain";
                    break;
                case "ico":
                    contentType = "image/x-icon";
                    break;
                case "json":
                    contentType = "application/json";
                    break;
                case "css":
                    contentType = "text/css";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return contentType;
        }

        public static async Task WriteFile(IFormFile file, string directory)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                await File.WriteAllBytesAsync(directory, fileBytes);
            }
        }

        public static async Task DeleteFile(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFileSystemEntries(directoryPath);
                if (files.Any())
                {
                    var deleteTasks = files.Select(async file =>
                    {
                        await DeleteFile(file);
                    });
                    await Task.WhenAll(deleteTasks);
                }
                Directory.Delete(directoryPath);
            }
            else if (File.Exists(directoryPath))
            {
                File.Delete(directoryPath);
            }
            await Task.CompletedTask;
        }

        public static async Task<Dictionary<string, object>> GetControllerMethods()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var controllerTypes = assembly.GetTypes()
             .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
             .Where(s => s != typeof(BaseController) && s != typeof(TestApiController));

            var dict = new Dictionary<string, object>();

            var tasks = controllerTypes.Select(async controllerType =>
            {
                var data = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(s => s.ReturnType == typeof(Task<IActionResult>))
                    .Select(s => s.Name)
                    .ToList();

                dict.Add(controllerType.Name, data);
                await Task.CompletedTask;
            });

            await Task.WhenAll(tasks);
            return dict;
        }
    }
}