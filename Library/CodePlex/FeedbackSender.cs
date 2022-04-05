﻿using System;
using System.Net;
using System.Web;
using System.Text.Json;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Network;

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
                              JsonSerializer.Serialize(TrimBody(body)),
                              new[]
                                  {
                                      new Header("Content-Type", "application/json; charset=utf-8"),
                                      new Header("Accept-Encoding", "gzip,deflate")
                                  }
                              );
            if (result.HttpStatusCode == HttpStatusCode.OK) return true;
            log.Write("Can't send feedback, status code: " + result.HttpStatusCode);
            return false;
        }

        private static string TrimBody(string body)
        {
            const int max = UInt16.MaxValue - 3;
            return body.Length > max ? body.Substring(body.Length - max) : body;
        }
    }
}
