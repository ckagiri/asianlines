using System.Collections;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.Common
{
    /// <summary>
    /// Prevents double enumeration (and potential roundtrip to the data source) when checking 
    /// for the presence of items in an enumeration.
    /// </summary>
    internal static class CacheAnyEnumerableExtensions
    {
        /// <summary>
        /// Makes sure that calls to <see cref="IAnyEnumerable{T}.Any()"/> are 
        /// cached, and reuses the resulting enumerator.
        /// </summary>
        public static IAnyEnumerable<T> AsCachedAnyEnumerable<T>(this IEnumerable<T> source)
        {
            return new AnyEnumerable<T>(source);
        }

        /// <summary>
        /// Exposes a cached <see cref="Any"/> operator.
        /// </summary>
        public interface IAnyEnumerable<out T> : IEnumerable<T>
        {
            bool Any();
        }

        /// <summary>
        /// Lazily computes whether the inner enumerable has 
        /// any values, and caches the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
        private class AnyEnumerable<T> : IAnyEnumerable<T>
        {
            private readonly IEnumerable<T> _enumerable;
            private IEnumerator<T> _enumerator;
            private bool _hasAny;

            public AnyEnumerable(IEnumerable<T> enumerable)
            {
                _enumerable = enumerable;
            }

            public bool Any()
            {
                InitializeEnumerator();

                return _hasAny;
            }

            public IEnumerator<T> GetEnumerator()
            {
                InitializeEnumerator();

                return _enumerator;
            }

            private void InitializeEnumerator()
            {
                if (_enumerator == null)
                {
                    var inner = _enumerable.GetEnumerator();
                    _hasAny = inner.MoveNext();
                    _enumerator = new SkipFirstEnumerator(inner, _hasAny);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class SkipFirstEnumerator : IEnumerator<T>
            {
                private readonly IEnumerator<T> _inner;
                private readonly bool _hasNext;
                private bool _isFirst = true;

                public SkipFirstEnumerator(IEnumerator<T> inner, bool hasNext)
                {
                    _inner = inner;
                    _hasNext = hasNext;
                }

                public T Current { get { return _inner.Current; } }

                public void Dispose()
                {
                    _inner.Dispose();
                }

                object IEnumerator.Current { get { return Current; } }

                public bool MoveNext()
                {
                    if (_isFirst)
                    {
                        _isFirst = false;
                        return _hasNext;
                    }

                    return _inner.MoveNext();
                }

                public void Reset()
                {
                    _inner.Reset();
                }
            }
        }
    }
}
