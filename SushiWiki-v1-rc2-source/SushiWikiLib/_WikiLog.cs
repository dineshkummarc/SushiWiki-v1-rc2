using System;
using System.Threading;
using System.Collections;
using System.IO;


namespace Wiki.Tools
{

	/// <summary>
	/// Thanks to MartinL for this very cool log class.
	/// 
	/// It uses a queue and a thread.
	/// </summary>
	public class WikiLog : IDisposable
	{
		private AutoResetEvent evt;
		private Queue store;
		private Thread pwriteThread = null;
		private bool bThreadQuit = false;
		private string filename;

		protected bool IsMessage()
		{
			lock (store.SyncRoot)
				if (store.Count > 0) return true;
			return false;
		}

		protected string GetMessage()
		{
			lock (store.SyncRoot)
				if (store.Count > 0) return (string)store.Dequeue();
			return null;
		}

		protected void WriteThread()
		{
			StreamWriter sw = null;

			while (bThreadQuit == false)
			{
				evt.WaitOne();

				try 
				{
					sw = new StreamWriter(filename, true, System.Text.Encoding.ASCII);

					lock (store.SyncRoot)
					{
						while (IsMessage() == true)
						{
							string sMsg = GetMessage();
							sw.WriteLine(sMsg);
						}
					}
				
					sw.Close();
				}
				catch (Exception)
				{
					if (sw != null)
						sw.Close();
				}
			}
		}

		public WikiLog(string fileName)
		{
			filename = fileName + ".log";
			store = Queue.Synchronized(new Queue(0));
			evt = new AutoResetEvent(false);
			
			pwriteThread = new Thread(new ThreadStart(WriteThread));
			pwriteThread.Start();
		}

		~WikiLog()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (pwriteThread != null)
			{
				bThreadQuit = true;
				evt.Set();
				pwriteThread.Join();
				pwriteThread = null;
			}
			GC.SuppressFinalize(this);
		}

		protected void AddMessage(string str)
		{
			lock (store.SyncRoot)
			{
				store.Enqueue(str);
				evt.Set();
			}
		}

		public void LogLine(string str)
		{
			try 
			{
				AddMessage(str);
			}
			catch { }
		}

		public void LogLine(string format, params object[] args)
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.WriteLine(format, args);
			string sText = sw.ToString();
			
			LogLine(sText);
		}

		public TextReader GetLogTextReader()
		{
			return File.OpenText(filename);
		}
	}
}
