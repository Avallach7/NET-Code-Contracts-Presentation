using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;

namespace System.Diagnostics.Contracts
{
	[CompilerGenerated]
	internal static class __ContractsRuntime
	{
		[Serializable]
		private sealed class ContractException : Exception
		{
			[Serializable]
			private struct ContractExceptionData : ISafeSerializationData
			{
				public ContractFailureKind _Kind;

				public string _UserMessage;

				public string _Condition;

				void ISafeSerializationData.CompleteDeserialization(object obj)
				{
					__ContractsRuntime.ContractException ex = obj as __ContractsRuntime.ContractException;
					ex.m_data = this;
				}
			}

			[NonSerialized]
			private __ContractsRuntime.ContractException.ContractExceptionData m_data = default(__ContractsRuntime.ContractException.ContractExceptionData);

			public ContractFailureKind Kind
			{
				get
				{
					return this.m_data._Kind;
				}
			}

			public string Failure
			{
				get
				{
					return this.Message;
				}
			}

			public string UserMessage
			{
				get
				{
					return this.m_data._UserMessage;
				}
			}

			public string Condition
			{
				get
				{
					return this.m_data._Condition;
				}
			}

			public ContractException(ContractFailureKind kind, string failure, string userMessage, string condition, Exception innerException) : base(failure, innerException)
			{
				this.m_data._Kind = kind;
				this.m_data._UserMessage = userMessage;
				this.m_data._Condition = condition;
				base.SerializeObjectState += delegate(object exception, SafeSerializationEventArgs eventArgs)
				{
					eventArgs.AddSerializedState(this.m_data);
				};
			}
		}

		[ThreadStatic]
		internal static int insideContractEvaluation;

		internal static void ReportFailure(ContractFailureKind kind, string msg, string conditionTxt, Exception inner)
		{
			string text = ContractHelper.RaiseContractFailedEvent(kind, msg, conditionTxt, inner);
			if (text != null)
			{
				__ContractsRuntime.TriggerFailure(kind, text, msg, conditionTxt, inner);
			}
		}

		internal static void TriggerFailure(ContractFailureKind kind, string msg, string userMessage, string conditionTxt, Exception inner)
		{
			throw new __ContractsRuntime.ContractException(kind, msg, userMessage, conditionTxt, inner);
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void Requires(bool condition, string msg, string conditionTxt)
		{
			if (!condition)
			{
				__ContractsRuntime.ReportFailure(ContractFailureKind.Precondition, msg, conditionTxt, null);
			}
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void Ensures(bool condition, string msg, string conditionTxt)
		{
			if (!condition)
			{
				__ContractsRuntime.ReportFailure(ContractFailureKind.Postcondition, msg, conditionTxt, null);
			}
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void Invariant(bool condition, string msg, string conditionTxt)
		{
			if (!condition)
			{
				__ContractsRuntime.ReportFailure(ContractFailureKind.Invariant, msg, conditionTxt, null);
			}
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void Assume(bool condition, string msg, string conditionTxt)
		{
			if (!condition)
			{
				__ContractsRuntime.ReportFailure(ContractFailureKind.Assume, msg, conditionTxt, null);
			}
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void Assert(bool condition, string msg, string conditionTxt)
		{
			if (!condition)
			{
				__ContractsRuntime.ReportFailure(ContractFailureKind.Assert, msg, conditionTxt, null);
			}
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void Requires<TException>(bool condition, string msg, string conditionTxt) where TException : Exception
		{
			if (condition)
			{
				return;
			}
			string text = ContractHelper.RaiseContractFailedEvent(ContractFailureKind.Precondition, msg, conditionTxt, null);
			Exception ex = null;
			ConstructorInfo constructor = typeof(TException).GetConstructor(new Type[]
			{
				typeof(string),
				typeof(string)
			});
			if (constructor != null)
			{
				if (constructor.GetParameters()[0].Name == "paramName")
				{
					ex = (constructor.Invoke(new object[]
					{
						msg,
						text
					}) as Exception);
				}
				else
				{
					ex = (constructor.Invoke(new object[]
					{
						text,
						msg
					}) as Exception);
				}
			}
			else
			{
				constructor = typeof(TException).GetConstructor(new Type[]
				{
					typeof(string)
				});
				if (constructor != null)
				{
					ex = (constructor.Invoke(new object[]
					{
						text
					}) as Exception);
				}
			}
			if (ex == null)
			{
				throw new ArgumentException(text, msg);
			}
			throw ex;
		}

		[DebuggerNonUserCode, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		internal static void EnsuresOnThrow(bool condition, string msg, string conditionTxt, Exception originalException)
		{
			if (!condition)
			{
				__ContractsRuntime.ReportFailure(ContractFailureKind.PostconditionOnException, msg, conditionTxt, originalException);
			}
		}
	}
}
