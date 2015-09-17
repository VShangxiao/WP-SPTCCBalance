using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SptccBalance.Core
{
    public interface ISptccClient
    {
        string ServiceEndpointUrl { get; set; }

        Task<Response<SearchResult>> DoSingleQuery(string cardNumber);
    }
}
