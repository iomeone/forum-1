﻿using HtmlAgilityPack;
using System;
using System.Net;

namespace Forum3.Services {
	public class GzipWebClient : WebClient {
		public HtmlDocument DownloadDocument(string remoteUrl) {
			var siteWithoutHash = remoteUrl.Split('#')[0];

			var document = new HtmlDocument();

			try {
				var data = DownloadString(siteWithoutHash);

				if (string.IsNullOrEmpty(data))
					return null;

				document.LoadHtml(data);
			}
			catch (UriFormatException) { }
			catch (AggregateException) { }
			catch (ArgumentException) { }
			catch (WebException) { }

			return document;
		}

		protected override WebRequest GetWebRequest(Uri address) {
			ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

			var request = base.GetWebRequest(address) as HttpWebRequest;

			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246";
			request.AllowAutoRedirect = true;
			request.MaximumAutomaticRedirections = 3;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.Timeout = 5000;
			request.CookieContainer = new CookieContainer();

			return request;
		}
	}
}
