using System;
using System.Threading;

using Zlo4NET.Core.Data;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZSynchronizationWrapper
    {
        private static SynchronizationContext _context;

        //public static void Initialize(ZConfiguration config) => _context = config.SynchronizationContext;

        /// <summary>
        /// Execute action async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="state"></param>
        internal static void Post<T>(Action<T> action, T state = default(T))
            => _context.Post(new SendOrPostCallback((s) => action((T) s)), state);
        /// <summary>
        /// Execute action sync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="state"></param>
        internal static void Send<T>(Action<T> action, T state = default(T))
            => _context.Send(new SendOrPostCallback((s) => action((T) s)), state);
        /// <summary>
        /// Execute action sync and return value
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TState"></typeparam>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        internal static TResult SendReturn<TResult, TState>(Func<TState, TResult> action, TState state = default(TState))
        {
            var __state = new ZActionState<TResult, TState>
            {
                State = state
            };
            
            _context.Send(new SendOrPostCallback((s) =>
            {
                var internalState = (ZActionState<TResult, TState>) s;
                internalState.Result = action(internalState.State);
            }), __state);

            return __state.Result;
        }
    }
}