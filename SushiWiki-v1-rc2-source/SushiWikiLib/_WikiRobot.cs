using System;
using System.IO;
using System.Diagnostics;
using System.Web.Mail;
using System.Xml;
using System.Web.SessionState;
using System.Data;
using System.Collections;

namespace Wiki.Tools
{
	/// <summary>
	/// This singleton class manages is Wiki's robot. It is called by System.Timer classes.
	/// 
	/// History :
	/// <code>
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 26/07/2002 | EGE         | Initial version from scratch
	/// | 0.2   | 26/08/2002 | EGE         | Added HOUR schedule
	/// | 0.3   | 10/10/2002 | EGE         | Added Log and fixed mail contain
	/// | 0.4   | 17/10/2002 | EGE         | Cache purge
	/// </code>
	/// </summary>
	/// 
	///
	public class WikiRobot
	{
		/// <summary>
		/// Version management : version
		/// </summary>
		public static string v = "0.4";
		/// <summary>
		/// Version management : release
		/// </summary>
		public static int r = 4;

		#region Singleton stuff
		private static WikiRobot singleton = null;

		/// <summary>
		/// Return singleton instance
		/// </summary>
		/// <returns>singletin instance</returns>
		public static WikiRobot Singleton()
		{
			if (singleton == null)
				singleton = new WikiRobot();
			return singleton;
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public WikiRobot()
		{
			if (singleton != null) throw new InvalidOperationException ("Cannot create twice this singleton class") ;
			scheduling = new Hashtable();
		}

		private Hashtable scheduling;

		/// <summary>
		/// Here is where the Robot is waken !
		/// </summary>
		public void MainTicker()
		{
			Log("WAKED","Robot waked");
			try 
			{ 
				CheckForMails(); 
			} 
			catch (Exception e)
			{ 
				Log("EXCEPTION","A exception occured while checking mails : " + e.Message);
			}
			PurgeCache();
		}

		// Private data
		private string localPath;

		/// <summary>
		/// Robot initialisation
		/// </summary>
		/// <param name="localpath">Local path</param>
		public void Init(string localpath)
		{
			localPath = localpath;
			Log("INIT","WikiRobot singleton init (localpath=\""+ localpath + "\")");
			LoadScheduling();

		}

		/// <summary>
		/// Returns last robot cheking date for given user
		/// </summary>
		/// <param name="username">user id</param>
		/// <returns>formatted date or "Never"</returns>
		public string GetLastCheckDate(string username)
		{
			if (scheduling.ContainsKey(username))
				return scheduling[username].ToString();
			else return "Never";
		}

		#region Mail Robot

		/// <summary>
		/// Check for mails to be sent to users
		/// </summary>
		public void CheckForMails()
		{
			Log("CHECKING", "Checking for mail to be send");
			// Browse user config files
			DirectoryInfo d = new DirectoryInfo(localPath + "\\private" );
			foreach (FileInfo f in d.GetFiles())
			{
				if (f.Name.EndsWith(".userconf.xml"))
				{
					// Read user's settings
					XmlTextReader data = new XmlTextReader(f.FullName);
					string sLastNodeName = "";
					string selectedpages = "";
					string email = "";
					string emailfrequence = "";
					string emailpages = "";
					string username = "";
					while (data.Read())
					{
						if (data.NodeType == XmlNodeType.Element)
							sLastNodeName = data.Name;
						if (data.NodeType == XmlNodeType.Text)
						{
							switch (sLastNodeName)
							{
								case "SELECTED_PAGES":
									selectedpages = data.Value;
									break;
								case "EMAIL":
									email = data.Value;
									break;
								case "EMAIL_FREQUENCE":
									emailfrequence = data.Value;
									break;
								case "EMAIL_PAGES":
									emailpages = data.Value;
									break;
								case "USER_NAME":
									username = data.Value;
									break;
							}
						}
					}
					data.Close();
					// Check scheduling
					DateTime last = Convert.ToDateTime((string)scheduling[username]);
					bool doit = false;
					TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - last.Ticks);
					if ( (emailfrequence == "DAY") && ( ts.TotalHours >= 24) ) doit=true;
					if ( (emailfrequence == "WEEK") && ( ts.TotalDays >= 7 ) ) doit=true;
					if ( (emailfrequence == "HOUR") && ( ts.TotalMinutes >= 60 ) ) doit=true;
					if (emailfrequence == "REALTIME") doit = true;
					if (email.Length  == 0) doit=false; // Missing eMail
					if (doit)
					{
						Log("USER","Cheking updates for\"" + username + "\"");
						if (scheduling.ContainsKey(username))
							scheduling[username] = DateTime.Now.ToString();
						else
							scheduling.Add(username,DateTime.Now.ToString());
						SaveScheduling();
						string body = "";
						string subject = "";
						int updates = 0;
						if (emailfrequence == "WEEK")
						{
							subject = "RIKIWIKI : Pages updated this week";
							DataSet datas = WikiManager.Singleton().GetPagesToEmailForWeek();
							body += "This <b>week</b>, <font color=red>" + Convert.ToString(datas.Tables["pages"].Rows.Count) + "</font> pages has been updated<br>";
							body += "Here is the list :<br>";
							foreach (DataRow row in datas.Tables["pages"].Rows)
							{
								body += "<a href=" + WikiSettings.Singleton().webURL + "wiki.aspx?page=" +  row["title"] + ">" + row["title"] + "</a>&nbsp;&nbsp;";
								updates++;
							}
						}
						if (emailfrequence == "DAY")
						{
							subject = "RIKIWIKI : Pages updated in last 24 hours";
							DataSet datas = WikiManager.Singleton().GetPagesToEmailForDay();
							body += "Last <b>24 hours</b>, <font color=red>" + Convert.ToString(datas.Tables["pages"].Rows.Count) + "</font> pages has been updated<br>";
							body += "Here is the list :<br>";
							foreach (DataRow row in datas.Tables["pages"].Rows)
							{
								body += "<a href=" + WikiSettings.Singleton().webURL + "wiki.aspx?page=" +  row["title"] + ">" + row["title"] + "</a>&nbsp;&nbsp;";
								updates++;
							}
						}
						if (emailfrequence == "HOUR")
						{
							subject = "RIKIWIKI : Pages updated last hour";
							DataSet datas = WikiManager.Singleton().GetPagesToEmailForHour();
							body += "Last <b>24 hours</b>, <font color=red>" + Convert.ToString(datas.Tables["pages"].Rows.Count) + "</font> pages has been updated<br>";
							body += "Here is the list :<br>";
							foreach (DataRow row in datas.Tables["pages"].Rows)
							{
								body += "<a href=" + WikiSettings.Singleton().webURL + "wiki.aspx?page=" +  row["title"] + ">" + row["title"] + "</a>&nbsp;&nbsp;";
								updates++;
							}
						}
						if (emailfrequence == "REALTIME")
						{
							subject = "RIKIWIKI : Pages updated in 'realtime'";
							DataSet datas = WikiManager.Singleton().GetPagesToEmail(last);
							body += "Since <b>"+ last.ToString()+"/b>, <font color=red>" + Convert.ToString(datas.Tables["pages"].Rows.Count) + "</font> pages has been updated<br>";
							body += "Here is the list :<br>";
							foreach (DataRow row in datas.Tables["pages"].Rows)
							{
								body += "<a href=" + WikiSettings.Singleton().webURL + "wiki.aspx?page=" +  row["title"] + ">" + row["title"] + "</a>&nbsp;&nbsp;";
								updates++;
							}
						}
						if (updates >0)
						{
							Log("SENDING MAIL","Sending " + updates.ToString() + " updates to " + email);
							try 
							{
								// Current settings
								body += "<br><br>Your current settings are : Send me an eMail every <b>" + emailfrequence + "</b> showing <b>" + emailpages + "</b> updated pages.<br>";
								body += "<a href=" + WikiSettings.Singleton().webURL + "UserSettings.aspx>Click here for changing your settings</a>.";
								MailMessage	mail = new MailMessage();
								mail.From = WikiSettings.Singleton().RobotMailAdress;
								mail.To = email;
								mail.Priority = MailPriority.Normal;
								mail.Subject = subject;
								mail.BodyFormat = MailFormat.Html;
								mail.Body = body;
								if (WikiSettings.Singleton().smtpServer.Length > 0)
								{
									SmtpMail.SmtpServer = WikiSettings.Singleton().smtpServer;
								}
								SmtpMail.Send(mail);
							}
							catch (Exception ex)
							{
								Log("EXCEPTION",ex.Message);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Loads scheduling information (stored in /private/robot_scheduling.xml)
		/// </summary>
		public void LoadScheduling()
		{
			string file = localPath + "\\private\\robot_scheduling.xml";
			Log("SCHEDULING","Loading scheduling from \"" + file + "\"");
			if (System.IO.File.Exists(file))
			{ // Load settings
				XmlTextReader data = new XmlTextReader(file);
				string sLastNodeName = "";
				while (data.Read())
				{
					if (data.NodeType == XmlNodeType.Element)
						sLastNodeName = data.Name;
					if (data.NodeType == XmlNodeType.Text)
					{
						switch (sLastNodeName)
						{
							case "DATES":
							{
								string[] a = data.Value.Split('|');
								foreach (string s in a)
								{
									if (s.Length > 0)
									{
										string[] b = s.Split('=');
										scheduling.Add(b[0],b[1]);
									}
								}
							}
							break;
						}
					}
				}
				data.Close();
			}
			Log("SCHEDULING",scheduling.Count.ToString() + " schedule(s) found");
		}

		/// <summary>
		/// Save scheduling information (into in /private/robot_scheduling.xml)
		/// </summary>
		public void SaveScheduling()
		{
			string file = localPath + "\\private\\robot_scheduling.xml";
			string s = "|";
			IDictionaryEnumerator enumer = scheduling.GetEnumerator();
			while ( enumer.MoveNext() )
			{
				s += (string)enumer.Key + "=" + (string)enumer.Value + "|";
			}
			XmlTextWriter data = new XmlTextWriter(file,System.Text.Encoding.UTF8);
			data.WriteStartElement("RIKIWIKI");
			data.WriteStartElement("ROBOT_SCHEDULING");
			data.WriteElementString("DATES",s);
			data.Close();
		}

		#endregion


		/// <summary>
		/// Robot activities loggin
		/// </summary>
		/// <param name="type">Log message type</param>
		/// <param name="sErrMsg">Log message</param>
		public void Log(string type,string sErrMsg)
		{
			WikiManager.Singleton().Log('i',"ROBOT",type,sErrMsg);
		}

		/// <summary>
		/// Check for obsolete files and delete them.
		/// </summary>
		public void PurgeCache()
		{
			// Delete tmp.xml files after 1 hour (create by the export tool in WikiManager)
			DirectoryInfo di = new DirectoryInfo(localPath + "\\cache");
			foreach (FileInfo fi in di.GetFiles("*.tmp.xml"))
			{
				if (fi.LastAccessTime.Subtract(DateTime.Now).Ticks > TimeSpan.TicksPerHour)
				{
					this.Log("CACHE","Deleting " + fi.Name);
					fi.Delete();
				}
			}
		}
	}
}
