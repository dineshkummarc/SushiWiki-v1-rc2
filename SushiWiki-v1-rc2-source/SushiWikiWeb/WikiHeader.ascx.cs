namespace Wiki.GUI.Templates
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Web.Security;
	using Wiki.GUI;
	using System.Resources;
	using System.Security;

	/// <summary>
	///		Summary description for Header.
	/// </summary>
	public abstract class Header : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblUser;
		protected System.Web.UI.WebControls.HyperLink hlHelp;
		protected System.Web.UI.WebControls.HyperLink hlIndex;
		protected System.Web.UI.WebControls.HyperLink hlTools;
		protected System.Web.UI.WebControls.HyperLink hlSearch;
		protected System.Web.UI.WebControls.Image imgUser;
		protected System.Web.UI.WebControls.HyperLink hlHome;
		protected System.Web.UI.WebControls.HyperLink hlUser;
		protected System.Web.UI.WebControls.HyperLink hlLogin;
		protected System.Web.UI.WebControls.HyperLink hlCalendar;
		protected System.Web.UI.WebControls.ImageButton ImageButton1;
		protected System.Web.UI.WebControls.ImageButton Go;
		protected System.Web.UI.WebControls.LinkButton lbLogout;
		protected System.Web.UI.WebControls.TextBox txtGoTo;

		private void Page_Load(object sender, System.EventArgs e)
		{ 
			// Hide login link if NT login mode
			if (WikiSettings.Singleton().LoginMode == "WINDOWS") 
			{
				hlLogin.Visible = false;
				lbLogout.Visible = false;
			}
			else
			{
				if ((string)Session["username"] == "Guest")
				{
					hlLogin.Visible = true;
					lbLogout.Visible = false;
				}
				else
				{
					hlLogin.Visible = false;
					lbLogout.Visible = true;
				}
			}
			string user = WikiUserSettings.Singleton().GetUserName(Session);
			lblUser.Text = user;
			if (WikiUserSettings.IsUserAdministrator(Session))
			{ 
				imgUser.ImageUrl = "images/icon_user_admin.gif";
				WikiGui.AddPopupToWebControl(imgUser,WikiGui.GetString("Guide.Header.AdminIcon"));			
			}
			else
			{ 
				if (user == "Guest")
				{
					imgUser.ImageUrl = "images/icon_user_guest.gif"; 
					WikiGui.AddPopupToWebControl(imgUser,WikiGui.GetString("Guide.Header.GuestIcon"));			
				}
				else
				{
					imgUser.ImageUrl = "images/icon_user.gif";
					WikiGui.AddPopupToWebControl(imgUser,WikiGui.GetString("Guide.Header.UserIcon"));			
				}
			}
			// Display help link if help page exists
			WikiPage wf = (WikiPage)this.Parent.Parent; // this->HTMLForm->WebForm
			try 
			{ wf.DisplayHelpLink(hlHelp); }
			catch 
			{ // Data acces error -> do nothing because error page uses this template (infinite loop!) 
			}
			// Add Popups
			// Load messages ressource
			WikiGui.AddPopupToWebControl(hlHome,WikiGui.GetHtmlString("Guide.Header.Home"));// "Go to home page");
			WikiGui.AddPopupToWebControl(hlUser,WikiGui.GetHtmlString("Guide.Header.User"));
			WikiGui.AddPopupToWebControl(hlHelp,WikiGui.GetHtmlString("Guide.Header.Help"));
			WikiGui.AddPopupToWebControl(hlIndex,WikiGui.GetHtmlString("Guide.Header.Index"));
			WikiGui.AddPopupToWebControl(hlCalendar,WikiGui.GetHtmlString("Guide.Header.Calendar"));
			WikiGui.AddPopupToWebControl(hlTools,WikiGui.GetHtmlString("Guide.Header.Tools"));
			WikiGui.AddPopupToWebControl(hlLogin,WikiGui.GetHtmlString("Guide.Header.Login"));
			WikiGui.AddPopupToWebControl(hlSearch,WikiGui.GetHtmlString("Guide.Header.Search"));
			WikiGui.AddPopupToWebControl(txtGoTo,WikiGui.GetHtmlString("Guide.Header.Goto"));
			WikiGui.AddPopupToWebControl(Go,WikiGui.GetHtmlString("Guide.Header.Goto"));
			//do databinding to localize strings in ascx.
			DataBind();
			
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtGoTo.TextChanged += new System.EventHandler(this.txtGoTo_TextChanged);
			this.lbLogout.Click += new System.EventHandler(this.lbLogout_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void txtGoTo_TextChanged(object sender, System.EventArgs e)
		{
			Response.Redirect (wfWiki.GetUrlForOpenPage(txtGoTo.Text)) ;		
		}

		private void lbLogout_Click(object sender, System.EventArgs e)
		{
			FormsAuthentication.SignOut();
			Session["username"] = "Guest";
			WikiUserSettings.Singleton().LoadUserSettings(Session,WikiManager.Singleton().IsThisALocalIP(Request.UserHostAddress));
			Response.Redirect(wfWiki.GetUrlForOpenHomePage());
		}
	}
}