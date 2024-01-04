using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using SportNow.Model;
using System.Collections.ObjectModel;

namespace SportNow.Services.Data.JSON
{
	public class SponsorManager
    {
		//IRestService restService;

		HttpClient client;
		

		public SponsorManager()
		{
			HttpClientHandler clientHandler = new HttpClientHandler();
			clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
			client = new HttpClient(clientHandler);

		}

		public async Task<List<Sponsor>> GetSponsors()
		{
			Debug.WriteLine("SponsorManager.GetSponsors " + Constants.RestUrl_Get_Sponsors);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Get_Sponsors, string.Empty));
			try
			{
				HttpResponseMessage response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					//return true;
					string content = await response.Content.ReadAsStringAsync();
					List<Sponsor> sponsors = JsonConvert.DeserializeObject<List<Sponsor>>(content);
					return sponsors;
				}
				else
				{
					Debug.WriteLine("SponsorManager.GetSponsors - error getting sponsors");
					return null;
				}

				return null;
			}
			catch (Exception e)
			{
				Debug.WriteLine("SponsorManager.GetSponsors http request error " + e.ToString());
				return null;
			}
		}

		public async Task<string> CreateMbWayPayment(string memberid, string paymentid, string orderid, string phonenumber, string value, string email)
		{
			Debug.Print("CreateMbWayPayment begin "+ Constants.RestUrl_Create_MbWay_Payment + "?userid=" + memberid + "&paymentid=" + paymentid + "&phonenumber=" + phonenumber + "&value=" + value + "&email=" + email + "&orderid=" + orderid);
			Uri uri = new Uri(string.Format(Constants.RestUrl_Create_MbWay_Payment + "?userid=" + memberid + "&paymentid=" + paymentid + "&phonenumber=" + phonenumber + "&value=" + value + "&email=" + email + "&orderid=" + orderid, string.Empty));
			try {
				HttpResponseMessage response = await client.GetAsync(uri);
				string content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("content=" + content);

				if (response.IsSuccessStatusCode)
				{
					//return true;
					//string content = await response.Content.ReadAsStringAsync();
					Debug.WriteLine("content=" + content);
					List<Result> createResultList = JsonConvert.DeserializeObject<List<Result>>(content);

					return createResultList[0].result;

				}
				else
				{
					Debug.WriteLine("error creating payment MBWay");
					return "-2";
				}

			}
			catch
			{
				Debug.WriteLine("http request error");
				return "-3";
			}
		}


	}
}