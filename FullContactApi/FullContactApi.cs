using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FullContactApi

{// namespace


	public interface IFullContactAPi
	{
		Task<FullContactPerson> LookupPersonByEmailAsync(string email);
	}


	public class FullContactPerson
	{
		public ContactInfo contactInfo;
		public Demographic demographics;
		public List<SocialProfile> socialProfiles;
	}


	public class ContactInfo
	{
		public string familyName { get; set; }
		public string fullName { get; set; }
		public string givenName { get; set; }
		public List<Website> websites;
		public List<Chat> chats;
	}


	public class Website
	{
		public string url { get; set; }
	}

	public class Demographic
	{
		public double likelihood { get; set; }
	}

	public class Chat
	{
		public string client { get; set; }
		public string handle { get; set; }
	}

	public class SocialProfile
	{
		public string bio { get; set; }
		public string type { get; set; }
		public string typeId { get; set; }
		public string typeName { get; set; }
		public string url { get; set; }
		public string username { get; set; }
	}


}//namespace

