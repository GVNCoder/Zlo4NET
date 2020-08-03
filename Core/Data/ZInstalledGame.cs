using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Data
{
    internal class ZInstalledGame
    {
        public ZGame EnumGame { get; set; }
        public string FriendlyName { get; set; }
        public string ZloName { get; set; }
        public string RunnableName { get; set; }
    }
}