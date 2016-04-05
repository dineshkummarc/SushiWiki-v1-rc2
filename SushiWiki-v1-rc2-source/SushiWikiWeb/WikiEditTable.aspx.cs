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

using Wiki.Render;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiEditTable.
	/// </summary>
	[WikiPageHelp("EditTable")]
	[WikiPageSecurity(true,false)]
	public class WikiEditTable : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblInput;
		protected System.Web.UI.WebControls.Button butSave;
		protected System.Web.UI.WebControls.PlaceHolder phInput;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.Label lblTitle;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				
				string pageName = Request.QueryString["p"];
				int tableId = Convert.ToInt32(Request.QueryString["t"]);
				int tableLine = Convert.ToInt32(Request.QueryString["l"]);
				// Load strings
				lblTitle.Text = WikiGui.GetString((tableLine == -1) ? "Gui.WikiEditTable.TitleAdd" : "Gui.WikiEditTable.TitleEdit");
				butSave.Text = WikiGui.GetString((tableLine == -1) ? "Gui.WikiEditTable.SaveAdd" : "Gui.WikiEditTable.SaveEdit");
				// Load the page
				WikiManager.PageData page = WikiManager.Singleton().GetWikiPage(pageName);
				// Check user wrights on page
				wfWikiEdit.CheckUserWrightsOnPage(page,Session,butSave,lblInfo);
				// Load the header
				WikiRenderWiki.STableEditionRequest request = new WikiRenderWiki.STableEditionRequest();
				request.SetGetLineMode(tableId,0);
				BaseRenderer rend = WikiRender.GetRenderer("WIKI",page.pageData,page.title);
				rend.Items["tableRequest"] = request;
				string header = rend.Format(false);
				string[] labels = header.Split('|');
				string[] values = null;
				if (tableLine == -1)
				{ // add new line to table
				}
				else
				{ // Edit line
					request.SetGetLineMode(tableId,tableLine);
					rend = WikiRender.GetRenderer("WIKI",page.pageData,page.title);
					rend.Items["tableRequest"] = request;
					string line = rend.Format(false);
					values = line.Split('|');
				}
					// Load the requested line
				// Build input field 
				StringBuilder buff = new StringBuilder();
				int n = labels.Length;
				for (int i=1 ; i<n-1 ; i++)
				{
					string val =  (tableLine == -1) ? "" : HttpUtility.HtmlEncode(values[i]);
					buff.AppendFormat("<tr><td style='TEXT-ALIGN: right' width=20%>{0}</td><td><input name=field{1} size=80 type=text value=\"{2}\"></td></tr>",labels[i],i,val);
				}
				buff.AppendFormat("<input type=hidden name=nbrfields value='{0}'>",n-2);
				buff.AppendFormat("<input type=hidden name=pagename value='{0}'>",pageName);
				buff.AppendFormat("<input type=hidden name=timestamp value='{0}'>",page.timeStamp);
				buff.AppendFormat("<input type=hidden name=tableid value='{0}'>",tableId);
				buff.AppendFormat("<input type=hidden name=tableline value='{0}'>",tableLine);
				lblInput.Text = buff.ToString();
			}
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
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void butSave_Click(object sender, System.EventArgs e)
		{
			

			bool redirect = true;
			int nbrfields = Convert.ToInt32(Request.Form["nbrfields"]);
			string pageName = Request.Form["pagename"];
			int tableId = Convert.ToInt32(Request.Form["tableid"]);
			int tableLine = Convert.ToInt32(Request.Form["tableline"]);

			StringBuilder newLineData = new StringBuilder("|");
			for (int i=1 ; i<=nbrfields ; i++)
			{
				string val = Request["field" + i.ToString()];
				if (val.IndexOf('|') != -1) 
				{
					lblInfo.Text = WikiGui.GetString("Gui.WikiEditTable.ForbiddenChar");
					lblInfo.CssClass = "warning";
					return;
				}
				newLineData.AppendFormat("{0}|",val);

			}
			// Now get the updated pageData
			WikiManager.PageData data = WikiManager.Singleton().GetWikiPage(pageName);
			WikiRenderWiki.STableEditionRequest request = new WikiRenderWiki.STableEditionRequest();
			if (tableLine == -1)
			{
				request.SetAddLineMode(tableId,newLineData.ToString());
			}
			else
			{
				request.SetUpdateLineMode(tableId,tableLine,newLineData.ToString());
			}
			BaseRenderer rend = WikiRender.GetRenderer("WIKI",data.pageData,data.title);
			rend.Items["tableRequest"] = request;
			string newData = rend.Format(false);
			// Now update the page
			data.pageData = newData;
			data.timeStamp = Convert.ToInt64(Request.Form["timestamp"]);
			data.updatedBy = (string)Session["username"];
			try
			{
				WikiManager.Singleton().WriteNewPage (data) ;
			}
			catch (WikiException)
			{
				WikiManager.WikiPageShortInfo info =  WikiManager.Singleton().GetPageShortInfo(data.title);
				lblInfo.Text = WikiGui.GetString("Guide.WikiEdit.OptimisticLock",info.updatedBy);
				lblInfo.CssClass = "warning";
				redirect = false;
			}
			if (redirect) Response.Redirect(wfWiki.GetUrlForOpenPage(pageName));
		}
	}
}
