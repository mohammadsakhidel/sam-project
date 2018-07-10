using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Objects.Utilities
{
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _lambdaComparer;

        public LambdaComparer(Func<T, T, bool> lambdaComparer)
        {
            _lambdaComparer = lambdaComparer;
        }

        public bool Equals(T x, T y)
        {
            return _lambdaComparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<T>.Default.GetHashCode();
        }
    }
}
