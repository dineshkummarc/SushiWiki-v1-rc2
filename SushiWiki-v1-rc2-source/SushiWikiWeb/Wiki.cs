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
    using System.Diagnostics;
    using System.Text;
    using Wiki.Tools;
    using Wiki.Render;

    /// <summary>
    ///    Summary description for Wiki.
    /// </summary>
	[WikiPageHelp("Wiki")]
	[WikiPageSecurity(true,false)]
	public class wfWiki : WikiPage
    {
        protected System.Web.UI.WebControls.HyperLink hlRefresh;
        protected System.Web.UI.WebControls.Label lblBy;
        protected System.Web.UI.WebControls.Label lblOnDate;
        protected System.Web.UI.WebControls.HyperLink hlEditPage;
        protected System.Web.UI.WebControls.Label lblHisto;
        protected System.Web.UI.WebControls.Label lblOwner;
        protected System.Web.UI.WebControls.HyperLink hlAttach;
        protected System.Web.UI.WebControls.LinkButton lSelect;
        protected System.Web.UI.WebControls.Label lblPage;
        protected System.Web.UI.WebControls.HyperLink hlSearch;
        protected System.Web.UI.WebControls.LinkButton lPublic;
        protected System.Web.UI.WebControls.LinkButton lbFullscreen;
        protected System.Web.UI.WebControls.LinkButton lbEndFullString;
        protected System.Web.UI.WebControls.Label lblHistoForFullScreen;
        protected System.Web.UI.WebControls.Label lblTitle;
        protected System.Web.UI.WebControls.Label lblAttachements;
        protected System.Web.UI.WebControls.HyperLink hlLastChanges;
        protected System.Web.UI.WebControls.Label lblPageContent;
        
        protected string pageTitle;
		protected System.Web.UI.WebControls.Label lblEvents;
		protected System.Web.UI.WebControls.HyperLink hlEvents;
		protected System.Web.UI.WebControls.Label lblTitleInfo;
        protected WikiManager.PageData currentPage;

        public wfWiki()
        {
            Page.Init += new System.EventHandler(Page_Init);
        }
       
        #region GetUrlFor...
        public static string GetUrlForOpenPage(string page)
        {
            return "Wiki.aspx?page=" + page;
        }
        public static string GetUrlForOpenHomePage()
        {
            return "Wiki.aspx?page=WikiHome";
        }
        public static string GetUrlForLastChanges(string page)
        {
            return "Wiki.aspx?option=lastchanges&page=" + page;
        }

        #endregion

        

        protected void Page_Load(object sender, EventArgs e)
        {
            // Disable page caching 
            Response.Cache.SetCacheability(HttpCacheability.NoCache) ;
            if (!IsPostBack) // First time call
            {   
				// What is the current page ? (By default it will be WikiHome)
				pageTitle = Request.QueryString["page"] ;
                if (pageTitle == null)
                { pageTitle = "WikiHome" ; } // Default to the entrance
            }
            // Is there any options to apply ?
            bool displayLastChanges = (Request.QueryString["option"] == "lastchanges");
            // Add page to history manager		
            WikiGui.AddPageToHistory(pageTitle,Session);
            // Does the page already exist ?
            WikiManager manager = WikiManager.Singleton() ;
            bool pageFound = false;
            try
            { 
                pageFound = manager.GetPageShortInfo(pageTitle).pageFound; 
            }
            catch (UnauthorizedAccessException)
            {	// Redirect to error page
                Response.Redirect(wfWikiError.GerUrlForUnauthorizedAccess(),true);
            }
            catch (Exception ex)
            {   // Redirect to error page
                Response.Redirect(wfWikiError.GetUrlForStorageError(ex),true);
            }
            if (pageFound)
            { 
				// Page exists -> then load data
                currentPage = manager.GetWikiPage(pageTitle) ;
                // Log visit if required
                if (WikiSettings.Singleton().isLogPageVisitsEnabled) 
                { WikiManager.Singleton().LogVisit(pageTitle,Request.ServerVariables["REMOTE_ADDR"],User.Identity.Name); }
                // Setup buttons & links
                hlEditPage.NavigateUrl = wfWikiEdit.GetUrlForEditPage(currentPage.title) ;
                hlSearch.NavigateUrl = wfWikiSearch.GetUrlForSearch(currentPage.title) ;
                hlRefresh.NavigateUrl = wfWiki.GetUrlForOpenPage(currentPage.title) ;
                hlAttach.NavigateUrl = wfWikiAttach.GetUrlForViewAttachements(currentPage.title) ;
                hlLastChanges.NavigateUrl = wfWiki.GetUrlForLastChanges(currentPage.title);
				hlEvents.NavigateUrl = wfWikiEvents.GetUrlForLoad(currentPage.title);
                // Display last changes option enabled ?
                if (displayLastChanges)
                {
                    DataTable histo = WikiManager.Singleton().GetWikiPageHistory(currentPage.title).Tables[0];
                    if (histo.Rows.Count > 0)
                    {
                        string id = Convert.ToString(histo.Rows[histo.Rows.Count-1]["id"]);
                        WikiManager.PageData pd = WikiManager.Singleton().GetWikiPageById(id);
                        string res =DiffRenderer.Render(pd.pageData,currentPage.pageData);
                        if (res != null) currentPage.pageData = res;
                    }
                }
				// Process page depending on type
				BaseRenderer renderer = WikiRender.GetRenderer(currentPage.type,currentPage.pageData,currentPage.title); 
				lblPageContent.Text = renderer.Format(false); 
				// Refresh some buttons status
                CheckButtonsStatus(currentPage);
                // Add help popups to widgets
                WikiGui.AddPopupToWebControl(hlEditPage,WikiGui.GetHtmlString("Guide.Wiki.Edit"));
				WikiGui.AddPopupToWebControl(hlLastChanges,WikiGui.GetHtmlString("Guide.Wiki.LastChanges"));
                WikiGui.AddPopupToWebControl(hlSearch,WikiGui.GetHtmlString("Guide.Wiki.Search"));
                WikiGui.AddPopupToWebControl(hlAttach,WikiGui.GetHtmlString("Guide.Wiki.Attach"));
                WikiGui.AddPopupToWebControl(hlRefresh,WikiGui.GetHtmlString("Guide.Wiki.Refresh"));
                WikiGui.AddPopupToWebControl(lSelect,WikiGui.GetString("Guide.Wiki.Select"));
                WikiGui.AddPopupToWebControl(lPublic,WikiGui.GetHtmlString("Guide.Wiki.Public"));
                WikiGui.AddPopupToWebControl(lbFullscreen,WikiGui.GetHtmlString("Guide.Wiki.FullScreen"));
				WikiGui.AddPopupToWebControl(hlEvents,WikiGui.GetHtmlString("Guide.Wiki.Events"));
				// Update history information
                lblHisto.Text = WikiGui.GetHisto(Session);
                lblHistoForFullScreen.Text = lblHisto.Text;
                // Display attachements list
                DataTable data = WikiManager.GetAttachementsFor(-1,currentPage.title);
                StringBuilder buff = new StringBuilder();
				int nbrAttachements = data.Rows.Count;
                if ( nbrAttachements > 0)
                {
					buff.Append("<hr>");
                    foreach (DataRow row in data.Rows)
                    {
                        buff.Append("<a ");
                        string type = (string)row["type"];
						if ( (type == "png") || (type == "gif") || (type == "jpg") )
						{
							string url = "WikiThumbnail.aspx?page=" + currentPage.title + "&file=" + row["Name"];
							buff.Append(WikiGui.PopupInfo(WikiGui.GetHtmlString("Gui.Wiki.ViewImage"),"<img src='" + url + "'>"));
						}
                        buff.Append(" href=\"pub\\" + currentPage.title + "\\" + row["Name"] + "\">");
                        buff.Append("<img src=\"images/icon_file_" + type + ".gif\" border='0'>");
                        buff.Append(row["Name"] + "</A>&nbsp;&nbsp;") ;
                    }
                }
                lblAttachements.Text = buff.ToString();
				// Display events list
				DataRow[] rows = WikiCalendar.Singleton().GetEventsForNext30Days(currentPage.title,DateTime.Now);
				int nbrEvents = rows.Length;
				if (nbrEvents > 0)
				{
					StringBuilder events = new StringBuilder("<hr>");
					foreach(DataRow row in rows)
					{
						events.Append("<a href=" + wfWikiEvents.GetUrlForLoad(currentPage.title));
						events.Append(" " + WikiGui.PopupInfo("Event",(string)row["subject"]) + "><image src='images/icon_event.gif' border=0>");
						events.Append(((DateTime)row["date"]).ToShortDateString());
						events.Append("</a>&nbsp;");
					}
					lblEvents.Text = events.ToString();
				}
				// Update title bar
				lblTitle.Text = currentPage.title;
				lblTitleInfo.Text = nbrAttachements.ToString() + "<img src=images/icon_attach.gif border=0>&nbsp;&nbsp;" + nbrEvents.ToString() + "<img src=images/icon_events.gif border=0>";
            }
            else
            {
                // Page does not exist -> Error page
                Response.Redirect (wfWikiError.GetUrlForInvalidPageName(pageTitle),true) ;
            }
            // Resource resolution
            DataBind();
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
			this.lbEndFullString.Click += new System.EventHandler(this.lbEndFullString_Click);
			this.lSelect.Click += new System.EventHandler(this.lSelect_Click);
			this.lPublic.Click += new System.EventHandler(this.lPublic_Click);
			this.lbFullscreen.Click += new System.EventHandler(this.lbFullscreen_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

        private void lSelect_Click(object sender, System.EventArgs e)
        {
            if (WikiUserSettings.Singleton().IsPageSelectedByUser(Session,pageTitle))
            {
                WikiUserSettings.Singleton().RemoveUserSelectedPage(Session,pageTitle);
                SetSelectButtonStatus(false);
            }
            else
            {
                WikiUserSettings.Singleton().AddUserSelectedPage(Session,pageTitle);			
                SetSelectButtonStatus(true);
            }
        }

        private void lPublic_Click(object sender, System.EventArgs e)
        {
            bool status = WikiManager.Singleton().SwitchPublicAccessStatus(pageTitle);
            SetPublicButtonStatus(status);
        }

		/// <summary>
		/// Sets some buttons status depending on page properties
		/// </summary>
		/// <param name="currentPage">Page name</param>
        private void CheckButtonsStatus(WikiManager.PageData currentPage)
        {
            // Selection status
            SetSelectButtonStatus(WikiUserSettings.Singleton().IsPageSelectedByUser(Session,currentPage.title));
            // Private/Public status
            if ( currentPage.ownedBy == User.Identity.Name)
            {
                // Only owner can change this status
                SetPublicButtonStatus(currentPage.publicAccess );
            }
            else
            {
                lPublic.Visible = false;
                // If page private : no attachements link
                if (!currentPage.publicAccess) 
                { hlAttach.Visible = false; }
            }
            // Hide Select button for Guest account
            if (User.Identity.Name == "Guest") 
            { lSelect.Visible = false; }
        }

        private void SetSelectButtonStatus(bool status)
        {
            if (status)
                lSelect.Text = "<img border='0' src='images/icon_selected.gif'>" + WikiGui.GetHtmlString("Gui.Header.UnSelect");
            else
                lSelect.Text = "<img border='0' src='images/icon_notselected.gif'>" + WikiGui.GetHtmlString("Gui.Header.Select");
        }

        private void SetPublicButtonStatus(bool status)
        {
            // Public/Private button
            if (status)
            { lPublic.Text = "<img border='0' src='images/icon_public.gif'>" + WikiGui.GetHtmlString("Gui.Wiki.MakePrivate"); }
            else
            { lPublic.Text = "<img border='0' src='images/icon_private.gif'>" + WikiGui.GetHtmlString("Gui.Wiki.MakePublic"); }
        }

        private void lbFullscreen_Click(object sender, System.EventArgs e)
        {
            Session["fullscreen"] = "ON";
        }

        private void lbEndFullString_Click(object sender, System.EventArgs e)
        {
            Session["fullscreen"] = "OFF";
        }


    
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState (savedState);
            pageTitle=ViewState["pageTitle"].ToString();
        }
    
        protected override object SaveViewState()
        {
            ViewState["pageTitle"]=pageTitle;
            return base.SaveViewState ();
            
        }
    }
	
}
