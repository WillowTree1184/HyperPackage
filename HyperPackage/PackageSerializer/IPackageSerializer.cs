using HyperPackage.Core;

namespace HyperPackage.PackageSerializer;

/// <summary>
/// Interface for package serialization.
/// </summary>
public interface IPackageSerializer
{
    /// <summary>
    /// Checks if the specified version is supported.
    /// </summary>
    /// <param name="version">Version to check</param>
    /// <returns>If the version is supported, return true</returns>
    public bool IsSupported(int version);

    /// <summary>
    /// Serializes the specified HyperPackage.
    /// </summary>
    /// <param name="items">Items to serialize</param>
    /// <returns>Serialized byte array</returns>
    public byte[] Serialize(List<PackageItem> items);

    /// <summary>
    /// Deserializes the specified byte array into a list of PackageItem objects.
    /// </summary>
    /// <param name="data">Byte array to deserialize</param>
    /// <returns>List of PackageItem objects</returns>
    public List<PackageItem> Deserialize(byte[] data);
}
