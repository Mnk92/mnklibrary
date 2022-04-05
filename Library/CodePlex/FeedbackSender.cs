using System;
using System.Net;
using System.Web;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Network;
using ServiceStack.Text;

namespace Mnk.Library.CodePlex
{
    public class FeedbackSender : IFeedbackSender
    {
        private readonly string application;
        private readonly string serverUrl;
        private readonly ILog log = LogManager.GetLogger<FeedbackSender>();
        private readonly Request request = new Request();

        public FeedbackSender(string application, string serverUrl)
        {
            this.application = application;
            this.serverUrl = serverUrl;
        }

        public bool Send(string title, string body)
        {
            var url = string.Format(serverUrl + "/send/{0}", HttpUtility.UrlPathEncode(application + " " + title));
            var result = request.GetResult(url,
                              HttpMethod.POST,
                              JsonSerializer.SerializeToString(TrimBody(body)),
                              new[]
								  {
									  new Header("Content-Type", "application/json; charset=utf-8"),
									  new Header("Accept-Encoding", "gzip,deflate")
								  }
                              );
            if (result.HttpStatusCode != HttpStatusCode.OK)
            {
                log.Write("Can't send feedback, status code: " + result.HttpStatusCode);
                return false;
            }
            return true;
        }

        private static string TrimBody(string body)
        {
            const int max = UInt16.MaxValue-3;
            if (body.Length > max)
            {
                return body.Substring(body.Length - max);
            }
            return body;
        }
    }
}
