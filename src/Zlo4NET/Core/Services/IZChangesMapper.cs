using System.Collections.Generic;

namespace Zlo4NET.Core.Services
{
    /// <summary>
    /// Defines ZModel change mapper based on ZMapperProperty attribute
    /// </summary>
    internal interface IZChangesMapper
    {
        void MapChanges<T>(T source, T target);
        void MapCollection<T>(IEnumerable<T> source, ICollection<T> target);
    }
}