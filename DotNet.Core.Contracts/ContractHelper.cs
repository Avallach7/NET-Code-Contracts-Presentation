using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace DotNet.Core.Contracts
{
	public static class ContractHelper
	{
		public static void TriggerFailure(Exception exception)
		{
			if (Debugger.IsAttached)
				Debug.Fail(exception.Message);
			else
				throw exception;
		}

		public static void TriggerFailure<TException>(Exception inner) 
			where TException : Exception, new()
		{
			throw PrepareCustomException<TException>(inner);
		}

		private static TException PrepareCustomException<TException>(Exception inner) 
			where TException : Exception, new()
		{
			var typeInfo = typeof(TException).GetTypeInfo();
			TException exception;
			
			var wrapperExceptionConstructor = typeInfo.GetConstructor(
				new Type[] { typeof(Exception) });
			if (wrapperExceptionConstructor != null)
				exception = (TException) wrapperExceptionConstructor.Invoke(new[]{ inner });
			else
			{
				var describedExceptionConstructor = typeInfo.GetConstructor(
					new Type[] { typeof(string) });
				if (describedExceptionConstructor != null)
					exception = (TException) describedExceptionConstructor.Invoke(new[]{ inner.Message });
				else
					exception = new TException();
			}
			return exception;
		}
	}
}
