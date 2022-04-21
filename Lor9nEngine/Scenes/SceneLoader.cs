using Newtonsoft.Json;

namespace Lor9nEngine.Scenes
{
    internal class SceneLoader
    {
        public const string FILE_EXTENSION = ".lor";
        public const string FILENAME = "Scene";

        private static async void WriteToJsonFile<T>(string filePath, T objectToWrite)
        {
            if (!File.Exists(filePath))
            {
                var read = File.Create(filePath);
                read.Close();
            }
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer();
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                serializer.Formatting = Formatting.Indented;
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.MaxDepth = 1;
                serializer.Serialize(writer, objectToWrite);
            }
        }

      
        private static T ReadFromJsonFile<T>(string filePath)
        {
            using var sr = new StreamReader(filePath);
            using var jsonReader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            return serializer.Deserialize<T>(jsonReader);
        }
          

        public static void SaveScene(Scene scene)
        {
            Thread t = new Thread(async () =>
            {
                WriteToJsonFile(@"Scenes\" + scene.Name + FILE_EXTENSION, scene);
            });
            t.Start();
        }

        public static Scene LoadScene(string name) => ReadFromJsonFile<Scene>(name + FILE_EXTENSION);
        public static Scene LoadSceneByPath(string path) => ReadFromJsonFile<Scene>(path);
    }
}
