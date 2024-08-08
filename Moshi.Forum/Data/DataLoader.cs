using System.Reflection;

namespace Moshi.Forums.Data
{
    public static class DataLoader
    {
        public static List<string> LoadData(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Moshi.Forums.Data.SampleContent.{fileName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var data = new List<string>();
                while (!reader.EndOfStream)
                {
                    data.Add(reader.ReadLine());
                }
                return data;
            }
        }
    }
}
