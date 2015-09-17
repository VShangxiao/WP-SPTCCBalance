using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace SptccBalance.Common
{
    public class UIHelper
    {
        public static async Task ShowSystemTrayAsync(Color backgroundColor, Color foregroundColor,
            double opacity = 1, string text = "", bool isIndeterminate = false, bool showProgress = false)
        {
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = backgroundColor;
            statusBar.ForegroundColor = foregroundColor;
            statusBar.BackgroundOpacity = opacity;

            if (showProgress)
            {
                statusBar.ProgressIndicator.Text = text;
                if (!isIndeterminate)
                {
                    statusBar.ProgressIndicator.ProgressValue = 0;
                }
                await statusBar.ProgressIndicator.ShowAsync();
            }
        }

        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }

    public static class CollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll)
            {
                c.Add(e);
            }
            return c;
        }
    }
}
