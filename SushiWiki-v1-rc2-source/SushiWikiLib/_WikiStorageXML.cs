using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Web;

namespace Wiki.Storage.XML
{
	/// <summary>
	/// XML file storage management layer. 
	/// Uses XML serialisation.
	/// <summary>

	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 10/01/2003 | EGE         | Initial version
	/// | 0.2   | 06/02/2003 | EGE         | First complete version
	/// | 0.3   | 21/02/2003 | EGE         | Added page version management
	/// | 0.4   | 24/05/2003 | YZ          | Refactored file and directory access
	/// | 0.5   | 20/08/2003 | EGE         | Added title to WikiPageShortInfo
	/// | 0.6   | 01/10/2003 | EGE         | GetTop was not correctly working
	/// | 0.7   | 04/12/2003 | EGE         | GetPageList : avoid the ".xml" file
	/// </code>
	/// 
	/// </summary>
	public class WikiStorageXml : IStorageInterface,IDisposable
	{	
		#region Version management
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.6";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 6;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public WikiStorageXml()
		{
		}

		/// <summary>
		/// Dispose resources
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// Get quick information on requested page.
		/// The WikiHome page is automaticaly created if it doesn't exist in database.
		/// </summary>
		/// <param name="title">Page name</param>
		/// <returns>Page quick informations</returns>
		public WikiManager.WikiPageShortInfo GetPageShortInfo (string title)
		{
			WikiManager.WikiPageShortInfo info = new WikiManager.WikiPageShortInfo();
			if (File.Exists(GetFilePath(title)))
			{
				info.pageFound = true;
				WikiManager.PageData data = GetPage(title);
				info.lastUpdated = data.lastUpdated;
				info.ownedBy = data.ownedBy;
				info.publicAccess = data.publicAccess;
				info.updatedBy = data.updatedBy;
				info.pageTitle = title;
				info.timeStamp = data.timeStamp;
			}
			else
			{
				info.pageFound = false;
			}

			return info;
		}

		/// <summary>
		/// Deletes wiki page
		/// </summary>
		/// <param name="str">Wiki page name</param>
		public void DeletePage(string title)
		{
			File.Delete(GetFilePath(title));
		}

		/// <summary>
		/// Returns all wiki page data (for export)
		/// </summary>
		/// <returns>Wiki pages</returns>
		public DataSet ExportAllWikiPages()
		{
			return ExportWikiPages("ALL","");
		}

		/// <summary>
		/// Returns given wiki page data (for export)
		/// </summary>
		/// <param name="name">Wiki page name</param>
		/// <returns>Wiki page</returns>
		public DataSet ExportMyWikiPages(string name)
		{
			return ExportWikiPages("MY",name);
		}

		/// <summary>
		/// Returns selected wiki page data (for export)
		/// </summary>
		/// <param name="selection">string containing selection</param>
		/// <returns>Wiki pages</returns>
		public DataSet ExportSelectedWikiPages(string selection)
		{
			return ExportWikiPages("SELECTED",selection);
		}


		#region Mailing
		/// <summary>
		/// Returns pages updated since the given date
		/// </summary>
		/// <param name="last">Date</param>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmail(DateTime last)
		{
			// Create DataSet
			DataSet data = new DataSet();
			DataTable table = data.Tables.Add("pages");
			table.Columns.Add("title",typeof(string));
			table.Columns.Add("updatedBy",typeof(string));
			// Scan files
			DirectoryInfo di = new DirectoryInfo(Path.Combine(WikiSettings.Singleton().LocalPath , "pub"));
			foreach (FileInfo f in di.GetFiles("*.xml"))
			{
				if (f.LastWriteTime > last)
				{
					string title = f.Name.Substring(0,f.Name.Length - 4);
					WikiManager.WikiPageShortInfo info = GetPageShortInfo(title);
					table.Rows.Add(new string[] { title,info.updatedBy });
				}
			}
			return data;
		}

		/// <summary>
		/// Returns pages updated during the day
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForDay()
		{
			TimeSpan ts = new TimeSpan(24,0,0);
			return GetPagesToEmail(DateTime.Now.Subtract(ts));
		}

		/// <summary>
		/// Returns pages updated during the last hout
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForHour()
		{
			TimeSpan ts = new TimeSpan(1,0,0);
			return GetPagesToEmail(DateTime.Now.Subtract(ts));
		}

		/// <summary>
		/// Returns pages updated during last week
		/// </summary>
		/// <returns>DataSet containing search result</returns>
		public DataSet GetPagesToEmailForWeek()
		{
			TimeSpan ts = new TimeSpan(7,0,0,0);
			return GetPagesToEmail(DateTime.Now.Subtract(ts));
		}
		#endregion

		private DateTime lastGetTop_WEEK10 = DateTime.MinValue;
		private DateTime lastGetTop_NEW10 = DateTime.MinValue;

		private string[] Week10 = null;
		private string[] New10 = null;

		/// <summary>
		/// Return TOP lists.
		/// WEEK_TOP10 = 10 last updated las week 
		/// NEW10 = 10 last created pages
		/// </summary>
		/// <param name="top">Top name</param>
		/// <returns>Array of wiki page names</returns>
		public string[] GetTop (string top)
		{
			// Lets do it only once an hour
			if (
				( (top == "WEEK_TOP10") && (lastGetTop_WEEK10.AddHours(1) < DateTime.Now) )
				||
				( (top == "NEW10") && (lastGetTop_NEW10.AddHours(1) < DateTime.Now) )
				)
			
			{
				if (top == "WEEK_TOP10") lastGetTop_WEEK10 = DateTime.Now;
				if (top == "NEW10") lastGetTop_NEW10 = DateTime.Now;
				// Create DataSet
				DataTable table = new DataTable("pages");
				table.Columns.Add("title",typeof(string));
				table.Columns.Add("date",typeof(DateTime));
				// Scan files
				DirectoryInfo di = new DirectoryInfo(Path.Combine(WikiSettings.Singleton().LocalPath, "pub"));
				foreach (FileInfo f in di.GetFiles("*.xml"))
				{
					DateTime d = DateTime.MinValue;
					string title = f.Name.Substring(0,f.Name.Length - 4);
					if (top == "WEEK_TOP10")
					{
						if (f.LastWriteTime.AddDays(7) > DateTime.Now)
							d = f.LastWriteTime;
					}
					if (top == "NEW10")
					{
						DateTime d2 = DateTime.MaxValue;
						DirectoryInfo di2 = new DirectoryInfo(GetVersionsDir(title));
						foreach (FileInfo f2 in di.GetFiles())
						{
							if (f2.LastWriteTime < d2) d2 = f2.LastWriteTime;
						}
						d = d2;
					}
					if (d != DateTime.MinValue)
						table.Rows.Add(new object[] { title,d});
				}
				DataRow[] rows = table.Select("","date ASC");
				string[] res = new string[rows.Length];
				for (int i = 0 ; i< rows.Length ; i++)
				{
					res[i] = rows[i]["title"].ToString();
				}
				// Keep it for next call
				if (top == "WEEK_TOP10") Week10 = res;
				if (top == "NEW10") New10 = res;
				return res;
			}
			else
			{
				// Return keeped data
				if (top == "WEEK_TOP10") return Week10;
				if (top == "NEW10") return New10;
				return null;
			}
		}

		/// <summary>
		/// Return wiki page full data
		/// </summary>
		/// <param name="title">Wiki page name</param>
		/// <returns>page data</returns>
		public WikiManager.PageData GetPage(string title)
		{
			using (TextReader reader = new StreamReader(GetFilePath(title)))
			{
			    XmlSerializer serializer = new XmlSerializer(typeof(WikiManager.PageData));
			    WikiManager.PageData data = (WikiManager.PageData)serializer.Deserialize(reader);
			    return data;
			 }
		}

		/// <summary>
		/// Return wiki page full data using internal ID 
		/// (in order to get all page versions)
		/// </summary>
		/// <param name="id">Page internal id</param>
		/// <returns>page data</returns>
		public WikiManager.PageData GetPageById(string id)
		{
			string[] s = id.Split(' ');
			string page = s[0];
			string ext = s[1];
			string file = Path.Combine(GetVersionsDir(page) , page + "." + ext);
			using (TextReader reader = new StreamReader(file))
			    {
			    XmlSerializer serializer = new XmlSerializer(typeof(WikiManager.PageData));
			    WikiManager.PageData data = (WikiManager.PageData)serializer.Deserialize(reader);
				return data;
				}
		}

		/// <summary>
		/// Return wiki page change history
		/// </summary>
		/// <param name="page">Wiki page name</param>
		/// <returns>Wiki page history</returns>
		public DataSet GetWikiPageHistory (string page)
		{
			DataSet data = new DataSet();
			DataTable table = data.Tables.Add("PageList");
			table.Columns.Add("date",typeof(DateTime));
			table.Columns.Add("id",typeof(string));
			table.Columns.Add("updatedby",typeof(string));
			
			DirectoryInfo di = new DirectoryInfo(GetVersionsDir(page));
			foreach (FileInfo f in di.GetFiles())
			{
				WikiManager.WikiPageShortInfo info = WikiManager.Singleton().GetPageShortInfo(page);
				table.Rows.Add(new object[] { f.LastWriteTime, page + " " + f.Extension.Substring(1), info.updatedBy});
			}
			table.DefaultView.Sort = "date DESC";
			return data;
		}

		/// <summary>
		/// Return all pages list
		/// </summary>
		/// <returns>string array containing all page names</returns>
		public string[] GetPageList ()
		{	
			string path = Path.Combine(WikiSettings.Singleton().LocalPath, "pub");
			DirectoryInfo di = new DirectoryInfo(path);
			FileInfo[] flist = di.GetFiles("*.xml");
			Array a = Array.CreateInstance(typeof(string),(File.Exists(path + "\\.xml")) ? flist.Length-1 : flist.Length);
			int i =0;
			foreach (FileInfo f in flist)
			{
				string name = f.Name;
				if (f.Name.Length>4) // Avoid the ".xml" file.
				{
					a.SetValue(f.Name.Substring(0,f.Name.Length-4),i);
					i++;				
				}
				
			}
			Array.Sort(a);
			return (string[])a;
		}


		/// <summary>
		/// Application logging
		/// </summary>
		/// <param name="type">Log message type</param>
		/// <param name="subtype">Log message subtype</param>
		/// <param name="text">Log message text</param>
		/// <param name="data">Log message data</param>
		public void Log(char type,string subtype,string text,string data)
		{
			WikiManager.Singleton().GetLog().LogLine(DateTime.Now.ToString() + "|" + Convert.ToString(type) + "|" + subtype + "|" + text + "|" + data); 
		}

		/// <summary>
		/// Logs Wiki page visits
		/// </summary>
		/// <param name="pagetitle">Wiki name</param>
		/// <param name="ipadress">User IP adress</param>
		/// <param name="username">User logon name</param>
		public void LogVisit(string pagetitle,string ipadress,string username)
		{
		}

		/// <summary>
		/// Simple FullText search
		/// </summary>
		/// <param name="str">String to search</param>
		/// <returns>DataSet containing search results</returns>
		public DataSet FullTextSearch(string str)
		{
			DataSet data = new DataSet();
			DataTable table = new DataTable("Results");
			table.Columns.Add("title",typeof(string));
			table.Columns.Add("found",typeof(string));
			data.Tables.Add(table);
			string[] names = GetPageList();
			foreach (string name in names)
			{
				WikiManager.PageData page = GetPage(name);
				int i = page.pageData.IndexOf(str);
				if ( i >= 0)
				{
					int a = Math.Max(0,i-20);
					int b = Math.Min(i+str.Length+20,page.pageData.Length-1);
					string nstr = page.pageData.Substring(a,b-a);
					nstr = HttpUtility.HtmlEncode(nstr);
					nstr = nstr.Replace(str,"<span style='BACKGROUND-COLOR:#ffff6f'><b>"+str+"</b></span>");
					table.Rows.Add(new string[] { name , nstr });
				}
			}
			return data;
		}

		/// <summary>
		/// Switch public access page status
		/// </summary>
		/// <param name="pageTitle">wiki page name</param>
		/// <returns>new status</returns>
		public bool SwitchPublicAccessStatus(string pageTitle)
		{
			WikiManager.PageData data = GetPage(pageTitle);
			data.publicAccess = ! data.publicAccess;
			WriteNewPage(data);
			return data.publicAccess;
		}

		/// <summary>
		/// Stores a new page
		/// </summary>
		/// <param name="newPage">Page data</param>
		public void WriteNewPage (WikiManager.PageData newPage)
		{
			if (WikiSettings.Singleton().isOptimisticLockEnabled)
			{
				WikiManager.WikiPageShortInfo info = WikiManager.Singleton().GetPageShortInfo(newPage.title);
				if ( (newPage.timeStamp != -1) && (newPage.timeStamp != info.timeStamp) )
				{
					throw new WikiLockException();
				}
				newPage.timeStamp++;
			}
			string dir = GetVersionsDir(newPage.title);
			lock(this)
			{
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
				// Backup current version
				string file = GetFilePath(newPage.title);
				if (File.Exists(file))
				{
					File.Move(file,
					    Path.Combine(dir ,newPage.title + "." + DateTime.Now.Ticks));
				}
				if (newPage.ownedBy.Length == 0) newPage.ownedBy = newPage.updatedBy;
				using (StreamWriter writer = File.CreateText(file))
				{
				    XmlSerializer serializer = new XmlSerializer(typeof(WikiManager.PageData));
				    serializer.Serialize(writer,newPage); 
				 }
			}
		}

//==============================================================================
// PRIVATE ZONE
//==============================================================================

		/// <summary>
		/// Returns physical file path for given wiki page name
		/// </summary>
		/// <param name="title">Page name</param>
		/// <returns>Physical path</returns>
		private string GetFilePath(string title)
		{
			return Path.Combine(WikiSettings.Singleton().LocalPath,  
			                                "pub" + Path.DirectorySeparatorChar + title + ".xml");
		}

		/// <summary>
		/// Returns directory path containing page versions
		/// </summary>
		/// <param name="title">Page name</param>
		/// <returns>Directory path</returns>
		private string GetVersionsDir(string title)
		{
			string dir = Path.Combine(WikiSettings.Singleton().LocalPath , 
				"pub" + Path.DirectorySeparatorChar + title + ".versions");
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			return dir;
		}

		/// <summary>
		/// Export Wiki pages into XML
		/// </summary>
		/// <param name="type">"ALL", "MY" or "SELECTED"</param>
		/// <param name="criteria">Criteria depending on exportation type</param>
		/// <returns>DataSet containing result</returns>
		private DataSet ExportWikiPages(string type, string criteria)
		{
			DataSet data = new DataSet();
			DataTable table = new DataTable("Results");
			table.Columns.Add("title",typeof(string));
			table.Columns.Add("ownedBy",typeof(string));
			table.Columns.Add("updatedBy",typeof(string));
			table.Columns.Add("pageData",typeof(string));
			table.Columns.Add("type",typeof(string));
			data.Tables.Add(table);
			string[] names = GetPageList();
			foreach (string name in names)
			{
				WikiManager.PageData page = GetPage(name);
				if ( (type == "ALL") 
					|| ( (type == "MY") && (page.ownedBy == criteria) )
					|| ( (type == "SELECTED") && (criteria.IndexOf("\\" + name + "\\") >= 0) ) )
					table.Rows.Add(new string[] { page.title,page.ownedBy,page.updatedBy,page.pageData,page.type});
			}
			return data;			
		}
	}
}
