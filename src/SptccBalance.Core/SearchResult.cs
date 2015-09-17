using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SptccBalance.Core
{
    public class SearchResult
    {
        public string CardNumber { get; set; }
        public double Balance { get; set; }

        public string Date { get; set; }

        public SearchResult()
        {
            Date = DateTime.Today.ToString("yyyy年 MM月 dd日");
        }

        public override string ToString()
        {
            return string.Format("卡号: {0}, 余额:{1} (截止{2})" + Environment.NewLine, CardNumber, Balance, Date);
        }
    }
}
