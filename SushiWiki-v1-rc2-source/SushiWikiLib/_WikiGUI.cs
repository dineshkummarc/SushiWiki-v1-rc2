using System;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Resources; 
using System.IO;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Configuration;
using Wiki.Tools.Resources;


namespace Wiki.GUI
{
	/// <summary>
	/// GUI stuff. 
	/// - Page history management
	/// - DHTML Popups (based on overLIB 3.50 - Erik Bosrup - http://www.bosrup.com/web/overlib/)
	/// - Generic links
	/// 
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 20/08/2002 | EGE         | Initial version based
	/// | 0.2   | 12/09/2002 | EGE         | Page help
	/// | 0.3   | 04/11/2002 | EGE         | Bug B00004 fixed. Added GetFileIcon(...)
	/// | 0.4   | 09/02/2003 | EGE         | Added string ressources
	/// | 0.5   | 11/06/2003 | YZ		   | New ressource management, javascript improvement
	/// | 0.6   | 12/06/2003 | EGE		   | Bug 755440 fixed (Problem with "You are in" breadcrum)
	/// | 0.7   | 11/08/2003 | EGE		   | Current culture now used for string ressources
	/// | 0.8   | 01/02/2004 | EZ          | Refactorings : using BaseRenderer and derived renderers for Ascii Html and Wiki. Modified regexps.

	/// </code>
	/// </summary>
	public class WikiGui
	{
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.8";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 8; 

		/// <summary>
		/// Pages history management.
		/// Adds a page to page history.
		/// If page is already in history, we rollback to this page.
		/// </summary>
		/// <param name="str">Page name</param>
		/// <param name="session">Session</param>
		static public void AddPageToHistory(string str,HttpSessionState session)
		{
			string page_histo = (string)session["page_histo"];
			if (page_histo != null)
			{
				string[] histo;
				if (str == "WikiHome") page_histo = "";
				if ( page_histo.Length > 0 )
				{ 
					histo = page_histo.Split(new char[] {','});
					int i = 0; int pos = -1;
					// Is current page in history
					foreach (string h in histo)
					{ if (h == str) {pos = i; break;} i++;}
					if (pos == -1)
					{
						// Page is not in the list : add it to history
						if (page_histo.Length > 0) page_histo += ",";
						page_histo += str;
					}
					else
					{
						// Page already here : lets rollback to it
						page_histo = "";
						for (int n=0 ; n<=i ; n++)
						{
							if (page_histo.Length > 0) page_histo += ",";
							page_histo += histo[n];
						}
					}
				} 
				else 
				{
					// First page
					page_histo = str;
				}
				session.Add("page_histo",page_histo);
			}
			else
			{
				session.Add("page_histo",str);
			}
		}
			
		/// <summary>
		/// Pages history management.
		/// Returns HTML code displaying history links.
		/// </summary>
		/// <param name="session">Session</param>
		/// <returns>history HTML code</returns>
		static public string GetHisto(HttpSessionState session)
		{
			string html= "";
			string page_histo = (string)session["page_histo"];
			if (page_histo == null) return String.Empty;
			string[] histo =null;
			if (page_histo.Length >0)
			{ 
				histo = page_histo.Split(new char[] {','});
				foreach (string h in histo)
				{
					html += "&gt;<a href=\"wiki.aspx?page=" + h + "\">" + h + "</a> ";
				}
			}
			return html.Remove(0,4);
		}

		/// <summary>
		/// Return the client side javascript code displaying Popups.
		/// </summary>
		/// <param name="title">Popup title</param>
		/// <param name="info">Popup text</param>
		/// <returns></returns>
		static public string PopupInfo(string title, string info)
		{
			return PopupInfo(title, info,false);
			
		}

        static private string JavaScriptEncode(string jsString)
        {
            return jsString.Replace(@"\",@"\\");
        }
		
		/// <summary>
		/// Return the client side javascript code displaying Popups.
		/// </summary>
		/// <param name="title">Popup title</param>
		/// <param name="info">Popup text</param>
		/// <returns></returns>
		static public string PopupInfo(string title, string info,bool sticky)
		{
			string options = ",TIMEOUT,5000";
			if (sticky) options += ",STICKY";
			return @"onmouseover=""return overlib('" + JavaScriptEncode(info.Replace("\"","'")).Replace("'","\\'") + "',CAPTION,'" + title.Replace("\"","'").Replace("'","\\'") + @"'"+ options + @");"" onmouseout='return nd();'";
		}

		/// <summary>
		/// Return the client side javascript code displaying Popups.
		/// <seealso cref="Wiki.GUI.WikiGUI.PopupInfo"/>
		/// </summary>
		/// <param name="info">Popup text</param>
		/// <returns></returns>
		static public string PopupInfoNoBorder(string info)
		{
			return @"onmouseover=""return overlib('" + JavaScriptEncode(info.Replace("'","\\'")) + @"',FULLHTML,DELAY,1000);"" onmouseout='return nd();'";
		}
		
		static public string ExistingWikiPagePopup(string wname,WikiManager.WikiPageShortInfo info)
		{
		return WikiGui.PopupInfo(WikiGui.GetHtmlString("Gui.Popup.Info.Title",wname),
			            WikiGui.GetString("Gui.Popup.Info.Content", info.updatedBy,
			                info.lastUpdated.ToString(WikiSettings.Singleton().DateFormat, null)));
		}

        static public string NewWikiPagePopup(string wname)
        {
            return "onmouseover=\"return overlib('" 
			+ WikiGui.GetString("Gui.Popup.Info.NewPage",wname)
			+ "');\" onmouseout='return nd();'";
        }

		/// <summary>
		/// HTML link for existing wiki pages for wikinames links.
		/// </summary>
		/// <param name="wname">Wiki name</param>
		/// <param name="label">Link label</param>
		/// <param name="info">Short information on wiki page</param>
		/// <returns></returns>
		static public string ExistingWikiPageLink(string wname,string label, WikiManager.WikiPageShortInfo info)
		{

			string overlib = ExistingWikiPagePopup(wname,info);
			return "<a href='Wiki.aspx?page=" + wname + "' " + overlib +">" + label + "</a>";
		}

		/// <summary>
		/// HTML link for non existing wiki pages for wikinames links.
		/// </summary>
		/// <param name="wname">Wikiname</param>
		/// <param name="label">Link label</param>
		/// <returns></returns>
		static public string NewWikiPageLink(string wname,string label)
		{
			string overlib = NewWikiPagePopup(wname);
			return label + "<a href='WikiEdit.aspx?page="+wname+"' " + overlib + ">?</a>";
		}

		/// <summary>
		/// Adds client side javascript on given WebControl displaying the Popup. 
		/// <seealso cref="Wiki.GUI.WikiGUI.AddPopupToWebControl"/>
		/// </summary>
		/// <param name="wc">Webcontrol</param>
		/// <param name="text">Popup text</param>
		static public void AddPopupToWebControl(IAttributeAccessor wc,string text)
		{
			AddPopupToWebControl(wc,GetString("Guide.Title"),text);
		}


		/// <summary>
		/// Adds client side javascript on given WebControl displaying the Popup.
		/// </summary>
		/// <param name="wc">Webcontrol</param>
		/// <param name="title">Popup title</param>
		/// <param name="text">Popup text</param>
		//TODO: refactor javascript escaping
		static public void AddPopupToWebControl(IAttributeAccessor wc,string title,string text)
		{
			wc.SetAttribute("onmouseover","return overlib('" + text.Replace("'","\\'") + "',CAPTION,'"+ title + "');");
			wc.SetAttribute("onmouseout","return nd();");
		}

		/// <summary>
		/// Return file icon name. 
		/// Based on icons found in /images directory (icon_file_*.gif)
		/// </summary>
		/// <param name="filename">File name</param>
		/// <returns>Icon name</returns>
		static public string GetFileIcon(string filename)
		{
			int i = filename.LastIndexOf(".");
			if (i == -1) return "unknown";
			string ext = filename.Substring(i+1);
			if (WikiSettings.Singleton().IconsExtensions.IndexOf("(." + ext + ")") >= 0) 
				return ext;
			else return "unknown";

		}

		public static bool IsTypePicture(string typename)
		{
			string type = typename.ToUpper();
			return ((type == "PNG") || (type == "GIF") || (type == "JPG") || (type == "JPEG"));
		}

		/// <summary>
		/// Load strings ressource file.
		/// These ressources are in plain text files located in BaseResourceDir (application settings).
		/// </summary>
		static public void LoadStrings()
		{
			ResourceManager rm;
			string filePattern = null;
			string baseDir = null;
			
			try
			{
			    // YZ Get plain text resource manager
			    baseDir = ConfigurationSettings.AppSettings["resourceBaseDir"];
			    if (baseDir == null)
			        baseDir = "~/Resources";
			        
			    filePattern = ConfigurationSettings.AppSettings["resourceFilePattern"];
			    if(filePattern == null)
			        filePattern = "Strings.{0}.txt";
			        
				rm = new PlainTextResourceManager(HttpContext.Current.Server.MapPath(baseDir),
				                                  filePattern);
			}
			catch (Exception e)
			{
				throw new WikiException("String ressource file not found (" + 
				                        String.Format(Path.Combine(baseDir,filePattern),
				                                System.Threading.Thread.CurrentThread.CurrentUICulture) +
				                         ")",e);
			}
			HttpContext.Current.Application["RM_Strings"] = rm;
		}

		

		/// <summary>
		/// Retrieves the given key associated value from strings ressource file
		/// and embed arguments in it with String.Format()
		/// </summary>
		/// <param name="key">String key</param>
		/// <param name="args">String.Format arguments</param>
		/// <returns>String value</returns>
		static public string GetString(string key,params object[] args)
		{
			return String.Format(GetString(key),args);
		}

		/// <summary>
		/// Retrieves the given key associated value from strings ressource file
		/// </summary>
		/// <param name="key">String key</param>
		/// <returns>String value</returns>
		static public string GetString(string key)
		{
			string str = null;
			
			try 
			{
				ResourceManager rm = (ResourceManager)HttpContext.Current.Application["RM_Strings"];
				str = rm.GetString(key,WikiSettings.Culture);
				
			}
			catch (MissingManifestResourceException ex)
			{
				throw new WikiException("Ressource file not found",ex);
			}
			catch
			{}
			if (str == null)
			{
				return "RESSOURCE ERROR! : Key "+ key +" not found in strings ressource file.";
			}
			else
			{
				return str;
			}
		}

		static public string GetHtmlString(string key)
		{
			return HttpUtility.HtmlEncode(GetString(key));
		}

		/// <summary>
		/// Retrieves the given key associated value from strings ressource file
		/// and embed arguments in it with String.Format(), encoded for HTML.
		/// </summary>
		/// <param name="key">String key</param>
		/// <param name="args">String.Format arguments</param>
		/// <returns>String value</returns>
		static public string GetHtmlString(string key,params object[] args)
		{
			return HttpUtility.HtmlEncode(String.Format(GetString(key),args));
		}

		static public void DayRender(DayRenderEventArgs e)
		{
			DataTable events = WikiCalendar.Singleton().GetEventsByMonth(e.Day.Date);
			if (events == null) return;
			DataRow[] rows = events.Select("date='" + e.Day.Date + "'");
			if (rows.Length >0) 
			{
				e.Cell.ToolTip = rows.Length.ToString() + " events";
				e.Cell.BorderWidth = 3;
				e.Cell.BorderColor = Color.Black;
				string image = "<img border=0 src=images/calendar_bar_" + Convert.ToString(Math.Min(rows.Length,8)) + ".gif>";
				e.Cell.Controls.Add(new LiteralControl(image));
			}
		}
	} // class WikiGui

	
}
