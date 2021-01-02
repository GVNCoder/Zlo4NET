using System.Collections.ObjectModel;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public class ZMapRotation : ZObservableObject
    {
        private int[] _rotationIndexes;

        /// <summary>
        /// Creates a new instance of <see cref="ZMapRotation"/> class
        /// </summary>
        /// <param name="rotation"></param>
        public ZMapRotation(int[] rotation)
        {
            _rotationIndexes = rotation;
        }

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