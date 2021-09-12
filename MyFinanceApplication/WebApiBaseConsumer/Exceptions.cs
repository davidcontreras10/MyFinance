using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiBaseConsumer
{
	public class BackendTokenException : Exception
	{
		#region Attributes

		public string ErrorMessage { get; set; }
		public int ErrorCode { get; set; }

		#endregion
	}

	[Serializable]
	internal class InvalidObjectTypeException : Exception
	{
		public InvalidObjectTypeException(Type type) : base($"Type {type.FullName} is invalid for this operation.") { }
	}

	[Serializable]
	internal class ObjectTypeNotSupportedException : Exception
	{
		public ObjectTypeNotSupportedException(Type type) : base($"Type {type.FullName} is not supported for this operation.") { }
	}
}
