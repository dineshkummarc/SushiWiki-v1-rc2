using System;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Resources;
using System.IO;
using System.Web.Security;
using System.Security.Principal;
using System.Net;


namespace Wiki.GUI
{

	/// <summary>
	/// Parent class of Wiki's WebForms.
	/// 
	/// - Help page management
	/// - Authentification
	/// </summary>
	public class WikiPage : System.Web.UI.Page
	{
		public readonly static string HelpPagePrefix = "WikiHelp.PageHelp.";

		private string p_helpId;
		private bool p_isAuthenticationRequired = true;
		private bool p_isAdminRoleRequired = false;

		public WikiPage()
		{
			// Scan Wiki custom attributes
			// Warning : We must get base type because ASP.NET generates a new class at runtime.
			WikiPageHelpAttribute[] helps = (WikiPageHelpAttribute[])this.GetType().BaseType.GetCustomAttributes(typeof(WikiPageHelpAttribute),false);
			if (helps.Length == 1)
			{
				p_helpId = helps[0].HelpId;
			}
			WikiPageSecurityAttribute[] secus = (WikiPageSecurityAttribute[])this.GetType().BaseType.GetCustomAttributes(typeof(WikiPageSecurityAttribute),false);
			if (secus.Length == 1)
			{
				p_isAuthenticationRequired = secus[0].IsAuthenticationRequired;
				p_isAdminRoleRequired = secus[0].IsAdminRoleRequired;
			}
			// Handle the "Load" event
			this.Load += new System.EventHandler(this.LoadEventHandler);
		}

		private void LoadEventHandler(object sender, EventArgs e)
		{
			if (p_isAuthenticationRequired)
			{
				CheckUser();
			}
			if (p_isAdminRoleRequired)
			{
				if (!(bool)Session["admin"])
				{
					Response.Redirect("WikiError.aspx?code=4");
				}
			}
		}

		/// <summary>
		/// Displays help page link
		/// The link appears if WikiHelp.PageHelp.[helpId] wiki page exists.
		/// </summary>
		/// <param name="link">HyperLink widget</param>
		/// <param name="image">Image widget (</param>
		/// <returns>true if help exists</returns>
		public bool DisplayHelpLink(HyperLink link, Image image)
		{
			string helpPage = HelpPagePrefix + p_helpId;
			if (WikiManager.Singleton().GetPageShortInfo(helpPage).pageFound)
			{
				if (image != null) image.Visible = true;
				link.Visible = true;
				link.NavigateUrl = "Wiki.aspx?page=" + helpPage;
				return true;
			}
			else
			{
				if (image != null) image.Visible = false;
				link.Visible = false;
				return false;
			}
		}

		/// <summary>
		/// See DisplayHelpLink(HyperLink link, Image image)
		/// </summary>
		/// <param name="link">HyperLink widget</param>
		/// <returns>true if help exists</returns>
		public bool DisplayHelpLink(HyperLink link)
		{ return DisplayHelpLink(link,null); }


		/// <summary>
		/// Return current user name
		/// </summary>
		/// <returns>current user name</returns>
		public string GetUserName()
		{
			string user = User.Identity.Name;
			if (user == "") user = "Guest";
			return user;
		}

		/// <summary>
		/// Detects user and created correct credentials
		/// </summary>
		private void CheckUser()
		{
			if (!WikiUserSettings.Singleton().AreUserSettingsLoaded(Session))
			{
				// If user is not authenticated -> Force Use guest account
				if (!User.Identity.IsAuthenticated)
				{ 
					WikiManager.Singleton().Log('i',"SECUR","NOT AUTHENTICADED","Using Guest user");
					if (FormsAuthentication.Authenticate("Guest","Guest"))
					{
						WikiManager.Singleton().Log('i',"SECUR","FORMS AUTHENTICATION","Guest user created using Forms Authentication");
					}
					else
					{
						WikiManager.Singleton().Log('i',"SECUR","NT AUTHENTICATION","Guest user created using NT Authentication");
						GenericIdentity guser = new GenericIdentity("Guest");
						Context.User = new GenericPrincipal(guser,null);
					}
					WikiUserSettings.Singleton().LoadUserSettings(Session,WikiManager.Singleton().IsThisALocalIP(Request.UserHostAddress));
					// If user is local : make him administrator
					if (WikiSettings.Singleton().isLocalUserAdmin)
					{
						string hostname = Dns.GetHostName();
						IPHostEntry iphostentry = Dns.GetHostByName(hostname);
						bool ipfound = false;
						IPAddress localip = IPAddress.Parse(Request.UserHostAddress);
						foreach (IPAddress ip in iphostentry.AddressList)
						{
							if (localip == ip) ipfound = true;
						}
						IPAddress ip127 = IPAddress.Parse("127.0.0.1");
						if (ip127.Equals(localip))
						{
							ipfound = true;
						}
						if (ipfound) Session["admin"] = true;
					}
				}
			}
			string user = WikiUserSettings.Singleton().GetUserName(Session);
			if (user == "") 
			{
				WikiManager.Singleton().Log('i',"SECUR","USER NAME EMPTY","Using Guest user again");
				user = "Guest";
				Session["username"] = user;
				WikiUserSettings.Singleton().LoadUserSettings(Session,false);
			}

		}
	} // WikiPage

	[AttributeUsage(AttributeTargets.Class)]
	public class WikiPageHelpAttribute : Attribute
	{
		public WikiPageHelpAttribute(string helpid)
		{
			p_helpid = helpid;
		}

		private string p_helpid;

		public string HelpId
		{
			get { return p_helpid; }
		}

	}

	
	public class WikiPageSecurityAttribute : Attribute
	{
		public WikiPageSecurityAttribute(bool authenticationRequired,bool adminrolerequired)
		{
			p_isAuthenticationRequired = authenticationRequired;
			p_isAdminRoleRequired = adminrolerequired;
		}

		private bool p_isAuthenticationRequired;
		private bool p_isAdminRoleRequired;

		public bool IsAuthenticationRequired
		{
			get { return p_isAuthenticationRequired; }
		}

		public bool IsAdminRoleRequired
		{
			get { return p_isAdminRoleRequired; }
		}
	}
	
	}