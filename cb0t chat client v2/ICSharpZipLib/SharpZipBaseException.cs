using System;

#if !COMPACT_FRAMEWORK_V10 && !COMPACT_FRAMEWORK_V20
using System.Runtime.Serialization;
#endif

namespace ICSharpCode.SharpZipLib
{
	
#if !COMPACT_FRAMEWORK_V10 && !COMPACT_FRAMEWORK_V20
	[Serializable]
#endif
	public class SharpZipBaseException : ApplicationException
	{
#if !COMPACT_FRAMEWORK_V10 && !COMPACT_FRAMEWORK_V20
		protected SharpZipBaseException(SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
#endif
		
		public SharpZipBaseException()
		{
		}
		
		public SharpZipBaseException(string msg)
			: base(msg)
		{
		}

		public SharpZipBaseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
