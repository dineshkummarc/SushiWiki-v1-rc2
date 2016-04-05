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
using System.Xml;
using System.Diagnostics;
using System.Threading;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiTools.
	/// </summary>
	[WikiPageHelp("Tools")]
	[WikiPageSecurity(true,false)]
	public class wfWikiTools : WikiPage
	{
		protected System.Web.UI.WebControls.RadioButton rbExportAll;
		protected System.Web.UI.WebControls.RadioButton rbExportSelected;
		protected System.Web.UI.WebControls.RadioButton rbExportMy;
		protected System.Web.UI.HtmlControls.HtmlInputFile txtFile;
		protected System.Web.UI.WebControls.LinkButton lbImport;
		protected System.Web.UI.WebControls.Label lblAdmin;
		protected System.Web.UI.WebControls.Panel PanelAdmin;
		protected System.Web.UI.WebControls.Label lblAdminTools;
		protected System.Web.UI.WebControls.HyperLink hlViewVisitStats;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblExport;
		protected System.Web.UI.WebControls.Label lblImport;
		protected System.Web.UI.WebControls.Panel pnlImport;
		protected System.Web.UI.WebControls.HyperLink hlViewCache;
		protected System.Web.UI.WebControls.HyperLink hlViewLog;
		protected System.Web.UI.WebControls.HyperLink hlViewPlugins;
		protected System.Web.UI.WebControls.LinkButton lbExport;
	
		public static string GetDefaultUrl()
		{
			return "wikitools.aspx";
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiTools.Title");
			lblExport.Text = WikiGui.GetString("Gui.WikiTools.TitleExport");
			lblImport.Text = WikiGui.GetString("Gui.WikiTools.TitleImport");
			lblAdmin.Text = WikiGui.GetString("Gui.WikiTools.TitleAdmin");
			lbImport.Text = WikiGui.GetString("Gui.WikiTools.Import");
			lbExport.Text = WikiGui.GetString("Gui.WikiTools.Export");
			hlViewVisitStats.Text = WikiGui.GetString("Gui.WikiTools.Stats");
			hlViewCache.Text = WikiGui.GetString("Gui.WikiTools.Cache");
			hlViewLog.Text = WikiGui.GetString("Gui.WikiTools.ViewLog");
			hlViewPlugins.Text = WikiGui.GetString("Gui.WikiTools.ViewPlugins");
			rbExportAll.Text = WikiGui.GetString("Gui.WikiTools.ExportAll");
			rbExportSelected.Text = WikiGui.GetString("Gui.WikiTools.ExportSelected");
			rbExportMy.Text = WikiGui.GetString("Gui.WikiTools.ExportMy");

			//
			string id = Request.QueryString["cacheid"];
			if (id != null)
			{ // Return XML export data
				string name = Server.MapPath("cache") + "/" + id + ".tmp.xml";
				Response.Clear();
				Response.ContentType = "text/xml";
				Response.AddHeader("Content-Disposition","attachment; filename=SushiWikiPages.xml");
				Response.WriteFile(name);
				Response.End();
				File.Delete(name);
			}
			// Check for administrator tools
			if (WikiUserSettings.IsUserAdministrator(Session))
			{
				// Add administrator tools
				PanelAdmin.Visible = true;
				// Add administrator informations
				try 
				{
					Process p = Process.GetCurrentProcess();
					lblAdminTools.Text = WikiGui.GetString("Gui.WikiTools.AdminInfo",WikiSettings.Singleton().Storage,WikiSettings.Culture.DisplayName,p.ProcessName,p.HandleCount,p.Threads.Count,Convert.ToInt32(p.WorkingSet/1000),p.StartTime.ToString(),Convert.ToInt32(p.TotalProcessorTime.TotalSeconds));
				} 
				catch { lblAdminTools.Text += "Process information not available. (security restrictions)";}
			}
			// Hide import for Guest user
			if ( (User.Identity.Name == "Guest") || (User.Identity.Name.Length == 0) || (User.Identity.Name == null) )
			{
				pnlImport.Visible = false;
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
			this.lbExport.Click += new System.EventHandler(this.lbExport_Click);
			this.lbImport.Click += new System.EventHandler(this.lbImport_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void lbExport_Click(object sender, System.EventArgs e)
		{
			DataSet data = null;
			if (rbExportAll.Checked) data = WikiManager.Singleton().ExportAllWikiPages();
			if (rbExportMy.Checked) data = WikiManager.Singleton().ExportMyWikiPages(User.Identity.Name);
			if (rbExportSelected.Checked) data = WikiManager.Singleton().ExportSelectedWikiPages((string)Session["selectedpages"]);
			string id = DateTime.Now.Ticks.ToString();
			data.WriteXml(Server.MapPath("cache") + "\\" + id + ".tmp.xml");
			Response.Write("<script>window.open('wikitools.aspx?cacheid=" + id + "');</script>");
		}

		private void lbImport_Click(object sender, System.EventArgs e)
		{
			if (txtFile.PostedFile != null)
			{
				HttpPostedFile myFile = txtFile.PostedFile;
				int nFileLen = myFile.ContentLength; 
				if( nFileLen > 0 )
				{
					// Save sent file in cache directory
					byte[] myData = new byte[nFileLen];
					myFile.InputStream.Read(myData, 0, nFileLen);
					string id = DateTime.Now.Ticks.ToString();
					string path = Server.MapPath("cache") + "\\" + id + ".tmp.xml" ;
					FileStream newFile = new FileStream(path, FileMode.Create);
					newFile.Write(myData, 0, myData.Length);
					newFile.Close();
					Response.Redirect("WikiToolsImport.aspx?id=" + id,true);
				}

			}
		}


	}
}
