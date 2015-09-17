using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System;

namespace SptccBalance.Common
{
    public class Tasks
    {
        public static async Task OpenWebsiteAsync(string url)
        {
            await Launcher.LaunchUriAsync(new Uri(url, UriKind.Absolute));
        }

        public static async Task OpenReviewAsync()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }

        public static async Task OpenEmailComposeAsync(string toAddress, string subject, string body)
        {
            var uri = new Uri(string.Format("mailto:?to={0}&subject={1}&body={2}", toAddress, subject, body), UriKind.Absolute);
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
