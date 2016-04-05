using System;
using Wiki;
using System.Data;

namespace Wiki.Storage
{
	/// <summary>
	/// Exception raised when page is locked
	/// </summary>
	public class WikiLockException : WikiException
	{
	}

	/// <summary>
	/// Summary description for WikiStorage.
	/// </summary>
	public interface IStorageInterface
	{
		void Dispose();
		void WriteNewPage (WikiManager.PageData newPage);
		WikiManager.WikiPageShortInfo GetPageShortInfo (string title);
		WikiManager.PageData GetPage(string name);
		WikiManager.PageData GetPageById(string id);
		void DeletePage(string str);
		bool SwitchPublicAccessStatus(string pageTitle);
		DataSet FullTextSearch(string str);
		string[] GetPageList ();
		string[] GetTop (string top);
		DataSet ExportAllWikiPages();
		DataSet ExportMyWikiPages(string name);
		DataSet ExportSelectedWikiPages(string selection);
		DataSet GetWikiPageHistory (string page);
		DataSet GetPagesToEmailForWeek();
		DataSet GetPagesToEmailForDay();
		DataSet GetPagesToEmailForHour();
		DataSet GetPagesToEmail(DateTime last);
		void Log(char type,string subtype,string text,string data);
		void LogVisit(string pagetitle,string ipadress,string username);
		}
}
