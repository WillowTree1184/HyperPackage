namespace HyperPackageLib;

/// <summary>
/// Hyper package item data.
/// </summary>
public struct PackageItem
{
    /// <summary>
    /// Name of the item.
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Data of the item.
    /// </summary>
    public byte[] data { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageItem"/> struct.
    /// </summary>
    /// <param name="name">Name of the item</param>
    /// <param name="data">Data of the item</param>
    public PackageItem(string name, byte[] data)
    {
        this.name = name;
        this.data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageItem"/> struct.
    /// </summary>
    /// <param name="path">Path to the item</param>
    public PackageItem(string path)
    {
        this.name = Path.GetFileName(path);
        this.data = File.ReadAllBytes(path);
    }
}
