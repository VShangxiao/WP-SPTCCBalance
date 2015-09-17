using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using SptccBalance.Common;
using GalaSoft.MvvmLight.Command;
using SptccBalance.Core;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace SptccBalance.ViewModel
{
    public class MyCardsViewModel : ViewModelBase
    {
        private readonly Repository _repository = new Repository();

        private ObservableCollection<Card> _cardList;

        public ObservableCollection<Card> CardList
        {
            get { return _cardList; }
            set
            {
                _cardList = value;
                RaisePropertyChanged();
            }
        }

        private string _cardNumber;

        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                RaisePropertyChanged();
            }
        }

        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; RaisePropertyChanged(); }
        }

        public RelayCommand CommandSave { get; set; }

        public RelayCommand CommandDelete { get; set; }

        public Task InitDataTask { get; set; }

        public MyCardsViewModel()
        {
            CardList = new ObservableCollection<Card>();
            InitDataTask = InitData();
            CommandSave = new RelayCommand(async () => await AddItemAsync());
            CommandDelete = new RelayCommand(async () => await DeleteItems());

            if (IsInDesignMode)
            {
                CardList.Add(new Card() { CardNumber = "96777213213", Comment = "abc" });
                CardList.Add(new Card() { CardNumber = "96712321888", Comment = "fuck shit" });
                CardList.Add(new Card() { CardNumber = "14561677723", Comment = "dick" });
            }
        }

        private async Task AddItemAsync()
        {
            if (null != CardNumber && CardList.All(i => i.CardNumber != CardNumber))
            {
                CardList.Add(new Card()
                {
                    CardNumber = CardNumber,
                    Comment = Comment
                });
                await _repository.SaveDataAsync(CardList);
            }

            CardNumber = string.Empty;
            Comment = string.Empty;
        }

        private async Task DeleteItems()
        {
            CardList = CardList.Where(i => !i.IsSelected).ToObservableCollection();
            await _repository.SaveDataAsync(CardList);
        }

        #region Data Access

        private async Task InitData()
        {
            var cards = await _repository.LoadDataAsync();
            if (cards.Any())
            {
                CardList.Clear();
                foreach (var card in cards)
                {
                    CardList.Add(card);
                }
            }
        }

        #endregion
    }
}
