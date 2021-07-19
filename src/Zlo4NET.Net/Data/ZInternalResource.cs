using System.IO;
using System.Reflection;

namespace Zlo4NET.Data
{
    /// <summary>
    /// The static service. Helps get the embedded resource stream
    /// </summary>
    public static class ZInternalResource
    {
        /// <summary>
        /// Gets the internal resource <see cref="Stream"/> instance. Using template "Zlo4NET.Resources.internalPath"
        /// </summary>
        /// <param name="internalPath">The internal path to resource</param>
        /// <returns>Returns resource stream</returns>
        public static Stream GetResourceStream(string internalPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var result = assembly.GetManifestResourceStream($"Zlo4NET.Resources.{internalPath}");

            return result;
        }
    }
}