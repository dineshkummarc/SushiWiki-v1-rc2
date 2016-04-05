using System;
using System.Runtime.Serialization;

namespace Wiki
{
	/// History :
	/// <code>
	/// 
	/// | Vers. | Date       | Developper  | Description
	/// | 0.1   | 12/01/2004 | EGE         | Initial version


	/// <summary>
	/// Wiki Error codes
	/// </summary>
	public enum WikiErrorCodes
	{
		INVALID_PAGE_NAME=1,
		MISSING_PAGE_NAME=2,
		STORAGE_ACCESS_ERROR=3,
		ADMIN_ACCESS_DENIED=4,
		PAGE_ACCESS_DENIED=5,
		ATTACHEMENTS_DENIED=6,
		SQL_STORAGE_REQUIRED=7,
		UNAUTHORIZED_ACCESS=8
	}

	/// <summary>
	/// Generic exeption used by Wiki. Mainly used for database errors.
	/// </summary>
	[Serializable()]
	public class WikiException : ApplicationException
	{
		/// <summary>
		/// Inner exception constructor
		/// </summary>
		/// <param name="message">Exception message</param>
		/// <param name="inner">Inner exeption</param>
		public WikiException (string message, Exception inner)
			: base (message, inner) {}

		/// <summary>
		/// Initial exception constructor.
		/// </summary>
		/// <param name="message">Exception message</param>
		public WikiException(string message)
			: base (message) {}

		public WikiException() : base () {}

		protected WikiException(SerializationInfo si, StreamingContext sc) : base(si,sc) {}
	}
}
