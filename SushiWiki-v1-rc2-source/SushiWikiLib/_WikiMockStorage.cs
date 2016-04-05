using System;
using Wiki.Storage;

namespace Wiki.Storage
{
	/// <summary>
	/// Mock storage class for testing purposes
	/// </summary>
	public class WikiMockStorage : IStorageInterface
	{
		#region IStorageInterface Members

		public void Dispose()
		{
			// TODO:  Add _WikiMockStorage.Dispose implementation
		}

		public void WriteNewPage(Wiki.WikiManager.PageData newPage)
		{
			// TODO:  Add _WikiMockStorage.WriteNewPage implementation
		}

		public Wiki.WikiManager.WikiPageShortInfo GetPageShortInfo(string title)
		{
			// TODO:  Add _WikiMockStorage.GetPageShortInfo implementation
			return new Wiki.WikiManager.WikiPageShortInfo ();
		}

		public Wiki.WikiManager.PageData GetPage(string name)
		{
			// TODO:  Add _WikiMockStorage.GetPage implementation
			return null;
		}

		public Wiki.WikiManager.PageData GetPageById(string id)
		{
			// TODO:  Add _WikiMockStorage.GetPageById implementation
			return null;
		}

		public void DeletePage(string str)
		{
			// TODO:  Add _WikiMockStorage.DeletePage implementation
		}

		public bool SwitchPublicAccessStatus(string pageTitle)
		{
			// TODO:  Add _WikiMockStorage.SwitchPublicAccessStatus implementation
			return false;
		}

		public System.Data.DataSet FullTextSearch(string str)
		{
			// TODO:  Add _WikiMockStorage.FullTextSearch implementation
			return null;
		}

		public string[] GetPageList()
		{
			// TODO:  Add _WikiMockStorage.GetPageList implementation
			return null;
		}

		public string[] GetTop(string top)
		{
			// TODO:  Add _WikiMockStorage.GetTop implementation
			return null;
		}

		public System.Data.DataSet ExportAllWikiPages()
		{
			// TODO:  Add _WikiMockStorage.ExportAllWikiPages implementation
			return null;
		}

		public System.Data.DataSet ExportMyWikiPages(string name)
		{
			// TODO:  Add _WikiMockStorage.ExportMyWikiPages implementation
			return null;
		}

		public System.Data.DataSet ExportSelectedWikiPages(string selection)
		{
			// TODO:  Add _WikiMockStorage.ExportSelectedWikiPages implementation
			return null;
		}

		public System.Data.DataSet GetWikiPageHistory(string page)
		{
			// TODO:  Add _WikiMockStorage.GetWikiPageHistory implementation
			return null;
		}

		public System.Data.DataSet GetPagesToEmailForWeek()
		{
			// TODO:  Add _WikiMockStorage.GetPagesToEmailForWeek implementation
			return null;
		}

		public System.Data.DataSet GetPagesToEmailForDay()
		{
			// TODO:  Add _WikiMockStorage.GetPagesToEmailForDay implementation
			return null;
		}

		public System.Data.DataSet GetPagesToEmailForHour()
		{
			// TODO:  Add _WikiMockStorage.GetPagesToEmailForHour implementation
			return null;
		}

		public System.Data.DataSet GetPagesToEmail(DateTime last)
		{
			// TODO:  Add _WikiMockStorage.GetPagesToEmail implementation
			return null;
		}

		public void Log(string type, string subtype, string text, string data)
		{
			// TODO:  Add _WikiMockStorage.Log implementation
		}

		public void LogVisit(string pagetitle, string ipadress, string username)
		{
			// TODO:  Add _WikiMockStorage.LogVisit implementation
		}

		public void Log(char a,string b,string c,string d) {}

		#endregion
	}
}
