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
using System.IO;
using Wiki.Tools;
using System.Security.Principal;
using System.Web.Security;


namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for UserSettings.
	/// </summary>
	[WikiPageHelp("UserSettings")]
	[WikiPageSecurity(true,false)]
	public class wfUserSettings : WikiPage
	{
		protected System.Web.UI.WebControls.Label lblSelectedPages;
		protected System.Web.UI.WebControls.TextBox txtEMail;
		protected System.Web.UI.WebControls.RadioButton rbWeek;
		protected System.Web.UI.WebControls.RadioButton rbDay;
		protected System.Web.UI.WebControls.RadioButton rbAll;
		protected System.Web.UI.WebControls.RadioButton rbSelected;
		protected System.Web.UI.WebControls.LinkButton saveEMailSettings;
		protected System.Web.UI.WebControls.Label lblEMail;
		protected System.Web.UI.WebControls.LinkButton lbCheckNow;
		protected System.Web.UI.WebControls.RadioButton rbNone;
		protected System.Web.UI.WebControls.RadioButton rbRealTime;
		protected System.Web.UI.WebControls.DropDownList ddlStyles;
		protected System.Web.UI.WebControls.LinkButton lbSaveStyle;
		protected System.Web.UI.WebControls.CheckBox cbFullScreen;
		protected System.Web.UI.WebControls.LinkButton lbSaveFullScreen;
		protected System.Web.UI.WebControls.Label lblRole;
		protected System.Web.UI.WebControls.LinkButton lbLogout;
		protected System.Web.UI.WebControls.Panel pnlSettings;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.Label lblPersonnalPage;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Load string
			lblTitle.Text = WikiGui.GetString("Gui.WikiUserSettings.Title");
			// robot link only if robot is enabled
			lbCheckNow.Visible = (WikiSettings.Singleton().RobotTimer >0);
			// If Guest user -> read only
			if ( (User.Identity.Name != "Guest") && (User.Identity.Name.Length > 0) && (User.Identity.Name != null) )
			{
				WikiManager manager = WikiManager.Singleton();
				// USER Page
				string userPage = "WikiUser." + HttpContext.Current.User.Identity.Name.Replace("\\","-").Replace(" ","");
				WikiManager.WikiPageShortInfo info = manager.GetPageShortInfo(userPage);

				if (info.pageFound)
				{
					lblPersonnalPage.Text = WikiGui.ExistingWikiPageLink(userPage,userPage,info);
				}
				else
				{
					lblPersonnalPage.Text = "<font color=red>Your personal has not been created !</font><br>" + WikiGui.NewWikiPageLink(userPage,userPage);
				}
				lblSelectedPages.Text = "";
				// Role
				WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				if ( wp.IsInRole("WikiAdmin")) lblRole.Text = "You are administrator";
				// Selected pages
				foreach (string s in WikiUserSettings.Singleton().GetUserSelectedPages(Session))
				{
					WikiManager.WikiPageShortInfo linfo = manager.GetPageShortInfo(s);
					lblSelectedPages.Text += WikiGui.ExistingWikiPageLink(s,s,linfo) + "&nbsp;&nbsp;";
				}
				// fullscreen
				if ( ((string)Session["fullscreen"]) == "ON") cbFullScreen.Checked = true;

				if (!IsPostBack)
				{
					txtEMail.Text = (string)Session["email"];
					rbNone.Checked = ((string)Session["email_frequence"] == "NONE");
					rbDay.Checked = ((string)Session["email_frequence"] == "DAY"); 
					rbWeek.Checked = ((string)Session["email_frequence"] == "WEEK"); 
					rbRealTime.Checked = ((string)Session["email_frequence"] == "REALTIME"); 
					rbAll.Checked = ((string)Session["email_pages"] == "ALL"); 
					rbSelected.Checked = ((string)Session["email_pages"] == "SELECTED"); 
					lblEMail.Text = "Last send : " + WikiRobot.Singleton().GetLastCheckDate((string)Session["username"]);

					// Available styles
					DirectoryInfo d = new DirectoryInfo(Server.MapPath("styles"));
					int i=0; int n=0;
					foreach (FileInfo f in d.GetFiles("*.css"))
					{
						ListItem li = new ListItem(f.Name);
						if (f.Name == (string)Session["userstyle"]) n = i;
						ddlStyles.Items.Add(li);
						i++;
					}
					ddlStyles.SelectedIndex = n;
				}
			}
			else
			{ // Guest user
				pnlSettings.Visible = false;
				lblInfo.Visible = true;
				lblInfo.Text = WikiGui.GetString("Gui.WikiUserSettings.GuestUser");
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
			this.lbSaveFullScreen.Click += new System.EventHandler(this.lbSaveFullScreen_Click);
			this.saveEMailSettings.Click += new System.EventHandler(this.saveEMailSettings_Click);
			this.lbCheckNow.Click += new System.EventHandler(this.lbCheckNow_Click);
			this.lbSaveStyle.Click += new System.EventHandler(this.lbSaveStyle_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void saveEMailSettings_Click(object sender, System.EventArgs e)
		{
			Session["email"] = txtEMail.Text;
			if (rbNone.Checked) Session["email_frequence"] = "NONE";
			if (rbDay.Checked) Session["email_frequence"] = "DAY";
			if (rbWeek.Checked) Session["email_frequence"] = "WEEK";
			if (rbRealTime.Checked) Session["email_frequence"] = "REALTIME";
			if (rbAll.Checked) Session["email_pages"] = "ALL";
			if (rbSelected.Checked) Session["email_pages"] = "SELECTED";
			WikiUserSettings.Singleton().SaveUserSettings(Session);
		}

		private void lbCheckNow_Click(object sender, System.EventArgs e)
		{
			WikiRobot.Singleton().CheckForMails();
		}

		private void lbSaveStyle_Click(object sender, System.EventArgs e)
		{
			Session["userstyle"] = ddlStyles.SelectedItem.Value;
			WikiUserSettings.Singleton().SaveUserSettings(Session);
		}


		private void lbSaveFullScreen_Click(object sender, System.EventArgs e)
		{
			if (cbFullScreen.Checked) Session["fullscreen"] = "ON";
			else Session["fullscreen"] = "OFF";
			WikiUserSettings.Singleton().SaveUserSettings(Session);
		
		}

	}
}
