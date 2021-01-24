using System;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class ZProcessParameters : ZBaseParameters
    {
        public override ZGame Game { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override ZGameArchitecture? PreferredArchitecture { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        /// <summary>
        /// 
        /// </summary>
        public string ProcessName { get; set; }
    }
}