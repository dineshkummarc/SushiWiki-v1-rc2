namespace Wiki
{
    using System;
    using System.Collections ;
    using System.Web ;
    using System.Web.Configuration ;
	using System.Diagnostics;
	using System.Configuration;
	using System.Web.SessionState ;
	using System.Xml;
	using System.IO;

	/// <summary>
	/// This class privides user settings.
	/// 
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 07/06/2003 | EGE         | New class (some methods come from WikiSettings)
	/// | 0.2   | 01/12/2003 | EGE         | BUG fixed : local user was always admin. Internal user name is now "DOMAIN/NAME" and not any more "DOMAIN-NAME".
	/// </code>
	/// </summary>
    public class WikiUserSettings
    {
		#region Version management
		/// <summary>
		/// Version manangement : version
		/// </summary>
		public static string v = "0.2";
		/// <summary>
		///  Version manangement : release
		/// </summary>
		public static int r = 2;
		#endregion

		#region Singleton stuff

		/// <summary>
		/// static attribute containing singleton
		/// </summary>
		private static WikiUserSettings singleton = null;

		/// <summary>
		/// Return singleton
		/// </summary>
		/// <returns>singleton</returns>
		public static WikiUserSettings Singleton()
		{
			if (singleton == null)
				singleton = new WikiUserSettings();
			return singleton;
		}
#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		private WikiUserSettings()
        {
		}

		#region User Settings

		private void Log(char pType, string pLabel1, string pLabel2)
		{
			WikiManager.Singleton().Log(pType,"USRSTG",pLabel1,pLabel2);
		}

		/// <summary>
		/// Loads user settings from XML file.
		/// </summary>
		/// <param name="username">User name</param>
		/// <param name="localpath">Path where to store user settings</param>
		/// <returns>Hastable containing user settings</returns>
		private Hashtable GetUserSettings(string username)
		{
			Hashtable hash = new Hashtable();
			string file = WikiSettings.Singleton().LocalPath + "\\private\\" + username.Replace("\\","-") + ".userconf.xml";
			if (System.IO.File.Exists(file))
			{ // Load settings
				XmlTextReader data = new XmlTextReader(file);
				string sLastNodeName = "";
				while (data.Read())
				{
					if (data.NodeType == XmlNodeType.Element)
						sLastNodeName = data.Name;
					if (data.NodeType == XmlNodeType.Text)
					{
						switch (sLastNodeName)
						{
							case "SELECTED_PAGES":
								hash.Add("SELECTED_PAGES",data.Value);
								break;
							case "EMAIL":
								hash.Add("EMAIL",data.Value);
								break;
							case "EMAIL_FREQUENCE":
								hash.Add("EMAIL_FREQUENCE",data.Value);
								break;
							case "EMAIL_PAGES":
								hash.Add("EMAIL_PAGES",data.Value);
								break;
							case "USER_STYLE":
								hash.Add("USER_STYLE",data.Value);
								break;
							case "FULLSCREEN":
								hash.Add("FULLSCREEN",data.Value);
								break;
							case "DATE":
								hash.Add("DATE",data.Value);
								break;
						}
					}
				}
				data.Close();
			}
			return hash;
		}

		/// <summary>
		/// Loads user settings into the user Session
		/// </summary>
		/// <param name="session">User session</param>
		public void LoadUserSettings(HttpSessionState session,bool localuser)
		{
			// Retrieve user settings
			string username = (string)session["username"];
			Log('i',"LOAD","Loading user settings for <" + username + ">");
			// By default load guest user settings
			Hashtable hash = GetUserSettings(username);
			// Create user page if it doesn't exist
			if (!WikiManager.Singleton().GetPageShortInfo(UserPagePrefix + username).pageFound)
			{
				Log('i',"USER PAGE","Creating user page for <" + username + ">");
				WikiManager.PageData page = new WikiManager.PageData();
				page.title = UserPagePrefix + username;
				page.lastUpdated = DateTime.Now;
				page.updatedBy = username;
				page.lockedBy = "";
				page.ownedBy = username;
				page.pageData = GetUserPageData(username);
				page.type = "WIKI";
				page.publicAccess = false;
				WikiManager.Singleton().WriteNewPage(page);
			}
			// Copy them to user Session
			session["selectedpages"] = (string)hash["SELECTED_PAGES"];
			session["email"] = (string)hash["EMAIL"];
			session["email_frequence"] = (string)hash["EMAIL_FREQUENCE"];
			session["email_pages"] = (string)hash["EMAIL_PAGES"];
			session["userstyle"] = (string)hash["USER_STYLE"];
			session["fullscreen"] = (string)hash["FULLSCREEN"];
			string admins = "," + WikiSettings.Singleton().Administrators + ",";
			bool admin = ( (admins.IndexOf("," + username + ",") != -1) || ( (localuser) && (WikiSettings.Singleton().isLocalUserAdmin) ) );
			session["admin"] = admin;
			if (admin) Log('w',"ADMIN","This user (" + username + ") has administration privilege");
			// Default values
			if (session["userstyle"] == null) session["userstyle"] = "default.css";
		}

		private string GetUserPageData(string pName)
		{
			string data = "";
			Log('i',"NEW USER","Creating user page for <" + pName + ">");
			string fileName = WikiSettings.Singleton().LocalPath + "/install/" + WikiSettings.Culture.TwoLetterISOLanguageName + ".WikiUser.htm";
			try
			{	// Try to find localised ressource
				using (StreamReader sr = File.OpenText(fileName))
				{
					data = sr.ReadToEnd();
				}
			}
			catch
			{
				Log('w',"MANAGER","Localized user page template not found (" + fileName + "). Using backup (install/WikiUser.htm)");
				try
				{
					using (StreamReader sr = File.OpenText(WikiSettings.Singleton().LocalPath + "/install/WikiUser.htm"))
					{
						data = sr.ReadToEnd();
					}
				}
				catch
				{
					Log('w',"MANAGER","Packup user page template not found (install/WikiUser.htm). Creating blank page.");
					data = "---+++" + pName;
				}
			}
			return data.Replace("%NAME%",pName).Replace("%DATE%",DateTime.Now.ToLongDateString());
		}

		/// <summary>
		/// Save user settings from Session in a xml file.
		/// These settings ared stored in the /private subdirectory.
		/// </summary>
		/// <param name="session">Current Session</param>
		public void SaveUserSettings(HttpSessionState session)
		{
			// Create/overwrite file
			string username = (string)session["username"];
			string file = WikiSettings.Singleton().LocalPath + "\\private\\" + username.Replace("\\","-") + ".userconf.xml";
			XmlTextWriter data = new XmlTextWriter(file,System.Text.Encoding.UTF8);
			// Write XML data
			data.WriteStartElement("SUSHIWIKI");
			data.WriteStartElement("USER_SETTINGS");
			data.WriteElementString("SELECTED_PAGES",(string)session["selectedpages"]);
			data.WriteElementString("EMAIL",(string)session["email"]);
			data.WriteElementString("EMAIL_FREQUENCE",(string)session["email_frequence"]);
			data.WriteElementString("EMAIL_PAGES",(string)session["email_pages"]);
			data.WriteElementString("USER_NAME",(string)session["username"]);
			data.WriteElementString("USER_STYLE",(string)session["userstyle"]);
			data.WriteElementString("FULLSCREEN",(string)session["fullscreen"]);
			data.WriteElementString("DATE",DateTime.Now.ToLongTimeString());
			// Close file
			data.Close();
		}

		/// <summary>
		/// Are user settings already loaded ?
		/// </summary>
		/// <param name="session">Current session</param>
		/// <returns>answer</returns>
		public bool AreUserSettingsLoaded(HttpSessionState session)
		{
			return (session["username"] != null);
		}

		/// <summary>
		/// Returns user name
		/// </summary>
		/// <param name="session"></param>
		/// <returns>user name</returns>
		public string GetUserName(HttpSessionState session)
		{
			return (string)session["username"];
		}

		/// <summary>
		/// Returns array of string containing selected page names
		/// </summary>
		/// <param name="session">User session</param>
		/// <returns>String array</returns>
		public string[] GetUserSelectedPages(HttpSessionState session)
		{
			return GetUserSelectedPagesData(session).Split('\\');
		}

		/// <summary>
		/// Adds a page to user's selection list
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="pagename">Page name</param>
		public void AddUserSelectedPage(HttpSessionState session,string pagename)
		{
			string sessiondata = GetUserSelectedPagesData(session);
			if (sessiondata.Length == 0)
			{
				sessiondata += "\\";
			}
			SetUserSelectedPagesData(session,sessiondata + pagename + "\\");
		}
		
		/// <summary>
		/// Removes a page from the selection list
		/// </summary>
		/// <param name="session">User sessison</param>
		/// <param name="pagename">Page name</param>
		public void RemoveUserSelectedPage(HttpSessionState session,string pagename)
		{
			string sessiondata = GetUserSelectedPagesData(session);
			sessiondata = sessiondata.Replace("\\"+pagename + "\\","\\");
			SetUserSelectedPagesData(session,sessiondata);
		}

		/// <summary>
		/// Indicated if a page is in the selection list
		/// </summary>
		/// <param name="session">User Session</param>
		/// <param name="pagename">Page name</param>
		/// <returns>true if page is in the list, false otherwise</returns>
		public bool IsPageSelectedByUser(HttpSessionState session,string pagename)
		{
			return (GetUserSelectedPagesData(session).IndexOf("\\" + pagename + "\\") != -1);
		}

		/// <summary>
		/// Gets string containing list of select pages names. Information is stored is user session.
		/// </summary>
		/// <param name="session">User session</param>
		/// <returns>list of selected pages</returns>
		private string GetUserSelectedPagesData(HttpSessionState session)
		{
			string sessiondata = (string)session["selectedpages"];
			if (sessiondata == null) sessiondata = "";
			return sessiondata;
		}

		/// <summary>
		/// Sets string containing list of select pages names. Information is stored is user session.
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="data">list of selected pages</param>
		private void SetUserSelectedPagesData(HttpSessionState session,string data)
		{
			session["selectedpages"] = data;
			SaveUserSettings(session);
		}

		/// <summary>
		/// Indicates if the current user has administrator privileges
		/// </summary>
		/// <param name="session">User session</param>
		/// <returns>true if user is administrator, false otherelse</returns>
		public static bool IsUserAdministrator(HttpSessionState session)
		{
			if (session["admin"] != null)
			{
				return (bool)session["admin"];
			}
			else return false;
		}

#endregion
    
		public static string UserPagePrefix = "WikiUser.";
	}
}
