using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Wiki.Plugins
{
	/// <summary>
	/// This is the plugin manager. It scans for plugins in the plgugin directory and 
	/// stores them for further usage.
	/// </summary>
	public class WikiPluginsManager
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pPluginsPath">Path to scan for plugins</param>
		public WikiPluginsManager(string pPluginsPath,CultureInfo pCulture,LogDelegate pLogDelegate)
		{
			// Scan plugins
			if (Directory.Exists(pPluginsPath))
			{
				DirectoryInfo di = new DirectoryInfo(pPluginsPath);
				foreach (FileInfo fi in di.GetFiles("*.dll"))
				{
					Debug.WriteLine("dll=" + fi.FullName);
					try
					{
						CheckDll(fi.FullName,pCulture,pLogDelegate);	
					} 
					catch 
					{
						Debug.WriteLine("CheckDll failed");
					}
				}
			}
		}

		/// <summary>
		/// Loads the DLL and look for classes implementing the IWikiPlugin interface.
		/// </summary>
		/// <param name="pDllPath"></param>
		private void CheckDll(string pDllPath,CultureInfo pCulture,LogDelegate pLogDelegate)
		{
			Assembly ass = Assembly.LoadFrom(pDllPath);
			Debug.WriteLine("assembly loaded");
			foreach(Type t in ass.GetTypes())
			{
				Debug.WriteLine("type=" + t.Name);
				foreach(Type i in t.GetInterfaces())
				{
					Type pi = typeof(IWikiPlugin);
					if(i.Equals(pi))
					{
						// Plugin implements the IWikiPlugin interface
						AddPlugin(t,pDllPath,pCulture,pLogDelegate);
						break; 
					}
				}
			}
		}



		/// <summary>
		/// Adds found plugin to the list of managed plugins
		/// </summary>
		/// <param name="pType">Plugin type</param>
		/// <param name="pFullPath">Plugin full path</param>
		/// <param name="pCulture">Culture to be used by plugins</param>
		private void AddPlugin(Type pType,string pFullPath,CultureInfo pCulture,LogDelegate pLogDelegate)
		{
			IWikiPlugin plugin = (IWikiPlugin)Activator.CreateInstance(pType);
			plugin.Init(pFullPath,pCulture,pLogDelegate);
			_Plugins.Add(plugin);
			foreach (KeywordProperties kp in plugin.GetKeywordsProperties())
			{
				_KeywordsProperties.Add(kp);
			}
		}

		public IWikiPlugin[] GetPlugins()
		{
			return (IWikiPlugin[])_Plugins.ToArray(typeof(IWikiPlugin));
		}

		private ArrayList _Plugins = new ArrayList();
		private ArrayList _KeywordsProperties = new ArrayList();

		public KeywordProperties[] GetKeywordsProperties()
		{ 
			return (KeywordProperties[])_KeywordsProperties.ToArray(typeof(KeywordProperties));
		}

		public int Count { get { return _Plugins.Count; } }

		public IWikiPlugin GetPlugin(int i)
		{
			return (IWikiPlugin)_Plugins[i];
		}

	}

}
