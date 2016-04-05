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
	using System.Globalization;
	using System.Threading;

	/// <summary>
	/// This class provides application settings.
	/// They are loaded from WEB.CONFIG, and are stored in a HashTable
	/// 
	/// 
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 26/07/2002 | EGE         | New class
	/// | 0.2   | 19/07/2002 | EGE         | Fixed style default setting
	/// | 0.3   | 17/09/2002 | EGE         | New settings (thumbnails, log, logpagevisit)
	/// | 0.4   | 23/10/2002 | EGE         | New settings (robotmail,title,admins)
	/// | 0.4a  | 25/10/2002 | EGE         | New settings (visitpreservetime)
	/// | 0.5   | 04/11/2002 | EGE         | Rename IsUserADministrator into IsUserAdministrator, added readOnlyGuest setting
	/// | 0.6   | 09/03/2003 | EGE         | Clean thumbnails and enhance HTML render 
	/// | 0.7   | 07/06/2003 | EGE         | Some methods moved to WikiUserSettings
	/// | 0.8   | 21/12/2003 | EGE         | New settings (optimistic_lock,culture)
	/// </code>
	/// </summary>
    public class WikiSettings
    {
		#region Version management
		/// <summary>
		/// Version manangement : version
		/// </summary>
		public static string v = "0.8";
		/// <summary>
		///  Version manangement : release
		/// </summary>
		public static int r = 8;
		#endregion

		#region Singleton stuff

		/// <summary>
		/// static attribute containing singleton
		/// </summary>
		private static WikiSettings singleton = null;

		/// <summary>
		/// Return singleton
		/// </summary>
		/// <returns>singleton</returns>
		public static WikiSettings Singleton()
		{
			if (singleton == null)
				singleton = new WikiSettings();
			return singleton;
		}
#endregion

		/// <summary>
		/// Default constructor
		/// Warning : Do not log in this constructor !!! -> WikiManager singleton must be created AFTER WikiSettings singleton
		/// </summary>
		private WikiSettings()
        {
			// Store configuration settings in hashtable
			StoreConfig("databaseconnection");
			StoreConfig("datetimeformat");
			StoreConfig("preservetime");
			StoreConfig("smtpserver");
			StoreConfig("weburl");
			StoreConfig("robot_timer");
			StoreConfig("log");
			StoreConfig("logpagevisit");
			StoreConfig("admins");
			StoreConfig("title");
			StoreConfig("robotmail");
			StoreConfig("visitpreservetime");
			StoreConfig("readonlyguest");
			StoreConfig("storage");
			StoreConfig("login");
			StoreConfig("local_is_admin");
			StoreConfig("optimistic_lock");
			StoreConfig("culture");
			StoreConfig("signature");
			// Define application culture
			string language = Convert.ToString(configinfo["culture"]);
			if (language == "") culture = Thread.CurrentThread.CurrentCulture;
			else culture = new CultureInfo(language);
		}

		public void LogStatus()
		{
			LogConfig("databaseconnection");
			LogConfig("datetimeformat");
			LogConfig("preservetime");
			LogConfig("smtpserver");
			LogConfig("weburl");
			LogConfig("robot_timer");
			LogConfig("log");
			LogConfig("logpagevisit");
			LogConfig("admins");
			LogConfig("title");
			LogConfig("robotmail");
			LogConfig("visitpreservetime");
			LogConfig("readonlyguest");
			LogConfig("storage");
			LogConfig("login");
			LogConfig("local_is_admin");
			LogConfig("optimistic_lock");
			LogConfig("culture");
			LogConfig("signature");
			Log('i',"LANGUAGE","Used langage is <" + culture.TwoLetterISOLanguageName + ">");

		}

		public void LogConfig(string pName)
		{
			Log('i',"KEY",pName.ToUpper() + "=" + configinfo[pName]);
		}

		private void Log(char pLevel, string pLabel1, string pLabel2)
		{
			WikiManager.Singleton().Log(pLevel,"SETTG",pLabel1,pLabel2);
		}
		// Settings storage
		private Hashtable configinfo = new Hashtable();
		private string localPath;
		private static CultureInfo culture;

		/// <summary>
		/// Stores setting value found in WEB.CONFIG in our hashtable
		/// </summary>
		/// <param name="name">setting name</param>
		private void StoreConfig(string pName)
		{
			configinfo.Add(pName,ConfigurationSettings.AppSettings.Get(pName));
		}

		/// <summary>
		/// Setup used for NUnit testing purposes
		/// </summary>
		/// <param name="databaseconnection"></param>
		public void NUnitSetup(string pDatabaseconnection)
		{
			configinfo["databaseconnection"] = pDatabaseconnection;
			WikiManager.Singleton().Init(null);
		}
		/// <summary>
		/// Application settings defined at run time and added to our hashtable
		/// </summary>
		/// <param name="localpath">running local path</param>
		public void SetContextSettings(string pLocalPath)
		{
			localPath = pLocalPath;
			// Scan available file icons (used for attachements page)
			DirectoryInfo di = new DirectoryInfo(pLocalPath + "\\images");
			if (di.Exists)
			{
				string exts = "";
				foreach (FileInfo fi in di.GetFiles("icon_file_*.gif"))
				{
					exts += "(." + fi.Name.Substring(10,3) + ")";
				}
				configinfo.Add("iconextensions",exts);
			}
		}

		#region Application settings

		/// <summary>
		/// Local root directory. (set at runtime)
		/// </summary>
		public string LocalPath
		{
			get { return localPath; }
		}


		/// <summary>
		/// Gets database connection string. Used by the SQL storage manager.
		/// </summary>
        public string DbConnectionString
        {
            get { return (string) configinfo["databaseconnection"]; }
        }

		/// <summary>
		/// Gets data format used for date display
		/// </summary>
        public string DateFormat
        {
            get { return (string) configinfo["datetimeformat"] ; }
        }

		/// <summary>
		/// Gets page preserve time. After this delay, old versions are automatically deleted.
		/// </summary>
        public int PreserveTime
        {
            get { return Convert.ToInt32((string) configinfo["preservetime"]) ; }
        }

		/// <summary>
		/// Gets SMTP server to be used for sending mails.
		/// Used by WikiRobot.
		/// <seealso cref="Wiki.Tools.WikiRobot"/>
		/// </summary>
		public string smtpServer
		{
			get { return (string) configinfo["smtpserver"];	}
		}
		
		/// <summary>
		/// Web site URL to be used for extenal access.
		/// Used by WikiRobot for links in sent emails.
		/// </summary>
		public string webURL
		{
			get { return (string) configinfo["weburl"];	}
		}

		/// <summary>
		/// Robot scheduling delay.
		/// </summary>
		public int RobotTimer
		{
			get { return Convert.ToInt32(configinfo["robot_timer"]); }
		}

		/// <summary>
		/// Is debug log enabled ?
		/// </summary>
		public bool logEnabled
		{
			get { return ((string)configinfo["log"] == "on"); }
		}

		/// <summary>
		/// Is page visit log enabled ?
		/// </summary>
		public bool isLogPageVisitsEnabled
		{
			get { return ((string)configinfo["logpagevisit"] == "on"); }
		}

		/// <summary>
		/// Visits preserve time
		/// </summary>
		public int VisitsPreserveTime
		{
			get { return Convert.ToInt32((string) configinfo["visitpreservetime"]) ; }
		}

		/// <summary>
		/// WikiPage title
		/// </summary>
		public string ApplicationTitle
		{
			get { return (string)configinfo["title"]; }

		}

		/// <summary>
		/// Avalable icon extensions (for page attachements)
		/// </summary>
		public string IconsExtensions
		{
			get { return (string)configinfo["iconextensions"]; }
		}

		/// <summary>
		/// Mail adress used by Robot mail
		/// </summary>
		public string RobotMailAdress
		{
			get { return (string)configinfo["robotmail"]; }
		}

		/// <summary>
		/// Is Guest user limited to read-only ?
		/// </summary>
		public bool isReadOnlyGuest
		{
			get { return ((string)configinfo["readonlyguest"] == "on"); }
		}

		/// <summary>
		/// WikiPage title
		/// </summary>
		public string Storage
		{
			get
			{
				string s = (string)configinfo["storage"];
				if ( (s == null) || (s.Length == 0) ) 
					//s = "SQL";
					s = "NULL";
				return s;
			}
		}

		/// <summary>
		/// Authentication mode
		/// </summary>
		public string LoginMode
		{
			get { return (string)configinfo["login"]; }
		}

		/// <summary>
		/// Is local user (local IP) automatically administrator ?
		/// </summary>
		public bool isLocalUserAdmin
		{
			get { return ((string)configinfo["local_is_admin"] == "on"); }
		}

		/// <summary>
		/// Administrators
		/// </summary>
		public string Administrators
		{
			get { return (string)configinfo["admins"]; }
		}

		/// <summary>
		/// Is optimistic lock management (based on timestamp) enabled ?
		/// </summary>
		public bool isOptimisticLockEnabled
		{
			get { return ((string)configinfo["optimistic_lock"] == "on"); }
		}

		#endregion

		static public CultureInfo Culture
		{
			get { return culture; }
		}

		public string Signature
		{
			get { return (string)configinfo["signature"]; }
		}
    }
}
