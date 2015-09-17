using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using SptccBalance.Common;
using SptccBalance.Core;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace SptccBalance.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly Repository _repository = new Repository();
        private readonly AppSettings _settings;
        public ISptccClient SptccClient { get; private set; }

        #region Binding Properties

        private Card _selectedCard;

        public Card SelectedCard
        {
            get { return _selectedCard; }
            set { _selectedCard = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<SearchResult> _searchResults;

        public ObservableCollection<SearchResult> SearchResults
        {
            get { return _searchResults; }
            set
            {
                _searchResults = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Card> _cards;

        public ObservableCollection<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Commands

        public RelayCommand CommandSearch { get; set; }

        public RelayCommand CommandEmailSearchResults { get; set; }

        private async Task DoEmail()
        {
            var emailContent = string.Join(Environment.NewLine, SearchResults);
            await Tasks.OpenEmailComposeAsync(string.Empty, string.Empty, emailContent);
        }

        private async Task DoSearchAsync()
        {
            try
            {
                if (null == SelectedCard)
                {
                    var dig = new MessageDialog("首先，你得选择一张卡", "你TM在逗我？");
                    await dig.ShowAsync();
                    return;
                }

                bool isConnected = UIHelper.IsInternet();
                if (!isConnected)
                {
                    var dig = new MessageDialog("请检查网络连接", "你TM在逗我？");
                    await dig.ShowAsync();
                    return;
                }

                await ShowBusy("正在查询");

                var response = await SptccClient.DoSingleQuery(SelectedCard.CardNumber);
                if (response.IsSuccess)
                {
                    _settings.LastQueryCardNumber = response.Item.CardNumber;
                    SearchResults.Add(response.Item);
                }
                else
                {
                    var dig = new MessageDialog(response.Message, "查询失败");
                    await dig.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var dig = new MessageDialog(ex.Message, "爆了");
                dig.ShowAsync();
            }
            finally
            {
                HideBusy();
            }
        }

        #endregion

        public Task InitDataTask { get; set; }

        public MainPageViewModel()
        {
            SptccClient = new HtmlSptccClient();
            SearchResults = new ObservableCollection<SearchResult>();
            Cards = new ObservableCollection<Card>();

            if (IsInDesignMode)
            {
                SearchResults.Add(new SearchResult()
                {
                    CardNumber = "96777213213",
                    Balance = 253.38,
                    Date = "2015年01月21日"
                });

                SearchResults.Add(new SearchResult()
                {
                    CardNumber = "34696713319",
                    Balance = 400.12,
                    Date = "2015年01月21日"
                });
            }
            else
            {
                _settings = new AppSettings();
                InitDataTask = InitData();
            }

            CommandSearch = new RelayCommand(async () => await DoSearchAsync());
            CommandEmailSearchResults = new RelayCommand(async () => await DoEmail());
        }

        public async Task InitData()
        {
            await ShowBusy("加载中");

            var cards = await _repository.LoadDataAsync();
            if (cards.Any())
            {
                Cards.Clear();
                foreach (var card in cards)
                {
                    Cards.Add(card);
                }
            }

            await HideBusy();
        }

        #region Helpers

        private async Task ShowBusy(string message)
        {
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.ProgressIndicator.Text = message;
            await statusBar.ProgressIndicator.ShowAsync();
        }

        private async Task HideBusy()
        {
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.ProgressIndicator.Text = "查询完成";
            await statusBar.ProgressIndicator.HideAsync();
        }

        #endregion
    }
}