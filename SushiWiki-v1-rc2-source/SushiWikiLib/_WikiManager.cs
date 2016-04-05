using System;
using System.Collections ;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Web.SessionState;
using System.Xml.Serialization;
using Wiki.Storage;
using Wiki.Storage.SQL;
using Wiki.Storage.XML;
using Wiki.Tools;
using System.Xml;
using System.Runtime.Serialization;
using System.Web.Caching;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using Wiki.Macros;

using Wiki.Plugins;

namespace Wiki
{
	/// <summary>
	/// This singleton class is used by all application for accessing to main services.
	/// - Page storage management
	/// - Page Rendering
	/// - Mailing services
	/// - Exportation services
	/// - Loging services
	/// 
	/// <para>
	/// It keeps a reference on a object implementing the StorageInterface interface.
	/// This class depends on used storage system (SQL, XML ...). 
	/// </para>
	/// 
	/// <para>
	/// It uses the WikiSettings singleton in order to get application settings.
	/// </para>
	/// 
	/// History :
	/// <code>
	/// 
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 26/07/2002 | EGE         | Initial version based on Wiki.NET from Alistair J. R. Young
	/// | 0.2   | 07/07/2002 | EGE         | First deployed release
	/// | 0.3   | 13/07/2002 | EGE         | New DataBase Connection management
	/// | 0.4   | 26/08/2002 | EGE         | Added GetPagesToEmailForHour
	/// | 0.5   | 03/09/2002 | EGE         | Added DeletePage, GetTop and TrySQL (for install)
	/// | 0.6   | 26/09/2002 | EGE         | Table updates and stored procedures renamed.
	/// | 0.7   | 10/10/2002 | EGE         | Added log table
	/// | 0.8   | 10/10/2002 | EGE         | New global release
	/// | 0.9   | 04/11/2002 | EGE         | New global release
	/// | 0.10  | 25/11/2002 | EGE         | XML storage - Step 1 : preparation
	/// | 0.11  | 10/01/2003 | EGE         | XML storage - Step 2 : creation
	/// | 0.12  | 06/02/2003 | EGE         | XML storage - Step 3 : finished !
	/// | 0.13  | 16/03/2003 | EGE         | Few refactoring and V1 Beta 2 label.
	/// | 0.14  | 24/03/2003 | EGE         | V1 Beta 3 label.
	/// | 0.15  | 29/04/2003 | EGE         | V1 RC1 label.
	/// | 0.16  | 25/05/2003 | EGE         | V1 label.
	/// | 0.17  | 11/06/2003 | YZ          | RichEdit 
	/// | 0.18  | 16/08/2003 | EGE         | now GetPageShortInfo uses application cache
	/// | 0.19  | 12/01/2004 | EGE         | Moved Exception in _WikiError.cs	
	/// 
	/// /// </code>
	/// 
	/// <seealso cref="StorageInterface"/>
	/// <seealso cref="WikiSettings"/>
	/// </summary>
	public class WikiManager : IDisposable
	{
		public static readonly string ShortInfoCachePrefix = "_ShortInfo_.";

		#region Version management
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "1.0 RC2";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 19;

		#endregion
		
		#region Singleton stuff
		private static WikiManager singleton = null;
		private static bool initCalled = false;
		private Hashtable _macroMap;

		/// <summary>
		/// Get the singleton instance
		/// </summary>
		/// <returns>singleton instance</returns>
		public static WikiManager Singleton()
		{
			if (singleton == null) 
			{
				singleton = new WikiManager();
			}
			else
			{
				if (!initCalled)
				{
					throw new WikiException("Manager has not been initialised (Init method not called after first call)");
				}
			}
			return singleton;
		}

#endregion

		/// <summary>
		/// Dispose resources
		/// </summary>
		public void Dispose()
		{
			storageManager.Dispose();
			log.Dispose();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		private WikiManager()
		{
			// Create storage layer depending on current settings
			switch (WikiSettings.Singleton().Storage)
			{
				case "SQL" :
					storageManager = new WikiStorageSql();
					break;
				case "XML" : 
					storageManager = new WikiStorageXml();
					break;
				case "NULL":
					storageManager = new WikiMockStorage();
					break;
				default :
					throw new WikiException("\"" + WikiSettings.Singleton().Storage + "\" storage is not supported.");
			}	
			// Init log
			InitLog();
			InitMacroMap();
		}
		
		/// <summary>
		///	Loads macros defined 
		///	TODO:allow additional macros specified in web.config
		/// </summary>
		private void InitMacroMap()
		{
			_macroMap = new Hashtable();
			Assembly asm = Assembly.GetExecutingAssembly();
			Type[] types = asm.GetExportedTypes();
			foreach(Type type in types)
			{
				if (type.GetInterface("Wiki.Macros.IMacro") != null
					&& ! type.IsInterface
					&& ! type.IsAbstract)
				{
					object[] attr = type.GetCustomAttributes(typeof(WikiMacroAttribute),false);
					if (attr.Length > 0)
					{
						foreach(WikiMacroAttribute at in attr)
							_macroMap[at.Name.ToUpper()] = Activator.CreateInstance(type,false);
					}
					else
					{
						_macroMap[type.Name.ToUpper()] = Activator.CreateInstance(type,false);
					}
				}
				
			}


		}

		public void Init(Cache cache)
		{
			if (initCalled)
			{
				throw new WikiException("WikiManager already initialised (second Init method call)");
			}
			// Remember web application cache
			p_applicationCache = cache;
			// Init called
			initCalled = true;
		}

		private void PluginLog(string pInfo,IWikiPlugin pPlugin)
		{
			Log('i',"PLUGG",pPlugin.Id,pInfo);
		}

		#region Plugins

		public KeywordProperties[] KeywordsProperties { get { return _pluginsManager.GetKeywordsProperties(); } }
		
		private IWikiPlugin[] _plugins;

		private WikiPluginsManager _pluginsManager;

		public IWikiPlugin[] Plugins { get { return _plugins;} }

		public void InitPlugins()
		{
			// Init plugins
			try
			{
				_pluginsManager = new WikiPluginsManager(WikiSettings.Singleton().LocalPath + "/plugins",WikiSettings.Culture,new LogDelegate(PluginLog));
				int n = _pluginsManager.Count;
				Log('i',"PLUGS","LOADING",n.ToString() + " plugins found.");
				for (int i=0 ; i<n ; i++)
				{
					IWikiPlugin plugin = _pluginsManager.GetPlugin(i);
					Log('i',"PLUGS","FOUND","Plugin " + (i+1).ToString() + "/" + n + " : " + plugin.Information);
				}
				_plugins = _pluginsManager.GetPlugins();
			}
			catch (Exception e)
			{
				Log('e',"PLUGS","STARTUP FAIL","Enable to start PluginsManager : " + e.Message);
			}
		}

		#endregion Plugins

		#region Caching

		private void AddPageShortInfoToApplicationCache(WikiPageShortInfo sinfo)
		{
			p_applicationCache.Add(applicationCachePrefix + ShortInfoCachePrefix + sinfo.pageTitle,sinfo,null,DateTime.MaxValue,TimeSpan.Zero,CacheItemPriority.AboveNormal,null);
		}

		private void RemoveShortInfoFromApplicationCache(string name)
		{
			p_applicationCache.Remove(applicationCachePrefix + ShortInfoCachePrefix + name);
		}

		private IStorageInterface storageManager = null;

		static public void AddObjectToApplicationCache(string name,object obj)
		{
			p_applicationCache.Add(applicationCachePrefix + name,obj,null,DateTime.MaxValue,TimeSpan.Zero,CacheItemPriority.AboveNormal,null);
		}

		static public object GetObjectFromApplicationCache(string name)
		{
			return p_applicationCache.Get(applicationCachePrefix + name);
		}

		static public object RemoveObjectFromApplicationCache(string name)
		{
			return p_applicationCache.Remove(applicationCachePrefix + name);
		}

		#endregion

		/// <summary>
		/// Wiki application name
		/// </summary>
		public static string applicationName = "SushiWiki";

		public Hashtable MacroMap
		{
			get {return _macroMap;}
		}


		///////////////////////////////////////////////////////////////////////////////

#region Page Data Structures

		/// <summary>
		/// Contains all page data
		/// </summary>
		public class PageData
		{
			/// <summary>
			/// Last update tima
			/// </summary>
			public DateTime     lastUpdated ;
			/// <summary>
			/// Page title (wiki page name)
			/// </summary>
			public string       title ;
			/// <summary>
			/// Page type : 'WIKI'=WikiPage, 'ASCII'=ASCII page, 'HTML'=HTML page
			/// </summary>
			public string		type ;
			/// <summary>
			/// Last update done by...
			/// </summary>
			public string       updatedBy ;
			/// <summary>
			/// Page owned by...
			/// </summary>
			public string		ownedBy;
			/// <summary>
			/// Page body
			/// </summary>
			public string       pageData ;
			/// <summary>
			/// Page lock status (not used yet)
			/// </summary>
			public string       lockedBy ;
			/// <summary>
			/// Page public access status
			/// </summary>
			public bool			publicAccess;
			/// <summary>
			/// Used for optimistic lock management
			/// </summary>
			public long timeStamp;
			/// <summary>
			/// Default constructor. PageData is filled with default values.
			/// </summary>
			public PageData()
			{
				// Default values
				this.lastUpdated = DateTime.Now;
				this.lockedBy = "";
				this.type = "WIKI";
				this.publicAccess = true;
				this.timeStamp = -1;
			}

			/// <summary>
			/// Data constructor. PageData is filled with given values.
			/// </summary>
			/// <param name="title">see class attributes</param>
			/// <param name="type">see class attributes</param>
			/// <param name="ownedBy">see class attributes</param>
			/// <param name="pageData">see class attributes</param>
			/// <param name="publicAccess">see class attributes</param>
			public PageData(string title,string type,string ownedBy, string pageData,bool publicAccess)
			{
				this.lastUpdated = DateTime.Now;
				this.lockedBy = "";
				this.title = title;
				this.type = type;
				this.ownedBy = ownedBy;
				this.updatedBy = ownedBy;
				this.pageData = pageData;
				this.publicAccess = publicAccess;
			}
		}


		/// <summary>
		/// Contains page quick information
		/// </summary>
		public struct WikiPageShortInfo
		{
			/// <summary>
			/// true if page is found, false otherwise
			/// </summary>
			public bool pageFound;
			/// <summary>
			/// Page last updated by...
			/// </summary>
			public string updatedBy;
			/// <summary>
			/// Page owned by...
			/// </summary>
			public string ownedBy;
			/// <summary>
			/// Last update time
			/// </summary>
			public DateTime lastUpdated;
			/// <summary>
			/// Public access status
			/// </summary>
			public bool publicAccess;
			/// <summary>
			/// Page title
			/// </summary>
			public string pageTitle;
			/// <summary>
			/// Page timestamp (for optimistic lock)
			/// </summary>
			public long timeStamp;
		}

		#endregion

#region Storage management calls (redirected to used storage manager)

		/// <summary>
		/// Returns current storage manager.
		/// </summary>
		/// <returns>Current storage manager</returns>
		public IStorageInterface GetStorageManager()
		{
			return storageManager;
		}

		/// <summary>
		/// Stores a new page
		/// </summary>
		/// <param name="newPage">Page data</param>
		public void WriteNewPage (PageData newPage)
		{
			// EGE : escaping potential troublesome tags
			// and cross-site scripting in case of rich HTML edition
			if (newPage.type == "HTML")
			{
				string[] keywords = new string[] { "script" , "SCRIPT" , "javascript" , "JAVASCRIPT", "jscript" , "JSCRIPT" };
				foreach (string keyword in keywords)
				{
					newPage.pageData = Regex.Replace(newPage.pageData,@"<" + keyword + "((.|\n)*)</" + keyword +">","");
				}
			}	
			// Save new/updated page
			storageManager.WriteNewPage(newPage);
			// Update cache
			RemoveShortInfoFromApplicationCache(newPage.title);
			AddPageShortInfoToApplicationCache(ExtractShortInfo(newPage));
		}

		/// <summary>
		/// Return wiki page full data
		/// </summary>
		/// <param name="name">Wiki page name</param>
		/// <returns>page data</returns>
		public PageData GetWikiPage(string name)
		{
			return storageManager.GetPage(name);
		}

		/// <summary>
		/// Return wiki page full data using internal ID 
		/// (in order to get all page versions)
		/// </summary>
		/// <param name="id">Page internal id</param>
		/// <returns>page data</returns>
		public PageData GetWikiPageById(string id)
		{
			return storageManager.GetPageById(id);
		}

		/// <summary>
		/// Deletes wiki page
		/// </summary>
		/// <param name="pagename">Wiki page name</param>
		public void DeletePage(string pagename)
		{
			storageManager.DeletePage(pagename);
			// Invalidate page in cache
			RemoveShortInfoFromApplicationCache(pagename);
		}



		/// <summary>
		/// Get quick information on requested page.
		/// The WikiHome page is automaticaly created if it doesn't exist in database.
		/// </summary>
		/// <param name="title">Page name</param>
		/// <returns>Page quick informations</returns>
		public WikiPageShortInfo GetPageShortInfo (string title)
		{
			// Check if data is already in application cache
			object cacheddata = null;
			try
			{
				cacheddata = p_applicationCache.Get(applicationCachePrefix + ShortInfoCachePrefix + title);
			}
			catch
			{}

			if (cacheddata != null)
			{
				return (WikiPageShortInfo)cacheddata;
			}
			else
			{
				WikiPageShortInfo info = storageManager.GetPageShortInfo(title);
				if ( (!info.pageFound) && (title == "WikiHome") )
				{
					// Requesting non existing WikiHome -> Automatic creation
					WikiManager.Singleton().Log('i',"MANGR","INSTALL","Creating WikiHome page");
					string txt = null;
					string fileName = WikiSettings.Singleton().LocalPath + "/install/" + WikiSettings.Culture.TwoLetterISOLanguageName + ".WikiHome.htm";
					try
					{	// Try to find localised ressource
						txt = File.OpenText(WikiSettings.Singleton().LocalPath + "/install/" + Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + ".WikiHome.htm").ReadToEnd();
					}
					catch
					{
						WikiManager.Singleton().Log('w',"MANGR","WIKIHOME","Localized WikiHome page (" + fileName + ") not found. Using backup (WikiHome.htm)");
						using (StreamReader sr = File.OpenText(WikiSettings.Singleton().LocalPath + "/install/WikiHome.htm"))
						{
							txt = sr.ReadToEnd();
						}					
					}
					WikiManager.PageData wpage = new WikiManager.PageData();
					wpage.title = "WikiHome";
					wpage.pageData = txt;
					wpage.lastUpdated = DateTime.Now ;
					wpage.type = "HTML";
					wpage.lockedBy = "";
					wpage.updatedBy = "Wiki";
					wpage.ownedBy = "Wiki";
					wpage.publicAccess = true;
					WriteNewPage(wpage);
					info = ExtractShortInfo(wpage);
				}
				// Add it to application cache
				if (info.pageFound)	AddPageShortInfoToApplicationCache(info);
				return info;
			}
		}

		private WikiPageShortInfo ExtractShortInfo(WikiManager.PageData pagedata)
		{
			WikiPageShortInfo sinfo;
			sinfo.ownedBy = pagedata.ownedBy;
			sinfo.lastUpdated = pagedata.lastUpdated;
			sinfo.pageFound = true;
			sinfo.publicAccess = pagedata.publicAccess;
			sinfo.updatedBy = pagedata.updatedBy;
			sinfo.pageTitle = pagedata.title;
			sinfo.timeStamp = pagedata.timeStamp;
			return sinfo;
		}
		/// <summary>
		/// Switch public access page status
		/// </summary>
		/// <param name="pageTitle">wiki page name</param>
		/// <returns>new status</returns>
		public bool SwitchPublicAccessStatus(string pagename)
		{
			bool status = storageManager.SwitchPublicAccessStatus(pagename);
			// Invalidate page in cache
			RemoveShortInfoFromApplicationCache(pagename);
			return status;
		}

		/// <summary>
		/// Simple FullText search
		/// </summary>
		/// <param name="str">String to search</param>
		/// <returns>DataSet containing search results</returns>
		public DataSet SimpleFullTextSearch(string str)
		{
			return storageManager.FullTextSearch(str);
		}

		/// <summary>
		/// Return all pages names
		/// </summary>
		/// <returns>String array containing all page names (in alphabetic order)</returns>
		public string[] GetWikiPageList ()
		{
			return storageManager.GetPageList();
		}

		/// <summary>
		/// Return TOP lists.
		/// WEEK_TOP10 = 10 last updated las week 
		/// NEW10 = 10 last created pages
		/// </summary>
		/// <param name="top">Top name</param>
		/// <returns>Array of wiki page names</returns>
		public string[] GetTop (string top)
		{
			return storageManager.GetTop(top);
		}

		/// <summary>
		/// Returns all wiki page data (for export)
		/// </summary>
		/// <returns>Wiki pages</returns>
		public DataSet ExportAllWikiPages()
		{
			return storageManager.ExportAllWikiPages();
		}

		/// <summary>
		/// Returns given wiki page data (for export)
		/// </summary>
		/// <param name="name">Wiki page name</param>
		/// <returns>Wiki page</returns>
		public DataSet ExportMyWikiPages(string name)
		{
			return storageManager.ExportMyWikiPages(name);
		}

		/// <summary>
		/// Returns selected wiki page data (for export)
		/// </summary>
		/// <param name="selection">string containing selection</param>
		/// <returns>Wiki pages</returns>
		public DataSet ExportSelectedWikiPages(string selection)
		{
			return storageManager.ExportSelectedWikiPages(selection);
		}

		/// <summary>
		/// Return wiki page change history
		/// </summary>
		/// <param name="page">Wiki page name</param>
		/// <returns>Wiki page history</returns>
		public DataSet GetWikiPageHistory (string page)
		{
			return storageManager.GetWikiPageHistory(page);
		}

		/// <summary>
		/// Returns pages updated during last week
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForWeek()
		{
			return storageManager.GetPagesToEmailForWeek();
		}

		/// <summary>
		/// Returns pages updated during the day
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForDay()
		{
			return storageManager.GetPagesToEmailForDay();
		}

		/// <summary>
		/// Returns pages updated during the last hout
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForHour()
		{
			return storageManager.GetPagesToEmailForHour();
		}
		/// <summary>
		/// Returns pages updated since the given date
		/// </summary>
		/// <param name="last">Date</param>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmail(DateTime last)
		{
			return storageManager.GetPagesToEmail(last);
		}		
		#endregion

#region Log management

		/// <summary>
		/// Logs Wiki page visits
		/// </summary>
		/// <param name="pagetitle">Wiki name</param>
		/// <param name="ipadress">User IP adress</param>
		/// <param name="username">User logon name</param>
		public void LogVisit(string pagetitle,string ipadress,string username)
		{
			storageManager.LogVisit(pagetitle,ipadress,username);
		}

		/// <summary>
		/// Application logging
		/// </summary>
		/// <param name="type">Log message type. "i"=information,"d"=debug,"w"=warning,"e"=error,"c"=critical</param>
		/// <param name="subtype">Log message subtype</param>
		/// <param name="text">Log message text</param>
		/// <param name="data">Log message data</param>
		public void Log(char type,string subtype,string text,string data)
		{
			if (WikiSettings.Singleton().logEnabled)
			{
				storageManager.Log(type,subtype,text,data.Replace('|','-'));
			}
			Debug.WriteLine(text,"WikiLOG:" + type + "." + subtype);
		}



		private WikiLog log = null;

		private void InitLog()
		{
			DateTime now = DateTime.Now;
			string file = WikiSettings.Singleton().LocalPath + "/log/mainlog_" + now.ToString("yyyyMMdd");
			log = new WikiLog(file);
		}

		public WikiLog GetLog()
		{
			return log;
		}

#endregion

#region Attachements
		/// <summary>
		/// Returns the attachement directory path for given page and created it if 
		/// it doesn't exist.
		/// </summary>
		/// <param name="title">Page name</param>
		/// <param name="localpath">Web server root local path</param>
		/// <returns></returns>
		public string GetAttachementDirectory(string title)
		{
			string localpath = WikiSettings.Singleton().LocalPath;
			if (!Directory.Exists(localpath + @"\pub"))
				Directory.CreateDirectory(localpath + @"\pub");
			string path = localpath + @"\pub\" + title;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

		static public DataTable GetAttachementsFor(int itemToRemove,string page)
		{
			DataTable MyTable = new DataTable("files");
			MyTable.Columns.AddRange(new DataColumn[]  {
														   new DataColumn("View"),
														   new DataColumn("Name"),
														   new DataColumn("Label"),
														   new DataColumn("Size",typeof(long)),
														   new DataColumn("Type") }   );
			string path = WikiManager.Singleton().GetAttachementDirectory(page);
			DirectoryInfo d = new DirectoryInfo(path);
			int currentId = 0;
			long total = 0;
			foreach (FileInfo f in d.GetFiles())
			{
				// Exclude meta-files
				if (!f.Name.EndsWith(".rikiwiki.xml"))
				{ 
					// Is deletion requested ?
					if (itemToRemove == currentId)
					{
						File.Delete(f.FullName);
					}
					else
					{
						total += f.Length;
						string comment = "";
						if  (File.Exists(f.FullName + ".rikiwiki.xml"))
						{											  
							// Read meta data
							XmlTextReader meta = new XmlTextReader(f.FullName + ".rikiwiki.xml");
							string sLastNodeName = "";
							while (meta.Read())
							{
								if (meta.NodeType == XmlNodeType.Element)
									sLastNodeName = meta.Name;
								if (meta.NodeType == XmlNodeType.Text)
								{
									switch (sLastNodeName)
									{
										case "COMMENT":
											//comment = meta.Value;
											break;
									}
								}
							}
							meta.Close();
						}
						MyTable.Rows.Add(new object[] { "pub/" + page + "/" + f.Name , f.Name , comment , f.Length, Wiki.GUI.WikiGui.GetFileIcon(f.Name) });
					}
					currentId++;
				}
			}
			return MyTable;
		}
	
#endregion

#region Cache management

		private static Cache p_applicationCache = null;

		public readonly static string applicationCachePrefix = "SushiWikiCache_";

#endregion
	
#region IPHostEntry
		private IPHostEntry ipHostEntry = null;

		public bool IsThisALocalIP(string ipToCheck)
		{
			// This operation takes time, so we store result for further use
			
			if (ipHostEntry == null) 
				try
				{
					ipHostEntry = Dns.GetHostByName(Dns.GetHostName());
				}
				catch //does'nt work on W2K3
				{
					return false;
				}
			bool ipfound = false;
			IPAddress localip = IPAddress.Parse(ipToCheck);
			foreach (IPAddress ip in ipHostEntry.AddressList)
			{
				if (localip == ip) ipfound = true;
			}
			IPAddress ip127 = IPAddress.Parse("127.0.0.1");
			if (ip127.Equals(localip))
			{
				ipfound = true;
			}
			return ipfound;
			
		}
#endregion

	}

}
