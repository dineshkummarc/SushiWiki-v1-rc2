using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Timers;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Threading;
using System.Globalization;

using Wiki.Tools;
using Wiki.GUI;

namespace Wiki 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Main timer. Used for calling Robot.
		/// </summary>
		public static System.Timers.Timer mainTimer;

		/// <summary>
		/// Main timer tick method.
		/// </summary>
		/// <param name="sender">event sender</param>
		/// <param name="e">event arguments</param>
		protected void mainTimerTick(object sender, System.Timers.ElapsedEventArgs e)
		{
			WikiRobot.Singleton().MainTicker();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public Global()
		{
			InitializeComponent();
		}	
		
		/// <summary>
		/// Application start. 
		/// Inits application settings, creates Robot and start timer for calling robot.
		/// </summary>
		/// <param name="sender">event sender</param>
		/// <param name="e">event arguments</param>
		protected void Application_Start(Object sender, EventArgs e)
		{
			// Singletons init
			WikiSettings.Singleton().SetContextSettings(Server.MapPath("."));
			WikiManager.Singleton().Init(Context.Cache);

			WikiManager.Singleton().Log('s',"GLOBL","START",@"  _______");
			WikiManager.Singleton().Log('s',"GLOBL","START",@" /       \");
			WikiManager.Singleton().Log('s',"GLOBL","START",@"<SushiWIKI> is starting...");
			WikiManager.Singleton().Log('s',"GLOBL","START",@" \_______/");
			WikiManager.Singleton().Log('s',"GLOBL","START","  "+WikiManager.v);
			WikiSettings.Singleton().LogStatus();
			WikiManager.Singleton().InitPlugins();
			// Robot launching
			if (WikiSettings.Singleton().RobotTimer >0)
			{
				WikiRobot.Singleton().Init(Server.MapPath(".")); 
				mainTimer = new System.Timers.Timer();
				mainTimer.Interval = WikiSettings.Singleton().RobotTimer * 1000;
				mainTimer.Elapsed += new ElapsedEventHandler(mainTimerTick);
				mainTimer.Enabled = true;
				WikiManager.Singleton().Log('i',"ROBOT","SCHEDULING","Robot will be launched every " + WikiSettings.Singleton().RobotTimer + "s");
			}
			else
			{
				WikiManager.Singleton().Log('i',"ROBOT","SCHEDULING","Robot not launched (robot_timer=0 in web.config)");
			}
			// Load strings ressources
			WikiGui.LoadStrings(); 
		}
 
		/// <summary>
		/// New user session is starting. 
		/// </summary>
		/// <param name="sender">event sender</param>
		/// <param name="e">event arguments</param>
		protected void Session_Start(Object sender, EventArgs e)
		{
			string user = HttpContext.Current.User.Identity.Name;
			if (user != "")
			{
				Session["username"] = user;
			}
			else
			{
				WikiManager.Singleton().Log('i',"GLOBL","SESSION_START","Anonymous user -> switching to Guest");
				Session["username"] = "Guest";
			}
			// Log
			WikiManager.Singleton().Log('i',"GLOBL","SESSION_START","User is <" + (string)Session["username"] + ">");
			// Load user settings
			WikiUserSettings.Singleton().LoadUserSettings(Session,WikiManager.Singleton().IsThisALocalIP(Request.UserHostAddress));
		}

		protected void Application_End(Object sender, EventArgs e)
		{
			WikiManager.Singleton().Dispose();
		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			//WikiManager.Singleton().Log("d","GLOBL","","Application_AuthenticateRequest");
		}

//		protected void Application_BeginRequest(Object sender, EventArgs e)
//		{
//
//		}

//		protected void Application_EndRequest(Object sender, EventArgs e)
//		{
//
//		}

//		protected void Application_Error(Object sender, EventArgs e)
//		{
//
//		}

//		protected void Session_End(Object sender, EventArgs e)
//		{
//
//		}		
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

