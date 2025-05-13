using System.Security.Cryptography;
using System.Text;

namespace waterb.app;

public sealed class MD5HashFunction<TProcessableType> : IHashFunction<TProcessableType>
{
    public string HashName => "md5";
    public uint Process(TProcessableType value)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(value?.ToString() ?? string.Empty));
        return BitConverter.ToUInt32(hash, 0);
    }
}