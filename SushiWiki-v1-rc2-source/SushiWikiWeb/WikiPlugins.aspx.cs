using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using Wiki.Plugins;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiPlugins.
	/// </summary>
	[WikiPageSecurity(true,true)]
	public class WikiPlugins : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblInfo;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiPlugins.Title");
			// Retreive plugins information
			StringBuilder buffer = new StringBuilder("<table>");
			buffer.Append("<tr><td>" + WikiManager.Singleton().Plugins.Length.ToString() + " plugins loaded</td></tr>");
			foreach (IWikiPlugin plugin in WikiManager.Singleton().Plugins)
			{
				buffer.Append("<tr><td>");
				buffer.Append("<table border=1 width=100%><tr><td>");
				buffer.Append("<tr><td>Id:</td><td>" + plugin.Id + "</td></tr>");
				buffer.Append("<tr><td>Information:</td><td>" + plugin.Information + "</td></tr>");
				buffer.Append("<tr><td>Path:</td><td>" + plugin.FullPath + "</td></tr>");
				buffer.Append("<tr><td>Supported keyword:</td><td>");
				foreach (KeywordProperties kp in plugin.GetKeywordsProperties())
				{
					buffer.Append(kp.Name + " ");
				}
				buffer.Append("</td></tr></table>");
				buffer.Append("</td></tr>");
			}
			buffer.Append("</table>");
			lblInfo.Text = buffer.ToString();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
