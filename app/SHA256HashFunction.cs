using System.Security.Cryptography;
using System.Text;

namespace waterb.app;

public sealed class SHA256HashFunction<TProcessableType> : IHashFunction<TProcessableType>
{
    public string HashName => "sha256";
    public uint Process(TProcessableType value)
    {
        var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(value?.ToString() ?? string.Empty));
        return BitConverter.ToUInt32(hash, 0);
    }
}