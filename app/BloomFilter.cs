using System.Collections;

namespace waterb.app;

public sealed class BloomFilter<T>
{
    private readonly BitArray _bits;
    private readonly Func<T, uint>[] _hashes;

    public BloomFilter(int size, params Func<T, uint>[] hashFunctions)
    {
        _bits = new BitArray(size);
        _hashes = hashFunctions;
    }

    public void Add(T item)
    {
        for (var i = 0; i < _hashes.Length; i++)
        {
            var pos = (int)(_hashes[i](item) % (uint)_bits.Length);
            _bits.Set(pos, true);
        }
    }

    public bool Contains(T item)
    {
        for (var i = 0; i < _hashes.Length; i++)
        {
            var pos = (int)(_hashes[i](item) % (uint)_bits.Length);
            if (!_bits.Get(pos)) return false;
        }
        
        return true;
    }
}