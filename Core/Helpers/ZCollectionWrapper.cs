using System.Collections.ObjectModel;
using Zlo4NET.Api.Models.Server;

namespace Zlo4NET.Core.Helpers
{
    internal class ZCollectionWrapper
    {
        public ZCollectionWrapper(ObservableCollection<ZServerBase> collection)
        {
            Collection = collection;
        }

        public ObservableCollection<ZServerBase> Collection { get; }

        public int Count => ZSynchronizationWrapper.SendReturn<int, object>((s) => Collection.Count);

        public void Add(ZServerBase item) => ZSynchronizationWrapper.Send<ZServerBase>((s) => Collection.Add(s), item);

        public void Remove(ZServerBase item) =>
            ZSynchronizationWrapper.Send<ZServerBase>((s) => Collection.Remove(s), item);

        public void Flush() => ZSynchronizationWrapper.Send<object>((s) => Collection.Clear());
    }
}