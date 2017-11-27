using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using FullContactApi;


namespace webApiClient
{

	class Program:FullContactApi.IFullContactAPi
	{

      static HttpClient client = new HttpClient();


		static void Main()
		{

			bool programControl = true;
			string userInput;

			while (programControl)
			{
				Console.WriteLine("******************");
				Console.WriteLine("1: (Please provide an email address and press enter on the newline to get contact information)");
				Console.WriteLine("2: (enter '.exit' and press enter to close the program)");
				Console.WriteLine("******************");
				userInput = Console.ReadLine();

				 if (userInput.Trim()==".exit"){
					Console.WriteLine("exiting ...");
					programControl = false;

				} else if (Console.ReadKey(true).Key == ConsoleKey.Enter){

					RegexUtilities regexUtil = new RegexUtilities();
					// check if valide email
					if (regexUtil.IsValidEmail(userInput))
					{
						Console.WriteLine("processing ...");
						RunAsync(userInput).Wait();
					}else{
						Console.WriteLine("Please provide a valid emaill address");
					    continue;
					}
						}else{
					         // loop
					         continue;
				}
			}
		}



		static async Task RunAsync(string emaillAddress)
		{

			try
			{

				Program p = new Program();
				// get person details
				FullContactPerson personDetails = await p.LookupPersonByEmailAsync(emaillAddress);
				// print details
				printFullContactPerson(personDetails);
			}

			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}



		public async Task<FullContactPerson> LookupPersonByEmailAsync(string email)
		{

			FullContactPerson personDetails = null;

			string url = "https://api.fullcontact.com/v2/person.json?email="+email+"&apiKey=e0382def1e9359e9";
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


           HttpResponseMessage response = await client.GetAsync(url);

			if (response.IsSuccessStatusCode){
		      personDetails = await response.Content.ReadAsAsync<FullContactPerson>();
			}

			// return personDetails obj
			return personDetails;
		}

		// print retrieved person details
		 static void printFullContactPerson(FullContactPerson personDetails)
		{

			try
			{


			Console.WriteLine("********* Contact Info ********* ");

				// contact info

				ContactInfo _contactInfo;

				if (personDetails.contactInfo != null)
				{
					 _contactInfo = personDetails.contactInfo;

					Console.WriteLine("---------------------");
			        Console.WriteLine("familyname: "+_contactInfo.familyName);
			        Console.WriteLine("full name: "+_contactInfo.fullName);
			        Console.WriteLine("given name: "+_contactInfo.givenName);
			        Console.WriteLine("---------------------");

				}else{
					throw new Exception(" we could not find any information, try another email address");
				}

			// chats info
				if (_contactInfo.chats != null)
			{
					if (_contactInfo.chats.Count > 0)
					{
						List<Chat> _chatInfo = _contactInfo.chats;
						Console.WriteLine("*********  Chat Info ********* ");
						for (int i = 0; i<_chatInfo.Count; i++){
					Console.WriteLine("---------------------");
					Console.WriteLine("client: "+_chatInfo[i].client);
					Console.WriteLine("handle: "+_chatInfo[i].handle);
					Console.WriteLine("---------------------");
						}

					}
		
			}

				// websites info
				if (_contactInfo.websites!=null)
				{
						if (_contactInfo.websites.Count > 0)
			{
				List<Website> _websiteInfo = _contactInfo.websites;

					Console.WriteLine("*********  Website Info ********* ");
				for (int i = 0; i<_websiteInfo.Count; i++)
				{
					Console.WriteLine("---------------------");
					Console.WriteLine("url: "+_websiteInfo[i].url);
					Console.WriteLine("---------------------");
				}
				
			}

				}


				if (personDetails.demographics!= null)
				{
					// demographic info
					Demographic _demographiInfo = personDetails.demographics;
					Console.WriteLine("*********  Demographic Info ********* ");
					Console.WriteLine("---------------------");
					Console.WriteLine("likelihood: "+_demographiInfo.likelihood.ToString());
					Console.WriteLine("---------------------");
				}



				if (personDetails.socialProfiles!= null)
				{

			// social profiles info
					if (personDetails.socialProfiles.Count>0)
			{
				List<SocialProfile> _socialProfileInfo = personDetails.socialProfiles;

					Console.WriteLine("*********  Social Profiles Info ********* ");
				for (int i = 0; i<_socialProfileInfo.Count; i++)
				{
					Console.WriteLine("---------------------");
					Console.WriteLine("username: "+_socialProfileInfo[i].username);
					Console.WriteLine("bio: "+_socialProfileInfo[i].bio);
					Console.WriteLine("type: "+_socialProfileInfo[i].type);
					Console.WriteLine("typeId: "+_socialProfileInfo[i].typeId);
					Console.WriteLine("typeName: "+_socialProfileInfo[i].typeName);
					Console.WriteLine("url: "+_socialProfileInfo[i].url);
					Console.WriteLine("---------------------");
				}

			}


				}


			}

			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("---------------------");
			}
		}
	}


// utility class 

public class RegexUtilities
{
	bool invalid = false;

	public bool IsValidEmail(string strIn)
	{
		invalid = false;
		if (String.IsNullOrEmpty(strIn))
			return false;

		// Use IdnMapping class to convert Unicode domain names.
		strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
		if (invalid)
			return false;

		// Return true if strIn is in valid e-mail format.
		return Regex.IsMatch(strIn,
			   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
			   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
			   RegexOptions.IgnoreCase);
	}

	private string DomainMapper(Match match)
	{
		// IdnMapping class with default property values.
		IdnMapping idn = new IdnMapping();

		string domainName = match.Groups[2].Value;
		try
		{
			domainName = idn.GetAscii(domainName);
		}
		catch (ArgumentException)
		{
			invalid = true;
		}
		return match.Groups[1].Value + domainName;
   }
}

}
