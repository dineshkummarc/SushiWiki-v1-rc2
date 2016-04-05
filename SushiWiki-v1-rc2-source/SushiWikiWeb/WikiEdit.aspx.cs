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
	using System.IO;
	using System.Web.Caching;

    /// <summary>
    ///   24/03/2003 EGE Fixed bug 708848
    ///   28/04/2003 EGE Fixed bug 705296
    ///   28/04/2003 EGE Fixed bug 724223 (in ASPX file)
    ///   25/05/2003 EGE fixed bug 741651 (in ASPX file)
    /// </summary>
	[WikiPageHelp("Edit")]
	[WikiPageSecurity(true,false)]
	public class wfWikiEdit : WikiPage
    {
        protected System.Web.UI.WebControls.Button cmdCancel;
        protected System.Web.UI.WebControls.TextBox txtPageContent;
		protected System.Web.UI.WebControls.DropDownList ddlPageType;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.LinkButton lbSelectVersion;
		protected System.Web.UI.WebControls.Label lblOwner;
		protected System.Web.UI.WebControls.Button cmdDelete;
		protected System.Web.UI.WebControls.Button cmdSave;
		protected System.Web.UI.WebControls.Button cmdSaveAndReturn;
		protected System.Web.UI.WebControls.Panel panelVersions;
		protected System.Web.UI.WebControls.Panel PanelHTMLEditor;
		protected System.Web.UI.HtmlControls.HtmlGenericControl richedit;
		protected System.Web.UI.WebControls.Label lblPageName;
		protected System.Web.UI.WebControls.DropDownList ddlTemplates;
		protected System.Web.UI.WebControls.LinkButton lbCopyTemplate;
		protected System.Web.UI.WebControls.Label lblHtmlEditor;
		protected System.Web.UI.WebControls.Label lblRichTextEditor;
		protected System.Web.UI.WebControls.Label lblPreviousVersions;
		protected System.Web.UI.WebControls.Label lblPreviewVersion;
		protected System.Web.UI.WebControls.Label lblPageTitle;
		protected System.Web.UI.WebControls.Label lblHiddenFields;
		protected System.Web.UI.WebControls.DropDownList ddlVersions;
 
	    public wfWikiEdit()
	    {
	        Page.Init += new System.EventHandler(Page_Init);
        }

		#region GetUrlFor...
		static public string GetUrlForEditPage(string page)
		{
			return "WikiEdit.aspx?page=" + page;
		}
		#endregion

		public string RichTextSource = null; // Used by .aspx code
	
		//====================================================
        protected void Page_Load(object sender, EventArgs e)
        {
			// Disable page caching
            Response.Cache.SetCacheability(HttpCacheability.NoCache) ;
            if (!IsPostBack)
            {// First load
                string pageTitle ;
                pageTitle = Request.QueryString["page"] ;
                if ((pageTitle == null) || (pageTitle.Length == 0)  )
                {
                    Response.Redirect (wfWikiError.GetUrlForMissingPageName(),true) ;
                }
                // check whether the page exists
				if (WikiManager.Singleton().GetPageShortInfo(pageTitle).pageFound)
				{
					// Page exists
					WikiManager.PageData currentPage = WikiManager.Singleton().GetWikiPage (pageTitle) ;
					// Fill hidden fields
					lblHiddenFields.Text = String.Format("<input type=hidden name=pagename value='{0}'><input type=hidden name=timestamp value='{1}'>",currentPage.title,currentPage.timeStamp);
					// Update some labels
					txtPageContent.Text = currentPage.pageData ;
					lblOwner.Text = currentPage.ownedBy;
					lblPageName.Text = pageTitle;
					// Set page type combo selection
					int si = 0;
					switch (currentPage.type)
					{
						case "WIKI" : si=0 ; break;
						case "ASCII" : si=1 ; break;
						case "HTML" : si=2 ; break;
						default : throw new WikiException(("'" + currentPage.type + "' is a unknown page type")); 
					}
					ddlPageType.SelectedIndex = si ;
					// Check user wrights on current page
					CheckUserWrightsOnPage(currentPage,Session,cmdSave,lblInfo);
					cmdSaveAndReturn.Visible = cmdSave.Visible;
					// Only owner or administrator can delete page
					cmdDelete.Visible = ( ((bool)Session["admin"]) || (User.Identity.Name == currentPage.ownedBy) );
					// Load history
					DataTable table = WikiManager.Singleton().GetWikiPageHistory(pageTitle).Tables["PageList"];
					table.Columns.Add("label",typeof(string),WikiGui.GetString("Gui.WikiEdit.VersionsFormatting"));
					ddlVersions.DataSource = table.DefaultView;
					ddlVersions.DataBind();
					if (table.Rows.Count == 0) panelVersions.Visible = false;
				}
				else
				{
					// Fill hidden fields
					lblHiddenFields.Text = String.Format("<input type=hidden name=pagename value='{0}'><input type=hidden name=timestamp value='{1}'>",pageTitle,-1);
					// This is a new page
					cmdDelete.Visible = false;
					panelVersions.Visible = false;
				}
				// Add javascript stuff
				cmdDelete.Attributes.Add("onclick","javascript:return confirm('" + WikiGui.GetString("Guide.WikiEdit.ConfirmDelete") + "')");
				WikiGui.AddPopupToWebControl(cmdSave,WikiGui.GetString("Guide.WikiEdit.Save"));
				WikiGui.AddPopupToWebControl(cmdCancel,WikiGui.GetString("Guide.WikiEdit.Cancel"));
				WikiGui.AddPopupToWebControl(ddlPageType,WikiGui.GetString("Guide.WikiEdit.PageType"));
				WikiGui.AddPopupToWebControl(ddlVersions,WikiGui.GetString("Guide.WikiEdit.Versions",WikiSettings.Singleton().PreserveTime.ToString()));
				WikiGui.AddPopupToWebControl(lbSelectVersion,WikiGui.GetString("Guide.WikiEdit.SelectVersion"));
				// Load strings
				lblPageTitle.Text = WikiGui.GetString("Gui.WikiEdit.Title");
				cmdSaveAndReturn.Text = WikiGui.GetString("Gui.WikiEdit.SaveAndReturn");
				cmdSave.Text = WikiGui.GetString("Gui.WikiEdit.Save");
				cmdCancel.Text = WikiGui.GetString("Gui.WikiEdit.Cancel");
				cmdDelete.Text =  WikiGui.GetString("Gui.WikiEdit.Delete");
				lbCopyTemplate.Text = WikiGui.GetString("Gui.WikiEdit.CopyTemplate");
				lblHtmlEditor.Text = WikiGui.GetString("Gui.WikiEdit.HtmlEditor");
				lblRichTextEditor.Text = WikiGui.GetString("Gui.WikiEdit.RichTextEditor");
				lblPreviousVersions.Text = WikiGui.GetString("Gui.WikiEdit.PreviousVersions");
				lbSelectVersion.Text = WikiGui.GetString("Gui.WikiEdit.SelectVersion");
				lblPreviewVersion.Text = WikiGui.GetString("Gui.WikiEdit.PreviewVersion");
			} // if (!IsPostBack)

			// Load templates for type
			if (ddlTemplates.SelectedValue == "")
			{
				ddlTemplates.Items.Clear();
				// Get ou Update application cache
				ArrayList templates = (ArrayList)WikiManager.GetObjectFromApplicationCache("_Templates_." + ddlPageType.SelectedValue);
				if (templates == null)
				{
					templates = new ArrayList();
					string path = Server.MapPath("templates");
					if (Directory.Exists(path))
					{
						DirectoryInfo d = new DirectoryInfo(path);
						foreach (FileInfo f in d.GetFiles("*." + ddlPageType.SelectedValue))
						{
							templates.Add(f.Name);
						}
						WikiManager.AddObjectToApplicationCache("_Templates_." + ddlPageType.SelectedValue,templates);
					}
				}
				// Fill templates combo
				foreach (string o in templates)
				{
					int lpos = o.LastIndexOf('.');
					string tplname = o.Substring(0,lpos);
					string tpltype = o.Substring(lpos+1);
					ddlTemplates.Items.Add(tplname + " (" + tpltype.ToUpper() + ")" );
				}
			}
			SetupControls();
        }

		/// <summary>
		/// Checks user wrights on page
		/// - Is it a gest user ?
		/// - Is page private ?
		/// </summary>
		/// <param name="currentPage">Page to be checked</param>
		/// <param name="session">Current session</param>
		/// <param name="button">Action button</param>
		/// <param name="label">Label (for displaying errror messages)</param>
		public static void CheckUserWrightsOnPage(WikiManager.PageData currentPage, HttpSessionState session, WebControl button, Label label)
		{
			string user = WikiUserSettings.Singleton().GetUserName(session);
			if ( (!currentPage.publicAccess) && (user != currentPage.ownedBy) && (!(bool)session["admin"]) )
			{
				
				// Page access reserved to owner
				button.Visible = false ;
				label.Text = WikiGui.GetString("Gui.WikiEdit.Error.PrivatePage",currentPage.ownedBy );
				label.CssClass = "warning";
			}
			if ( (user == "Guest") && (WikiSettings.Singleton().isReadOnlyGuest) && (currentPage.title != "WikiTestPage") && (!((bool)session["admin"])))
			{
				// Guest can only update WikiTextPage page
				button.Visible = false ;
				label.Text = WikiGui.GetString("Gui.WikiEdit.Error.GuestUser");
				label.CssClass = "warning";
			}
		}


		/// <summary>
		/// Setup controls regarding page format and display options (richtext,...)
		/// </summary>
		protected void SetupControls()
		{
			// If Internet Explorer, display HTML Editor
			if ( (Request.Browser.Type == "IE6") && (ddlPageType.SelectedIndex == 2) )
			{
				PanelHTMLEditor.Visible = true;
				txtPageContent.Visible = false;
				this.RichTextSource = txtPageContent.Text;
				//this.RegisterStartupScript("InitHiddenTextBoxForRichEdit","<script language=javascript>RichTextOnBlur</script>");
			}
			else
			{
				PanelHTMLEditor.Visible = false;
				txtPageContent.Visible = true;
			}		
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
			this.lbCopyTemplate.Click += new System.EventHandler(this.lbCopyTemplate_Click);
			this.cmdSaveAndReturn.Click += new System.EventHandler(this.cmdSaveAndReturn_Click);
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
			this.lbSelectVersion.Click += new System.EventHandler(this.lbSelectVersion_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		// TODO : optimistic page locking

        public void cmdSave_Click (object sender, System.EventArgs e)
        {
			SavePage(false);
       }

		private void cmdSaveAndReturn_Click(object sender, System.EventArgs e)
		{
			SavePage(true);
		}

		private void SavePage(bool redirect)
		{
			WikiManager.PageData newPage = new WikiManager.PageData() ;
			newPage.lastUpdated = DateTime.Now ;
			newPage.title = Request.Form["pagename"];
			newPage.timeStamp = Convert.ToInt64(Request.Form["timestamp"]);
			newPage.type = ddlPageType.SelectedItem.Value;
			newPage.updatedBy = HttpContext.Current.User.Identity.Name;
			newPage.ownedBy = lblOwner.Text;
			newPage.pageData = txtPageContent.Text ;
			newPage.publicAccess = true;
			if ( (Request.Browser.Type == "IE6") && (ddlPageType.SelectedIndex == 2))
			{ // Rich text HTML editor
				newPage.pageData=Request.Form["richtextsource"];
			}
			newPage.lockedBy = "" ;
			newPage.publicAccess = WikiManager.Singleton().GetPageShortInfo(newPage.title).publicAccess; // TODO : find a nicer way
			try
			{
				WikiManager.Singleton().WriteNewPage (newPage) ;
			}
			catch (WikiException)
			{
				WikiManager.WikiPageShortInfo info =  WikiManager.Singleton().GetPageShortInfo(newPage.title);
				lblInfo.Text = WikiGui.GetString("Guide.WikiEdit.OptimisticLock",info.updatedBy);
				lblInfo.CssClass = "warning";
				redirect = false;
			}
			if (redirect) Response.Redirect (wfWiki.GetUrlForOpenPage(newPage.title)) ;
		}

        public void cmdCancel_Click (object sender, System.EventArgs e)
        {
			string name = Request.Form["pagename"];
			if (WikiManager.Singleton().GetPageShortInfo(name).pageFound)
			{
				Response.Redirect (wfWiki.GetUrlForOpenPage(name)) ;
			}
			else
			{
				Response.Redirect(wfWiki.GetUrlForOpenHomePage());
			}
		}

		private void lbSelectVersion_Click(object sender, System.EventArgs e)
		{
			txtPageContent.Text = WikiManager.Singleton().GetWikiPageById(ddlVersions.SelectedItem.Value).pageData;
		}

		private void cmdDelete_Click(object sender, System.EventArgs e)
		{
			WikiManager.Singleton().DeletePage(Request.Form["pagename"]);
			Response.Redirect (wfWiki.GetUrlForOpenHomePage()) ;
		}

		private void lbCopyTemplate_Click(object sender, System.EventArgs e)
		{
			string selection = ddlTemplates.SelectedValue;
			if (selection.Length == 0) return;
			int lpos = selection.LastIndexOf('(');
			string tname = selection.Substring(0,lpos-1);
			string ttype = selection.Substring(lpos+1,selection.Length-lpos-2);
			// Load template
			StreamReader sr = File.OpenText(Path.Combine(Server.MapPath("templates"),tname + "." + ttype));
			txtPageContent.Text = sr.ReadToEnd();
			ddlPageType.SelectedValue = ttype;
			sr.Close();
			SetupControls();
		}
    }
}
