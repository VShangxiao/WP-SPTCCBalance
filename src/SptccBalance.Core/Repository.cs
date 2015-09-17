using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace SptccBalance.Core
{
    public class Repository
    {
        private const string Jsonfilename = "data.json";
        public async Task SaveDataAsync(ObservableCollection<Card> cardList)
        {
            try
            {
                List<CardItemSerializeType> data =
                cardList.Select(i => new CardItemSerializeType
                {
                    CardNumber = i.CardNumber,
                    Comment = i.Comment
                }).ToList();

                var serializer = new DataContractJsonSerializer(typeof(List<CardItemSerializeType>));
                using (Stream stream = await ApplicationData.Current.LocalFolder
                                             .OpenStreamForWriteAsync(Jsonfilename, CreationCollisionOption.ReplaceExisting))
                {
                    serializer.WriteObject(stream, data);
                }
            }
            catch (Exception e)
            {
                var dig = new MessageDialog(e.Message, "保存爆了");
                dig.ShowAsync();
            }
        }
        public async Task<List<Card>> LoadDataAsync()
        {
            try
            {
                Stream ms = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(Jsonfilename);
                var serializer = new DataContractJsonSerializer(typeof(List<CardItemSerializeType>));
                object obj = serializer.ReadObject(ms);
                if (null != obj)
                {
                    var ids = obj as List<CardItemSerializeType>;
                    if (null != ids)
                    {
                        IEnumerable<Card> items = ids.Select(i => new Card()
                        {
                            CardNumber = i.CardNumber, 
                            Comment = i.Comment
                        });
                        return items.ToList();
                    }
                }
                return new List<Card>();
            }
            catch (Exception)
            {
                return new List<Card>();
            }
        }
    }

    [DataContract]
    public class CardItemSerializeType
    {
        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}
