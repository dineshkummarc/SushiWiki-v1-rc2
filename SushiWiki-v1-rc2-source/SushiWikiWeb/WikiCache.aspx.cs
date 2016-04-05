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

using System.Web.Caching;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for wfWikiCache.
	/// </summary>
	public class wfWikiCache : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.DataGrid DataGridCache;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiCache.Title");
			if (!IsPostBack)
			{
				// Put user code to initialize the page here
				DataTable data = GetCacheObjects();
				DataGridCache.DataSource = data;
				DataGridCache.DataBind();
				
				if (data.Rows.Count == 0)
				{
					DataGridCache.Visible = false;
					lblInfo.Text = WikiGui.GetString("Gui.WikiCache.CacheEmpty");
					lblInfo.Visible = true;
				}
			}
		}

		#region Custom 
		
		private DataTable GetCacheObjects()
		{
			DataTable dt = new DataTable("AppliactionCacheObjects");
			dt.Columns.Add("icon",typeof(string));
			dt.Columns.Add("name",typeof(string));
			dt.Columns.Add("type",typeof(string));		
			foreach (DictionaryEntry e in Cache)
			{
				string k = e.Key.ToString();
				string prefix = WikiManager.applicationCachePrefix;
				if (k.StartsWith(prefix))
				{
					Object o = e.Value;
					string type = o.GetType().ToString();
					switch (type)
					{
						case "System.String":
							type = "String";
							break;
					}
					dt.Rows.Add(  new string[] { type.ToLower(),k.Substring(prefix.Length),type});
				}
			}
			return dt;
		}

		#endregion
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
			this.DataGridCache.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridCache_DeleteCommand);
			this.DataGridCache.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridCache_DeleteCommand);
			this.DataGridCache.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridCache_DeleteCommand);
			this.DataGridCache.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridCache_DeleteCommand);
			this.DataGridCache.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridCache_DeleteCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void DataGridCache_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string key = e.Item.Cells[0].Text;
			WikiManager.RemoveObjectFromApplicationCache(key);
			// Put user code to initialize the page here
			DataGridCache.DataSource = GetCacheObjects();
			DataGridCache.DataBind();

		}

	}
}
