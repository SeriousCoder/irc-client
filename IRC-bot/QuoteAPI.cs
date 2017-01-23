using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace IRC_bot
{
	public class Quote
	{
		public string quoteText;
		private string quoteAutor;
		private string senderName;
		private string senderLink;
	}

	public class QuoteAPI
	{
		public static string GetQuote()
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://api.forismatic.com/api/1.0/POST?method=getQuote&key=457653&format=json&lang=ru");
			
			//request.Method = "POST:\r\nmethod=getQuote&key=457653&format=xml&lang=ru\r\n";

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream responseStream = response.GetResponseStream();

			StreamReader reader = new StreamReader(responseStream);
			//DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
			var str = reader.ReadToEnd();//(string)serializer.ReadObject(responseStream);
			var quote = JsonConvert.DeserializeObject<Quote>(str);


			responseStream.Close();
			reader.Close();
			response.Close();

			return quote.quoteText;
		}
	}
}