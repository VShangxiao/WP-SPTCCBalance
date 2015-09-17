using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace SptccBalance.Core
{
    public class AppSettings : INotifyPropertyChanged
    {
        private const string LastQueryCardNumberKeyName = "LastQueryCardNumberSettings";

        public string LastQueryCardNumber
        {
            get
            {
                return ReadSettings<string>(LastQueryCardNumberKeyName);
            }
            set
            {
                SaveSettings(LastQueryCardNumberKeyName, value);
                NotifyPropertyChanged();
            }
        }

        public ApplicationDataContainer LocalSettings { get; set; }

        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            return default(T);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
