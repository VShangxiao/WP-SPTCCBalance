using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DBCSCodePage;
using HtmlAgilityPack;

namespace SptccBalance.Core
{
    public class HtmlSptccClient : ISptccClient
    {
        public string ServiceEndpointUrl { get; set; }

        private string CardNumber { get; set; }

        public HtmlSptccClient()
        {
            ServiceEndpointUrl = @"http://jtk.sptcc.com:8080/servlet";
        }

        public async Task<Response<SearchResult>> DoSingleQuery(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return new Response<SearchResult>() { IsSuccess = false, Message = "请输入卡号" };
            }
            CardNumber = cardNumber;

            try
            {
                var rawHtml = await GetRawHtmlResponse();
                if (string.IsNullOrWhiteSpace(rawHtml))
                {
                    return new Response<SearchResult>() { IsSuccess = false, Message = "请检查卡号及格式是否正确" };
                }
                var result = new SearchResult() { CardNumber = cardNumber };
                var doc = new HtmlDocument();
                doc.LoadHtml(rawHtml);

                // Date
                var greenBoldTitles =
                    doc.DocumentNode.Descendants().Where(d => d.GetAttributeValue("class", "") == "greenBoldTitle").ToList();
                if (greenBoldTitles.Count == 2)
                {
                    var date = greenBoldTitles.ToList()[1].InnerText;
                    result.Date = date.Trim().Replace(" ", string.Empty);
                }

                // Balance
                var orangeNumbers =
                    doc.DocumentNode.Descendants().Where(d => d.GetAttributeValue("class", "") == "orangeNumber").ToList();
                if (orangeNumbers.Count == 2)
                {
                    var balance = orangeNumbers.ToList()[1].InnerText;
                    result.Balance = double.Parse(balance.Trim());
                }

                return new Response<SearchResult>()
                {
                    IsSuccess = true,
                    Item = result
                };
            }
            catch (Exception ex)
            {
                return new Response<SearchResult>() { IsSuccess = false, Exception = ex, Message = ex.Message };
            }
        }

        private async Task<string> GetRawHtmlResponse()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServiceEndpointUrl);
                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("Card_id", CardNumber),
                    new KeyValuePair<string, string>("hiddentype", "s_index"),
                    new KeyValuePair<string, string>("User_name", ""),
                    new KeyValuePair<string, string>("Pass_word", ""),
                    new KeyValuePair<string, string>("addr", "210.13.74.124"),
                    new KeyValuePair<string, string>("x", "21"),
                    new KeyValuePair<string, string>("y", "46")
                });

                var result = await client.PostAsync("", content);

                var resultContent = await result.Content.ReadAsByteArrayAsync();
                return DBCSEncoding.GetDBCSEncoding("gb2312").GetString(resultContent, 0, resultContent.Length - 1);
            }
        }
    }
}
