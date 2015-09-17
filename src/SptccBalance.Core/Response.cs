using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SptccBalance.Core
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public Response()
        {
            IsSuccess = false;
            Message = string.Empty;
        }
    }

    public class Response<T> : Response
    {
        public T Item { get; set; }
    }

    public static class ResponseExtension
    {
        public static string GetExceptionDetailMessages(this Response r)
        {
            var sb = new StringBuilder();
            var ex = r.Exception;

            if (null != ex)
            {
                sb.Append(ex.Message);
                while (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    sb.Append(" | " + ex.Message);
                }
            }

            string message = sb.ToString();
            return message;
        }
    }
}
