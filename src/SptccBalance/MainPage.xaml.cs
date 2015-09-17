using System;
using System.Linq;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SptccBalance.Common;
using SptccBalance.Core;
using SptccBalance.ViewModel;

namespace SptccBalance
{
    public sealed partial class MainPage : Page
    {
        private AppSettings _settings;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            _settings = new AppSettings();
            UIHelper.ShowSystemTrayAsync(Color.FromArgb(255, 48, 169, 62), Colors.White);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;

            var vm = this.DataContext as MainPageViewModel;
            if (null != vm)
            {
                await vm.InitData();

                if (vm.Cards.Any())
                {
                    CbCards.ItemsSource = vm.Cards;

                    if (!string.IsNullOrWhiteSpace(_settings.LastQueryCardNumber))
                    {
                        var lastCard = vm.Cards.FirstOrDefault(c => c.CardNumber == _settings.LastQueryCardNumber);
                        CbCards.SelectedItem = lastCard ?? vm.Cards[0];
                    }
                    else
                    {
                        CbCards.SelectedItem = vm.Cards[0];
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;
        }

        async private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;

            if (!Frame.CanGoBack)
            {
                Application.Current.Exit();
            }
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }

        private async void MenuReview_Click(object sender, RoutedEventArgs e)
        {
            await Tasks.OpenReviewAsync();
        }

        private void BtnMyCards_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyCards));
        }

        private void CbCards_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = this.DataContext as MainPageViewModel;
            if (null != vm)
            {
                vm.SelectedCard = (Card)CbCards.SelectedItem;
            }
        }
    }
}
