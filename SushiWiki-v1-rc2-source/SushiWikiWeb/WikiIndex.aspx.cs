namespace Wiki.GUI
{
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
	using Wiki.GUI;


	/// <summary>
	///    Summary description for WikiIndex.
	///    
	/// Fixed bugs :
	/// | B00001   | 24/10/2002 | EGE  | Missing quotes in URL link
	/// </summary>
	[WikiPageHelp("Index")]
	[WikiPageSecurity(true,false)]
	public class wfWikiIndex : WikiPage
	{
		protected System.Web.UI.WebControls.HyperLink HyperLink1;
		protected System.Web.UI.WebControls.HyperLink hlUsers;
		protected System.Web.UI.WebControls.HyperLink HyperLink2;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.HyperLink hlPages;
		protected System.Web.UI.WebControls.Label lblIndex;

		public wfWikiIndex()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		private bool filterUsers = false;

		public static string GetUrlForUsers() { return "WikiIndex.aspx?filter=users"; }
		public static string GetUrlForPages() { return "WikiIndex.aspx"; }

		protected void Page_Load(object sender, EventArgs e)
		{
			// Disable page caching
			Response.Cache.SetCacheability (HttpCacheability.NoCache) ;
			// Load strings
			hlUsers.Text = WikiGui.GetString("Gui.WikiIndex.Users");
			hlPages.Text = WikiGui.GetString("Gui.WikiIndex.Pages");
			lblTitle.Text = WikiGui.GetString("Gui.WikiIndex.Title");
			// Setup links
			hlUsers.NavigateUrl = GetUrlForUsers();
			hlPages.NavigateUrl = GetUrlForPages();
			// Parse url parameters
			if (Request["filter"] == "users") filterUsers = true;
			// First call
			if (!IsPostBack)
			{
				LoadData();
			}
		}

		private void LoadData()
		{
			// Evals true first time browser hits the page
			WikiManager manager = WikiManager.Singleton() ;

			// Get the Data
			string[] data = manager.GetWikiPageList() ;
			string letter = "";
			bool closetr = false;
			// Prepare stringbuilder
			StringBuilder html = new StringBuilder("<TABLE>");
			//parse data
			foreach (string title in data)
			{
				string name = null;
				string url = wfWiki.GetUrlForOpenPage(title);
				string newletter = null;
				if ( (filterUsers) && (title.StartsWith(WikiUserSettings.UserPagePrefix) ))
				{
					name = title.Substring(WikiUserSettings.UserPagePrefix.Length);
					newletter = title.Substring(WikiUserSettings.UserPagePrefix.Length,1);
				}
				else if (!(filterUsers) && (!title.StartsWith(WikiUserSettings.UserPagePrefix)))
				{
					name = title;
					newletter = title.Substring(0,1);
				}
				// Add letter if needed
				if (letter != newletter)
				{
					letter = newletter;
					if (closetr) { lblIndex.Text += "</TD></TR>"; }
					html.Append("<TR><TD class=index_letter>" + letter + "</TD><TD></TD></TR><TR><TD></TD><TD>");
					closetr = true;
				}
				// Add page link
				html.AppendFormat("<A href=\"{0}\" >{1}</A> &nbsp; ",url,name);
			}
			html.Append("</TD></TR></TABLE>");
			lblIndex.Text = html.ToString();
			HyperLinkColumn hlc = new HyperLinkColumn () ;
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP+ Windows Form Designer.
			//
			InitializeComponent();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}

	}
}
