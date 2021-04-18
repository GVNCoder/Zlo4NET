namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// Represents the response status codes
    /// </summary>
    internal enum ZResponseStatusCode
    {
        /// <summary>
        /// Occurs when a request was got a associated response
        /// </summary>
        Ok,
        /// <summary>
        /// Occurs when a request was got timeout
        /// </summary>
        Timeout, // Task.First(requestTask, timeoutTask)
        /// <summary>
        /// Occurs when a request was denied while trying to send it
        /// </summary>
        Declined,
        /// <summary>
        /// Occurs when a request was rejected from processing queue
        /// </summary>
        Rejected
    }
}