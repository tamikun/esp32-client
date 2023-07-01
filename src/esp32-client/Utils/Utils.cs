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
    }
}