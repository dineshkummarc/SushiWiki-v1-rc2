using System;
using System.IO;
using Wiki.Render;
using Wiki;
using Wiki.Storage.SQL;
using Wiki.Storage.XML;
using Wiki.GUI;
using Wiki.Tools;

namespace Wiki.Macros
{



	public abstract class AttachmentMacro : IMacro
	{
		#region IMacro Members
		protected string fileName;
		protected ParsingContext parsingContext;
		protected string comment;

		/// <summary>
		/// Marshall parameters
		/// </summary>
		/// <param name="pc"></param>
		/// <param name="parameters">
		///First param : attachment name
		///Second param (optional) : comment
		///</param>
		public virtual void Init(Wiki.Render.ParsingContext pc, object[] parameters)
		{

			if (parameters.Length > 2)
				throw new ApplicationException("Too many parameters");
			fileName = parameters[0] as string;
			if (fileName == null)
				throw new ApplicationException("Filename is not a string");
			
			fileName = Path.GetFileName(fileName); //make fileName canonical (prevents path traversal)
			
			if (parameters.Length ==2)
				comment = parameters[1] as string;			
			parsingContext = pc;
		}

		public abstract string Render();

		#endregion

	}


	[WikiMacro("APPLICATION_NAME")]
	public class AppNameMacro : IMacro
	{
		#region IMacro Members

		public void Init(ParsingContext pc, object[] parameters)
		{
		}

		public string Render()
		{
			return WikiManager.applicationName;
		}
		#endregion
	}

	[WikiMacro("WASABI")]
	public class WasabiMacro : IMacro
	{
		#region IMacro Members

		public void Init(ParsingContext pc, object[] parameters)
		{
		}

		public string Render()
		{
			return WikiSettings.Singleton().LocalPath + "/install/Wasabi.exe";
		}
		#endregion
	}

	[WikiMacro("ATTACHEDFILE")]
	[WikiMacro("FILE")]
	public class MacroAttachedFile : AttachmentMacro
	{
		public override string Render()
		{
			if (comment == null)
				comment = fileName;
			return String.Format("<a href='pub/{0}/{1}'>{2}</a>", parsingContext.pageName,fileName,comment);
		}
	}

	[WikiMacro("IMAGE")]
	[WikiMacro("ATTACHEDIMAGE")]
	public class MacroImage : AttachmentMacro
	{
		public override string Render()
		{
			return String.Format("<img src='pub/{0}/{1}'>",parsingContext.pageName,fileName);
		}
	}


	[WikiMacro("ATTACHURLPATH")]
	public class AttUrlPathMacro : IMacro
	{
		#region IMacro Members
		string pageName;

		public void Init(ParsingContext pc, object[] parameters)
		{
			pageName = pc.pageName;
		}

		public string Render()
		{
			return "pub/" + pageName;
		}
		#endregion
	}

	[WikiMacro("VERSIONS")]
	public class VersionsMacro : IMacro
	{
		#region IMacro Members

		public void Init(ParsingContext pc, object[] parameters)
		{
			// TODO:  Add VersionsMacro.Init implementation
		}

		public string Render()
		{
			string version = "<table border=1><tr><td><b>Module</b></td><td><b>Version (release)</b></td></tr>";
			version += "<tar><td>Wiki Manager</td><td>"+WikiManager.v + " (r" + WikiManager.r.ToString() + ")</td>";
			version += "<tr><td>Wiki Parsing</td><td>"+WikiRender.v + " (r" + WikiRender.r.ToString() + ")</td>";
			version += "<tr><td>Wiki Robot</td><td>"+WikiRobot.v + " (r" + WikiRobot.r.ToString() + ")</td>";
			version += "<tr><td>Wiki Settings</td><td>"+WikiSettings.v + " (r" + WikiSettings.r.ToString() + ")</td>";
			version += "<tr><td>Wiki GUI</td><td>"+WikiGui.v + " (r" + WikiGui.r.ToString() + ")</td>";
			version += "<tr><td>Wiki Render</td><td>"+WikiRender.v + " (r" + WikiRender.v.ToString() + ")</td>";
			version += "<tr><td>Wiki Render - Wiki formating</td><td>"+WikiRenderWiki.v + " (r" + WikiRenderWiki.v.ToString() + ")</td>";
			version += "<tr><td>Wiki SQL Storage</td><td>"+WikiStorageSql.v + " (r" + WikiStorageSql.r.ToString() + ")</td>";
			version += "<tr><td>Wiki XML Storage</td><td>"+WikiStorageXml.v + " (r" + WikiStorageXml.r.ToString() + ")</td>";
			version += "</table>";

			return version;

		}

		#endregion
	}

	[WikiMacro("RSS")]
	public class RssMacro : IMacro
	{
		#region IMacro Members

		string url;
		public void Init(ParsingContext pc, object[] parameters)
		{
			if (parameters.Length != 1)
				throw new WikiException("Wrong number of parameters");
			url = parameters[0] as string;			
		}

		public string Render()
		{
			string result;		
			try
			{
				result = "<table class=\"rssFeed\">" 
					+ WikiRSS.GetRSSHTMLTableRows(url) 
					+ "</table>";
			}
			catch 
			{
				result = "Error loading RSS feed (" + url +")"; 
			}
			return result;

		}

		#endregion

	}

	/// <summary>
	/// Usage : WIKITOP(since(in days),number)
	/// TODO: implement it (on top of storage layer...)
	/// </summary>
	[WikiMacro("WIKITOP")]
	public class TopMacro : IMacro
	{
		#region IMacro Members

		int periodAsked;
		int maxResults;

		public void Init(ParsingContext pc, object[] parameters)
		{
			if (parameters.Length > 2)
				throw new WikiException("Too many parameters for WIKITOP macro");
			if (parameters.Length == 0)
				return;
			periodAsked = Convert.ToInt32(parameters[0]);
			if (parameters.Length == 2)
				maxResults = Convert.ToInt32(parameters[1]);
			
			// TODO:  Add TopMacro.Init implementation
		}

		public string Render()
		{
			return "WIKITOP not implemented";
		}

		#endregion

	}
	[WikiMacro("WIKITOP_WEEK10")]
	public class TopWeekMacro : IMacro
	{
		#region IMacro Members

		public void Init(ParsingContext pc, object[] parameters)
		{
			// TODO:  Add TopWeekMacro.Init implementation
		}

		public string Render()
		{
			string[] results = WikiManager.Singleton().GetTop("WEEK_TOP10");
			string html = "";
			if (results != null) 
			{
				foreach (string s in results)
				{
					WikiManager.WikiPageShortInfo info = WikiManager.Singleton().GetPageShortInfo(s);
					string overlib = WikiGui.ExistingWikiPagePopup(s,info);
					html += "<tr><td><a href=Wiki.aspx?page=" + s + " " +  overlib +">" + s + "</a></td></tr>";
				}
			}
			return "<table class=wikitop>" + html + "</table>";
		}

		#endregion

	}

	[WikiMacro("WIKITOP_NEW10")]
	public class TopNewMacro : IMacro
	{
		#region IMacro Members

		public void Init(ParsingContext pc, object[] parameters)
		{
			// TODO:  Add TopNewMacro.Init implementation
		}

		public string Render()
		{
			string html;
			string[] results = WikiManager.Singleton().GetTop("NEW10");
			html = "";
			if (results != null) 
			{
				foreach (string s in results)
					html += "<tr><td><a href=\"Wiki.aspx?page=" + s + "\">" + s + "</a></td></tr>";
			}
			return "<table class=wikitop>" + html + "</table>";

		}

		#endregion

	}



}
