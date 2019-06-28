//# SiaClassicExplorer
//** An Explorer for SiaClassic blockchain in C# and .Net Framework **
//* Copyright(C) 2018-2019 Eugene Antonov*
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of version 3 of the GNU General Public License
//as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<https://www.gnu.org/licenses/>.

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ClassLibrary
{
    public class WebClient
    {
        private readonly string baseUri;
        private readonly string userAgent;

        public WebClient(string baseUri, string userAgent = null)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new ArgumentException("IsNullOrWhiteSpace(baseUri)", nameof(baseUri));

            this.baseUri = baseUri;
            this.userAgent = userAgent;
        }

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUri + uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = userAgent;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public object GetObject(string uri) => JsonConvert.DeserializeObject(Get(uri));

        public string Post(string uri, string data, string contentType, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUri + uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = userAgent;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                requestBody.Write(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
