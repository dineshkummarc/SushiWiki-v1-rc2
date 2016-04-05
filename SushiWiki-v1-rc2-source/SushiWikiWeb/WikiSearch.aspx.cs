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
    ///    29/04/2003 EGE Fixed bug 724243
    /// </summary>
	[WikiPageHelp("Search")]
	[WikiPageSecurity(true,false)]
	public class wfWikiSearch : WikiPage
    {
		protected System.Web.UI.WebControls.DataGrid linksGrid;
		protected System.Web.UI.WebControls.TextBox tbText;
		protected System.Web.UI.WebControls.LinkButton lbSearch;
		protected System.Web.UI.WebControls.Label lblSearch;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblLabel;

        public DataSet results ;
    
    	public wfWikiSearch()
	    {
	        Page.Init += new System.EventHandler(Page_Init);
        }

		private void BindData(string str)
		{
			if (str.Length > 0)
			{
				// Call the full search
				results = WikiManager.Singleton().SimpleFullTextSearch(str) ;
				if (results.Tables[0].Rows.Count > 0)
				{
					linksGrid.DataSource = results.Tables["results"] ;
					linksGrid.DataBind() ;
					linksGrid.Visible = true;
					lblSearch.Text = results.Tables[0].Rows.Count.ToString() +  WikiGui.GetString("Gui.WikiSearch.ResultsFound");
				}
				else
				{
					linksGrid.Visible = false;
					lblSearch.Text = WikiGui.GetString("Gui.WikiSearch.NothingFound");
				}
			}
			else linksGrid.Visible = false;
		}

		#region GetUrlFor...
		static public string GetUrlForSearch(string str)
		{
			return "WikiSearch.aspx?str=" + str;
		}
		#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
			// Disable page caching
            Response.Cache.SetCacheability(HttpCacheability.NoCache) ;
			// Load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiSearch.Title");
			lblLabel.Text = WikiGui.GetString("Gui.WikiSearch.Label");
			lbSearch.Text = WikiGui.GetString("Gui.WikiSearch.StartSearch");
			//
			string str;
			if (!IsPostBack)
			{
				str = Request.QueryString["str"] ;
				if ((str != null) && (str.Length > 0)) tbText.Text = str;
				BindData(tbText.Text);
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
			this.lbSearch.Click += new System.EventHandler(this.lbSearch_Click);
			this.linksGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.linksGrid_PageIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		private void lbSearch_Click(object sender, System.EventArgs e)
		{
			BindData(tbText.Text);
		}

		private void linksGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			linksGrid.CurrentPageIndex = e.NewPageIndex;
			BindData(tbText.Text);
		}

    }
}
