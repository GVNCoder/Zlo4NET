using System;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines game pipe event args
    /// </summary>
    public class ZGamePipeArgs : EventArgs
    {
        /// <summary>
        /// Gets full pipe message
        /// </summary>
        public string FullMessage { get; }
        /// <summary>
        /// Gets only first part of pipe message
        /// </summary>
        public string FirstPart { get; }
        /// <summary>
        /// Gets only second part of pipe message
        /// </summary>
        public string SecondPart { get; }

        public ZGamePipeArgs(string firstPart, string secondPart)
        {
            FullMessage = $"{firstPart} {secondPart}";
            FirstPart = firstPart;
            SecondPart = secondPart;
        }
    }
}