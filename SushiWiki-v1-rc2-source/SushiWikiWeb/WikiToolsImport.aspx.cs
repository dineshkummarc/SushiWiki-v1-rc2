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

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiToolsImport.
	/// </summary>
	[WikiPageHelp("ToolsImport")]
	[WikiPageSecurity(true,false)]
	public class wfWikiToolsImport : WikiPage
	{
		protected System.Web.UI.WebControls.DataGrid dgPages;
		protected System.Web.UI.WebControls.LinkButton lbImport;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblSelect;
		protected System.Data.DataSet dsPages;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiToolsImport.Title");
			lblSelect.Text = WikiGui.GetString("Gui.WikiToolsImport.Select");
			lbImport.Text = WikiGui.GetString("Gui.WikiToolsImport.Import");
			// Load data
			if (!IsPostBack)
			{
				LoadData();
				dgPages.DataBind();
			}
		}

		private void LoadData()
		{
			string id = Request.QueryString["id"];
			string path = Server.MapPath("cache") + "\\" + id + ".tmp.xml" ;					
			dsPages.ReadXml(path);
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
			this.dsPages = new System.Data.DataSet();
			((System.ComponentModel.ISupportInitialize)(this.dsPages)).BeginInit();
			// 
			// dsPages
			// 
			this.dsPages.DataSetName = "NewDataSet";
			this.dsPages.Locale = new System.Globalization.CultureInfo("fr-FR");
			this.dgPages.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgPages_ItemDataBound);
			this.lbImport.Click += new System.EventHandler(this.lbImport_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dsPages)).EndInit();

		}
		#endregion

		private void lbImport_Click(object sender, System.EventArgs e)
		{
			CheckBox chkSelected;

			LoadData();
			//DataBind();
			int i = 0;
			foreach (DataRow row in dsPages.Tables[0].Rows)
			{ // dsPages.Tables[0].Columns[0].ColumnName
				chkSelected = (CheckBox)dgPages.Items[i].FindControl("chkSelection");
				if (chkSelected.Checked)
				{
					WikiManager.PageData page = new WikiManager.PageData();
					page.title = (string)row["title"];
					page.ownedBy = (string)row["ownedBy"];
					page.updatedBy = (string)row["updatedBy"];
					page.pageData = (string)row["pageData"];
					page.type = (string)row["type"];
					lblInfo.Text += "Importing " + page.title + "<br>";
					WikiManager.Singleton().WriteNewPage(page);
				}
				i++;
			}
			Response.Redirect(wfWikiTools.GetDefaultUrl());
		}

		private void dgPages_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			// Check if page is in database and display result in datagrid
			WikiManager.WikiPageShortInfo info =  WikiManager.Singleton().GetPageShortInfo(e.Item.Cells[1].Text);
			if (info.pageFound)
			{
				e.Item.Cells[2].Text = "<font color=gray>Existing page : Owned by " + info.ownedBy + ", last updated on " + info.lastUpdated + "</font>";
			}
		}
	}
}
