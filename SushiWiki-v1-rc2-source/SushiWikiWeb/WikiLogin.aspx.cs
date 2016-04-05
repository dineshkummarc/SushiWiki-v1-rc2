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
using System.Web.Security;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Web.Mail;

namespace Wiki.GUI
{
	/// <summary>
	/// Summary description for WikiLogin.
	/// </summary>
	[WikiPageHelp("Login")]
	[WikiPageSecurity(false,false)]
	public class wfWikiLogin : WikiPage
	{
		protected System.Web.UI.WebControls.Label outMessage;
		protected System.Web.UI.WebControls.Label lblInfo;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
		protected System.Web.UI.WebControls.Label lblUserName;
		protected System.Web.UI.WebControls.Label lblPassword;
		protected System.Web.UI.WebControls.Button butSubmit;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator3;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator4;
		protected System.Web.UI.HtmlControls.HtmlInputText Text1;
		protected System.Web.UI.HtmlControls.HtmlInputText Password1;
		protected System.Web.UI.WebControls.Panel pnlLogin;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator5;
		protected System.Web.UI.HtmlControls.HtmlInputText Password2;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator6;
		protected System.Web.UI.HtmlControls.HtmlInputText Password3;
		protected System.Web.UI.WebControls.CheckBox cbPersist;
		protected System.Web.UI.WebControls.TextBox tbUser;
		protected System.Web.UI.WebControls.TextBox tbPassword;
		protected System.Web.UI.WebControls.Label lblNewUser;
		protected System.Web.UI.WebControls.TextBox tbNewUser;
		protected System.Web.UI.WebControls.Label lblNewPassword;
		protected System.Web.UI.WebControls.TextBox tbNewPassword;
		protected System.Web.UI.WebControls.Label lblRetypePassword;
		protected System.Web.UI.WebControls.TextBox tbRetypePassword;
		protected System.Web.UI.WebControls.Label lblEmail;
		protected System.Web.UI.WebControls.TextBox tbEmail;
		protected System.Web.UI.WebControls.Button butNewUser;
		protected System.Web.UI.WebControls.Label lblEmailInfo;
		protected System.Web.UI.WebControls.Panel pnlNewUser;
		protected System.Web.UI.WebControls.LinkButton lbNewUser;
		protected System.Web.UI.WebControls.Label lblTitle;
	

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
			this.butSubmit.Click += new System.EventHandler(this.butSubmit_Click);
			this.lbNewUser.Click += new System.EventHandler(this.lbNewUser_Click);
			this.butNewUser.Click += new System.EventHandler(this.butNewUser_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			lblTitle.Text = WikiGui.GetString("Gui.WikiLogin.Title");
			// Account activation .
			string activationKey = Request.QueryString["activate"];
			if (activationKey != null)
			{
				CheckActivation(activationKey);
				return;
			}
			// Check current login mode
			switch (WikiSettings.Singleton().LoginMode)
			{
				case "NT":
					pnlLogin.Visible = false;
					outMessage.Text = WikiGui.GetHtmlString("Gui.WikiLogin.NTAuthentication");
					break;
				case "CONFIG":
					break;
				case "XML":
					lbNewUser.Visible = true;
					break;
			}
			lblInfo.Text = WikiGui.GetHtmlString("Gui.WikiLogin.CurrentlyLogged") + "<b>"+(string)Session["username"] + "</b><br><br>";
			// Load string
			lblNewUser.Text = lblUserName.Text = WikiGui.GetString("Gui.WikiLogin.UserName");
			lblNewPassword.Text = lblPassword.Text = WikiGui.GetString("Gui.WikiLogin.Password");
			lblRetypePassword.Text = WikiGui.GetString("Gui.WikiLogin.RetypePassword");
			lblEmail.Text = WikiGui.GetString("Gui.WikiLogin.Email");
			lblEmailInfo.Text = WikiGui.GetString("Gui.WikiLogin.EmailInfo");
			cbPersist.Text = WikiGui.GetString("Gui.WikiLogin.Cookie");
			butSubmit.Text = WikiGui.GetString("Gui.WikiLogin.Submit");
			butNewUser.Text = WikiGui.GetString("Gui.WikiLogin.NewUserAction");
			lbNewUser.Text = WikiGui.GetString("Gui.WikiLogin.NewUser");
		}

		private void CheckActivation(string pKey)
		{
			pnlLogin.Visible = false;
			butSubmit.Visible = false;
			Login[] logins = LoadLogins();
			Login foundLogin = null;
			if (logins != null) 
			{
				foreach (Login login in logins)
				{
					if (login.activationKey == pKey)
					{
						foundLogin = login;
						break;
					}
				}
			}
			if (foundLogin == null)
			{
				outMessage.Text = WikiGui.GetString("Gui.WikiLogin.ActivationFailed");
			}
			else
			{
				foundLogin.activated = true;
				foundLogin.activationKey = null;
				SaveLogins(logins);
				outMessage.Text = WikiGui.GetString("Gui.WikiLogin.ActivationSuccess");
			}
		}
 
		private void butSubmit_Click(object sender, System.EventArgs e)
		{
			WikiManager.Singleton().Log('i',"LOGIN","AUTHENTICATE","<" + tbUser.Text + "> is trying to logon (current mode = " + WikiSettings.Singleton().LoginMode + ")");
			if (WikiSettings.Singleton().LoginMode == "CONFIG")
			{
				// Users-Passwords are directly stored in Web.config
				if (FormsAuthentication.Authenticate(tbUser.Text,tbPassword.Text))
				{
					// Authentication successfull
					Session["username"] = tbUser.Text;
					WikiManager.Singleton().Log('i',"LOGIN","SUCCESS","<" + tbUser.Text + "> is succefully authenticated");
					WikiUserSettings.Singleton().LoadUserSettings(Session,WikiManager.Singleton().IsThisALocalIP(Request.UserHostAddress));
					if (cbPersist.Checked) WikiManager.Singleton().Log('i',"LOGIN","COOKIE","Credential is stored in a cookie");
					FormsAuthentication.RedirectFromLoginPage(tbUser.Text,cbPersist.Checked);
					if (Request.QueryString["ReturnURL"] == null)
					{
						Response.Redirect(wfWiki.GetUrlForOpenHomePage());
					}
				}
				else
				{
					// Authentication failed
					WikiManager.Singleton().Log('w',"LOGIN","FAIL","<" + tbUser.Text + "> authentication failed");
					outMessage.Text =  WikiGui.GetString("Gui.WikiLogin.InvalidLogin");
				}
			}
			else if (WikiSettings.Singleton().LoginMode == "XML")
			{
				// Users-Passwords are stored in private/users.xml
				if (XmlAuthenticate(tbUser.Text,tbPassword.Text))
				{
					Session["username"] = tbUser.Text;
					WikiManager.Singleton().Log('i',"LOGIN","SUCCESS","<" + tbUser.Text + "> authentication succeeded");
					WikiUserSettings.Singleton().LoadUserSettings(Session,WikiManager.Singleton().IsThisALocalIP(Request.UserHostAddress));
					Response.SetCookie(FormsAuthentication.GetAuthCookie(tbUser.Text,cbPersist.Checked));
					FormsAuthentication.RedirectFromLoginPage(tbUser.Text,cbPersist.Checked);
					if (Request.QueryString["ReturnURL"] == null)
					{
						Response.Redirect(wfWiki.GetUrlForOpenHomePage());
					}				
				}
				else
				{
					// Authentication failed
					WikiManager.Singleton().Log('w',"LOGIN","FAIL","<" + tbUser.Text + "> authentication failed");
					outMessage.Text =  WikiGui.GetString("Gui.WikiLogin.InvalidLogin");
				}

			}
		}

		private bool XmlAuthenticate(string user,string password)
		{
			Login[] logins = LoadLogins();
			if (logins == null) return false;
			string cryptedPass = FormsAuthentication.HashPasswordForStoringInConfigFile(tbPassword.Text,"md5");
			foreach (Login login in logins)
			{
				if ( (login.activated) &&  (login.password == tbPassword.Text) )
				{
					Session["login_email"] = login.eMail;
					return true;
				}
			}
			return false;
		}


		public class Login
		{
			public string user ;
			public string password;
			public bool activated;
			public DateTime creationDate;
			public string eMail;
			public string activationKey;
		}

		private Login[] LoadLogins()
		{
			// Load the data
			string file = Path.Combine(WikiSettings.Singleton().LocalPath,"private/users.xml");
			if (File.Exists(file))
			{
				using (TextReader reader = new StreamReader(file))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Login[]));
					return (Login[])serializer.Deserialize(reader);
				}
			}
			return null;
		}

		private void SaveLogins(Login[] pLogins)
		{
			lock(typeof(wfWikiLogin))
			{
				// Save to XML
				string file = Path.Combine(WikiSettings.Singleton().LocalPath,"private/users.xml");
				using (StreamWriter writer = File.CreateText(file))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Login[]));
					serializer.Serialize(writer,pLogins); 
				}
			}
		}

		private void lbNewUser_Click(object sender, System.EventArgs e)
		{
			pnlLogin.Visible = false;
			pnlNewUser.Visible = true;
		}

		private void butNewUser_Click(object sender, System.EventArgs e)
		{
			Login[] logins = LoadLogins();
			// Check if user name is already used
			bool usernameOk = (tbNewUser.Text != "Guest");
			if (logins !=null) foreach (Login l in logins)
			{
				if (tbNewUser.Text == l.user)
	 			{
					usernameOk = false;
		 			break;
				}
			}
			if (!usernameOk)
			{
				outMessage.Text = WikiGui.GetString("Gui.WikiLogin.UsernameAlreadyUsed");
				tbNewUser.BackColor = Color.Red;
				return;
			}
			else
			{
				tbNewUser.BackColor = Color.Empty;
			}
			if (tbNewPassword.Text != tbRetypePassword.Text)
			{
				tbRetypePassword.BackColor = Color.Red;
				return;
			} else tbRetypePassword.BackColor = Color.Empty;
			Login[] nlogins = null;
			Login login = new Login();
			login.user = tbNewUser.Text;
			login.password = tbNewPassword.Text;
			login.eMail = tbEmail.Text;
			login.activated = false;
			login.creationDate = DateTime.Now;
			login.activationKey = DateTime.Now.ToString("yyyyMMdd")+DateTime.Now.Ticks+Process.GetCurrentProcess().TotalProcessorTime;
			// Sending mail
			MailMessage mail = new MailMessage();
			mail.From = WikiSettings.Singleton().RobotMailAdress;
			mail.To = tbEmail.Text;
			mail.Priority = MailPriority.Normal;
			mail.Subject = WikiGui.GetString("Mail.WikiLogin.ActivationSubject");
			mail.BodyFormat = MailFormat.Html;
			mail.Body = "<a href=" + WikiSettings.Singleton().webURL + "WikiLogin.aspx?activate=" + login.activationKey + ">"+WikiGui.GetString("Mail.WikiLogin.ActivationLink")+"</a>";
			SmtpMail.SmtpServer = WikiSettings.Singleton().smtpServer;
			try
			{
				SmtpMail.Send(mail);																																				
			}
			catch (Exception ex)
			{
				WikiManager.Singleton().Log('w',"LOGIN","SEND MAIL","Failed sending mail to <" + login.eMail + "> using <" + SmtpMail.SmtpServer  + "> : " + ex.Message);
				outMessage.Text = WikiGui.GetString("Gui.WikiLogin.SendMailFailed",login.eMail);
				return;
			}
			WikiManager.Singleton().Log('i',"LOGIN","SEND MAIL","Mail sent to <" + login.eMail + "> using <" + SmtpMail.SmtpServer  + ">");
			// Storing user data
			if (logins == null)
				nlogins = new Login[] {login };
			else 
			{
				nlogins = new Login[logins.Length+1];
				logins.CopyTo(nlogins,0);
				nlogins[logins.Length] = login;
			}
			// Save to XML
			SaveLogins(nlogins);
			// Display info
			pnlNewUser.Visible = false;
			butNewUser.Visible = false;
			outMessage.Text = WikiGui.GetString("Gui.WikiLogin.SendMailSuccess",login.eMail);
		}
	}
}
