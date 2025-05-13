using System.Security.Cryptography;
using System.Text;

namespace waterb.app;

public sealed class SHA512HashFunction<TProcessableType> : IHashFunction<TProcessableType>
{
    public string HashName => "sha512";
    public uint Process(TProcessableType value)
    {
        using var sha512 = SHA512.Create();
        var hash = sha512.ComputeHash(Encoding.ASCII.GetBytes(value?.ToString() ?? string.Empty));
        return BitConverter.ToUInt32(hash, 0);
    }
}