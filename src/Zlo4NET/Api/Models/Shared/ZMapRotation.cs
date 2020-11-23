using System.Collections.ObjectModel;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class ZMapRotation : ZObservableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [ZObservableProperty]
        [ZMapperProperty]
        public ZMap Current { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ZObservableProperty]
        [ZMapperProperty]
        public ZMap Next { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ZMap> Rotation { get; set; }
    }
}