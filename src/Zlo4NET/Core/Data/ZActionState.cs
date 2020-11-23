namespace Zlo4NET.Core.Data
{
    internal class ZActionState<TResult, TState>
    {
        public TResult Result { get; set; }
        public TState State { get; set; }
    }
}