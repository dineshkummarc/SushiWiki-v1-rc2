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

using Wiki.Render;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiPreview.
	/// </summary>
	[WikiPageSecurity(false,false)]
	public class wfWikiPreview : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblPageContent;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// check whether the page exists
			WikiManager manager = WikiManager.Singleton() ;

			string id = Request.QueryString["id"];
			string page = Request.QueryString["page"];
			WikiManager.PageData currentPage = new WikiManager.PageData();
			if (id != null)
			{ currentPage = manager.GetWikiPageById (id) ;}
			else if (page != null)
			{ currentPage = manager.GetWikiPage(page) ;}


			if (currentPage.title != null)
			{
				BaseRenderer renderer = WikiRender.GetRenderer(currentPage.type,currentPage.pageData,currentPage.title);
				lblPageContent.Text = renderer.Format(true);
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
