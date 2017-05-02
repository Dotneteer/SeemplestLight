using System;

namespace SeemplestLight.Core.Portable.Diagnostics
{
    /// <summary>
    /// This interface defines the operation data a trace operation should have
    /// </summary>
    public interface IOperationData
    {
        /// <summary>
        /// Timestamp information of the operation
        /// </summary>
        DateTime? Timestamp { get; }

        /// <summary>
        /// Optional session ID of the operation
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// Optional business transaction ID of the operation
        /// </summary>
        string BusinessTransactionId { get; }

        /// <summary>
        /// Gets the ID of the operation instance
        /// </summary>
        string OperationInstanceId { get; }

        /// <summary>
        /// Gets the type of the operation
        /// </summary>
        string OperationType { get; }
    }
}