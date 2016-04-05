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

    /// <summary>
    ///    Summary description for WikiError.
    /// </summary>
    [WikiPageHelp("Error")]
	[WikiPageSecurity(true,false)]
    public class wfWikiError : WikiPage
    {
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblErrMessage;
    
	public wfWikiError()
	{
	    Page.Init += new System.EventHandler(Page_Init);
       }

		#region GetUrlFor...

		static public string GetUrlForInvalidPageName(string name)
		{
			return "WikiError.aspx?code=1&page=" + name;
		}

		static public string GetUrlForMissingPageName()
		{
			return "WikiError.aspx?code=2";
		}

		static public string GetUrlForStorageError(Exception ex)
		{
			return "WikiError.aspx?code=3&errmess=" + HttpUtility.UrlEncode(ex.Message) ;
		}

		static public string GerUrlForPageAccessDenied()
		{
			return "WikiError.aspx?code=5";
		}

		static public string GerUrlForAttachementsDenied()
		{
			return "WikiError.aspx?code=6";
		}

		static public string GerUrlForSQLStorageRequired()
		{
			return "WikiError.aspx?code=7";
		}

		static public string GerUrlForUnauthorizedAccess()
		{
			return "WikiError.aspx?code=8";
		}

		#endregion
       
		protected void Page_Load(object sender, EventArgs e)
        {
			// Load string
			lblTitle.Text = WikiGui.GetString("Gui.WikiError.Title");
			// Extract params
			string errcode = Request.QueryString["code"]; // Wiki Error has been raised
			string errPage = Request.QueryString["ErrorPage"]; // ASP.Net Error has been raised
			string errMessage = Request.QueryString["errmess"]; // Original exception message
			string errPath = Request.QueryString["aspxerrorpath"];

			if (errPage != null)
			{
				lblError.Text = "Oops. It doesn't work !";
				lblErrMessage.Text = "Errors where not expected at this location.";
			}
			else if (errPath != null)
			{
				lblError.Text = WikiGui.GetString("WikiErrorCodes.UNKNOWN");
				lblErrMessage.Text = "(" + errPath + ")";
			}
			else
			{
				WikiErrorCodes code = 0;
				try { code = (WikiErrorCodes)Convert.ToInt32(errcode) ;} 
				catch {}
				string page = Request.QueryString["page"];
				string message = "";

				switch (code)
				{
					case WikiErrorCodes.INVALID_PAGE_NAME : //=1
						message = WikiGui.GetString("WikiErrorCodes.INVALID_PAGE_NAME",page);
						break;

					case WikiErrorCodes.MISSING_PAGE_NAME: //=2
						message = WikiGui.GetString("WikiErrorCodes.MISSING_PAGE_NAME");
						break;

					case WikiErrorCodes.STORAGE_ACCESS_ERROR: //=3
						message = WikiGui.GetString("WikiErrorCodes.STORAGE_ACCESS_ERROR",errMessage);
						break;

					case WikiErrorCodes.ADMIN_ACCESS_DENIED: //=4
						message = WikiGui.GetString("WikiErrorCodes.ADMIN_ACCESS_DENIED");
						break;

					case WikiErrorCodes.PAGE_ACCESS_DENIED: //=5	
						message = WikiGui.GetHtmlString("WikiErrorCodes.PAGE_ACCESS_DENIED");
						break;

					case WikiErrorCodes.ATTACHEMENTS_DENIED: //=6
						message = WikiGui.GetString("WikiErrorCodes.ATTACHEMENTS_DENIED");
						break;

					case WikiErrorCodes.SQL_STORAGE_REQUIRED: //=7
						message = WikiGui.GetString("WikiErrorCodes.SQL_STORAGE_REQUIRED");
						break;

					case WikiErrorCodes.UNAUTHORIZED_ACCESS: //=8
						message = WikiGui.GetString("WikiErrorCodes.UNAUTHORIZED_ACCESS");
						break;

					default:
						message = WikiGui.GetString("WikiErrorCodes.UNKNOWN");
						break;

				}
				lblError.Text = "RikiWiki error " + code.ToString();
				lblErrMessage.Text = message;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
    }
}
