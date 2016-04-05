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

namespace Wiki.GUI
{
	/// <summary>
	/// This singleton class manages the page storage and the attachements.
	/// 
	/// WARNING : Viewstate is required on dgFiles.
	/// 
	/// BUG History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | B0003 | 04/11/2002 | EGE         | Can't delete attachement
	/// 
	/// </code>
	/// </summary>
	[WikiPageSecurity(true,false)]
	public class wfWikiAttach : WikiPage
	{
		protected System.Web.UI.HtmlControls.HtmlInputFile txtFile;
		protected System.Web.UI.HtmlControls.HtmlInputText txtComment;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtPage;
		protected System.Data.DataSet dsFiles;
		protected System.Web.UI.WebControls.DataList dlFiles;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.HyperLink hlPage;
		protected System.Web.UI.WebControls.Label lblAttachements;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblLocalFile;
		protected System.Web.UI.WebControls.Label lblComment;
		protected System.Web.UI.WebControls.Button butSendFile;
		protected System.Web.UI.WebControls.DataGrid dgFiles;
	
		#region GetUrlFor...
		static public string GetUrlForViewAttachements(string str)
		{
			return "WikiAttach.aspx?page=" + str;
		}
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			string pageTitle = Request.QueryString["page"] ;
			Response.Cache.SetCacheability(HttpCacheability.NoCache) ;
			// Load strings
			lblTitle.Text = WikiGui.GetString("Gui.WikiAttach.Title");
			lblLocalFile.Text = WikiGui.GetString("Gui.WikiAttach.LocalFile");
			lblComment.Text = WikiGui.GetString("Gui.WikiAttach.Comment");
			butSendFile.Text = WikiGui.GetString("Gui.WikiAttach.SendFile");
			// check whether the page exists
			WikiManager manager = WikiManager.Singleton() ;
			WikiManager.WikiPageShortInfo currentPage = manager.GetPageShortInfo(pageTitle);
			if (currentPage.pageFound)
			{
				// Check user rights
				if ( (currentPage.ownedBy != User.Identity.Name) && (!currentPage.publicAccess) )
				{
					Response.Redirect(wfWikiError.GerUrlForPageAccessDenied(),true);
				}
				// Check for Guest user
				if ( ( (User.Identity.Name == "Guest") || (User.Identity.Name.Length == 0) || (User.Identity.Name == null) ) && (WikiSettings.Singleton().isReadOnlyGuest) )
				{
					Response.Redirect(wfWikiError.GerUrlForAttachementsDenied(),true);
				}

				// Page Link
				hlPage.Text = pageTitle;
				hlPage.NavigateUrl = wfWiki.GetUrlForOpenPage(pageTitle);
				txtPage.Value = pageTitle;
				// Displays existing attachements
				if (!IsPostBack)
				{
					DataTable table = WikiManager.GetAttachementsFor(-1,Request.QueryString["page"]);
					if (table.Rows.Count > 0)
					{
						dsFiles.Tables.Add(table);				
						dgFiles.DataBind();
						long total = (long)table.Compute("sum(Size)","");
						lblAttachements.Text =  WikiGui.GetString("Gui.WikiAttach.Total",total/1000,table.Rows.Count);
					}

				}
			}
			else
			{
				// Page does not exist      
				Response.Redirect (wfWiki.GetUrlForOpenPage(pageTitle)) ;
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
			this.dsFiles = new System.Data.DataSet();
			((System.ComponentModel.ISupportInitialize)(this.dsFiles)).BeginInit();
			// 
			// dsFiles
			// 
			this.dsFiles.DataSetName = "NewDataSet";
			this.dsFiles.Locale = new System.Globalization.CultureInfo("fr-FR");
			this.butSendFile.Click += new System.EventHandler(this.sendFile_Click);
			this.dgFiles.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFiles_DeleteCommand);
			this.Load += new System.EventHandler(this.Page_Load);
			((System.ComponentModel.ISupportInitialize)(this.dsFiles)).EndInit();

		}
		#endregion

		private void sendFile_Click(object sender, System.EventArgs e)
		{
			WikiManager manager = WikiManager.Singleton() ;

			// Check to see if file was uploaded
			if (txtFile.PostedFile != null)
			{
				// Get a reference to PostedFile object
				HttpPostedFile myFile = txtFile.PostedFile;

				// Get size of uploaded file
				int nFileLen = myFile.ContentLength; 

				lblInfo.Text = "File of " + Convert.ToString(nFileLen) + "bytes received";

				// make sure the size of the file is > 0
				if( nFileLen > 0 )
				{
					// Allocate a buffer for reading of the file
					byte[] myData = new byte[nFileLen];
					// Read uploaded file from the Stream
					myFile.InputStream.Read(myData, 0, nFileLen);
					// Create a name for the file to store
					string strFilename = Path.GetFileName(myFile.FileName);
					// Prepare target directory
					string path = manager.GetAttachementDirectory(txtPage.Value);
					// Create a file
					FileStream newFile = new FileStream(path + @"\" + strFilename, FileMode.Create);
					// Write data to the file
					newFile.Write(myData, 0, myData.Length);
					// Write meta data file		
					XmlTextWriter meta = new XmlTextWriter(path + @"\" + strFilename + ".rikiwiki.xml",System.Text.Encoding.UTF8);
					meta.WriteStartElement("RIKIWIKI");
					meta.WriteStartElement("METADATA");
					meta.WriteElementString("COMMENT",txtComment.Value);
					meta.Close();
					// Close file
					newFile.Close();

					Response.Redirect(wfWikiAttach.GetUrlForViewAttachements(txtPage.Value));
				}
			}
		}

		private void dgFiles_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dsFiles.Tables.Add(WikiManager.GetAttachementsFor(e.Item.ItemIndex,Request.QueryString["page"]));
			dgFiles.DataBind();
		}

		private void dgFiles_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dgFiles.CurrentPageIndex = e.NewPageIndex;
			dsFiles.Tables.Add(WikiManager.GetAttachementsFor(-1,Request.QueryString["page"]));
			dgFiles.DataBind();

		}



	}
}
