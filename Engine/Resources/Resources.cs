namespace GameInConsoleEngine.Resources
{
    public static class Resources
    {
        private static Dictionary<string, ResourceEntry> resourceEntries = new Dictionary<string, ResourceEntry>();

        public static void Load<T>(string path, string key)
        {
            resourceEntries.Add(key, GetResourceEntry(path, typeof(T)));
        }
        public static void Load(string path, string key)
        {
            resourceEntries.Add(key, GetResourceEntry(path, null));
        }

        public static ResourceEntry GetEntry(string key)
        {
            if (!resourceEntries.ContainsKey(key))
            {
                throw new Exception();
            }
            return resourceEntries[key];
        }

        private static ResourceEntry GetResourceEntry(string path, Type type)
        {
            ResourceEntry entry = new ResourceEntry();

            entry.ResourcePath = path;
            entry.ResourceType = type;

            return entry;
        }
    }
}
