namespace TeamMerge.Utils
{
    public interface IConfigHelper
    {
        void AddValue<T>(string key, T value);
        T GetValue<T>(string key);
        void SaveDictionary();
    }

    //Created for unittesting
    public class ConfigHelper 
        : IConfigHelper
    {
        public T GetValue<T>(string key)
        {
            return ConfigManager.GetValue<T>(key);
        }

        public void AddValue<T>(string key, T value)
        {
            ConfigManager.AddValue(key, value);
        }

        public void SaveDictionary()
        {
            ConfigManager.SaveDictionary();
        }
    }
}
