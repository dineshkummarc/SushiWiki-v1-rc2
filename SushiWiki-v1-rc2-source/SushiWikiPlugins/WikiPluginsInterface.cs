using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Wiki.Plugins
{

	public interface IWikiPlugin
	{
		string FullPath { get; }
		void Init(string pFullPath, CultureInfo pCulture,LogDelegate pLogDelegate);
		string Information { get; }
		string Id { get; }
		KeywordProperties[] GetKeywordsProperties();
		string GetKeywordData(KeywordProperties pKeywordProperties,Match pMatch,string pPageData);
		ActionProperties[] GetActionProperties();
		string ReturnPageActionHtml(ActionProperties pActionProperties,string pPageData);
	}

	public struct KeywordProperties
	{
		public KeywordProperties(string pName,string pMatchingRegEx,bool pStoreInCache,long pCacheTimeout,IWikiPlugin pWikiPlugin)
		{
			Name = pName;
			MatchingRegEx = pMatchingRegEx;
			StoreInCache = pStoreInCache;
			CacheTimeout = pCacheTimeout;
			_plugin = pWikiPlugin;
		}

		public string Name;
		public string MatchingRegEx;
		public bool StoreInCache;
		public long CacheTimeout;
		private IWikiPlugin _plugin;

		public string GetKeywordData(Match pMatch, string pPageData)
		{
			return _plugin.GetKeywordData(this,pMatch, pPageData);
		}
	}

	public struct ActionProperties
	{
		public ActionPropertiesType Type;
		public string Name;
		public string Label;
	}

	public enum ActionPropertiesType
	{ GlobalAction,PageAction,PageEditAction}

	public delegate void LogDelegate(string pInfo,IWikiPlugin pWikiPlugin);
}
