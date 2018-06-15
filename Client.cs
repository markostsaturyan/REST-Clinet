using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using System.Xml;

namespace RESTClient
{
    public static class Client
    {
        private static HttpClient client= new HttpClient();

        public static IEnumerable<T> Get<T>(Uri uri)
        {
            var task = client.GetStringAsync(uri);

            var result = task.Result;

            IEnumerable<T> resultT;

            if (result[0] == '<')
            {
                List<T> tObjects = new List<T>();

                XmlDocument document = new XmlDocument();

                document.LoadXml(result);

                XmlNodeList nodeList = document.GetElementsByTagName(default(T).GetType().Name.ToLower());

                foreach (XmlNode node in nodeList)
                {
                    T tObject = default(T);

                    foreach (XmlNode xmlAtr in node.ChildNodes)
                    {
                        tObject.GetType().GetProperty(xmlAtr.Name).SetValue(tObject, xmlAtr.InnerText, null);
                    }
                    tObjects.Add(tObject);
                }

                resultT= tObjects;
            }
            else
            {
                resultT = JsonConvert.DeserializeObject<T[]>(result);
            }
            return resultT; 
        }

        public static void Put<T>(T item, Uri uri, Dictionary<string,object> parameters)
        {
            var builder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(builder.Query);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query[param.Key] = param.Value.ToString();
                }
            }

            builder.Query = query.ToString();

            var json = JsonConvert.SerializeObject(item);

            client.PutAsync(builder.ToString(), new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public static void Post<T>(T item, Uri uri, Dictionary<string, object> parameters)
        {
            var builder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(builder.Query);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query[param.Key] = param.Value.ToString();
                }
            }

            builder.Query = query.ToString();

            var json = JsonConvert.SerializeObject(item);

            client.PostAsync(builder.ToString(), new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public static void Delete(Uri uri)
        {
            client.DeleteAsync(uri);
        }
    }
}
