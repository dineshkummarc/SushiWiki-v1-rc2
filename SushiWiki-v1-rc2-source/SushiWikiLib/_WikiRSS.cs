using System;
using System.Data;
using System.Xml;
using System.Web.Caching;
using System.Text;

using Wiki.GUI;

namespace Wiki
{
	/// <summary>
	/// WikiRSS retrieves RSS feeds for Wiki page display.
	/// It is used by the %RSS()% wiki keyword.
	///
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 26/03/2003 | EGE         | Initial version from scratch
	/// </code>	public class WikiRSS
	/// 
	/// </summary>
	public class WikiRSS
	{
		/// <summary>
		/// Return RSS feed data retrieved from given url.
		/// The XML feed is simply loaded in a DataTable (using DataSet).
		/// </summary>
		/// <param name="url">RSS feed URL</param>
		/// <returns>RSS feed Data</returns>
		static public DataTable GetRSSData(string url)
		{
			string key = "RSS_" + url;
			// Is feed already in cache ?
			if (WikiManager.GetObjectFromApplicationCache(key) != null)
			{
				return (DataTable)WikiManager.GetObjectFromApplicationCache(key);
			}
			// Get the RSS feed
			XmlTextReader reader = new XmlTextReader(url);
			DataSet data = new DataSet();
			data.ReadXml(reader);
			// Store it in Cache for 6 hours
			DataTable table = data.Tables["item"];
			WikiManager.AddObjectToApplicationCache(key,table);
			return table;
		}

		/// <summary>
		/// Gets HTML rows (TR tags) for given RSS feed.
		/// </summary>
		/// <param name="url">RSS feed URL</param>
		/// <returns>HTML string</returns>
		static public string GetRSSHTMLTableRows(string url)
		{
			DataTable data = GetRSSData(url);
			StringBuilder str = new StringBuilder();
			foreach (DataRow row in data.Rows) 
			{
				str.Append("<tr " + WikiGui.PopupInfo(row["title"].ToString(),row["description"].ToString(),true) +"><td><a href=" + row["link"] + ">" + row["title"]+"</a></td></tr>");
			}
			return str.ToString();
		}
	}
}
