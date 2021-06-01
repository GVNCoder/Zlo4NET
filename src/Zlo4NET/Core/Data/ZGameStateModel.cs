namespace Zlo4NET.Core.Data
{
    internal class ZGameStateModel
    {
        public ZGameEvent Event { get; set; }
        public ZGameState[] States { get; set; }
        public string RawEvent { get; set; }
        public string RawState { get; set; }
    }
}