using System;
using System.Text;

namespace HyperPackageManager.PackageSerializer;

/*
Structure Of HyperPackage V1 ("Others" part)

(Global)
- Magic Number: "hpkg" => 4 bytes
- Version: int

(This version)
- Manifest Length: int
- Manifest: byte[]
  Each:
  - NameLength: int
  - Name: string
  - Location: int (Count start from the first byte of Data)
  - Length: int
- Data: byte[] (Match with the manifest)
  Each:
  - FileData: byte[]
*/

/// <summary>
/// Serializer for version 1 of the package format.
/// </summary>
public class PackageSerializerV1 : IPackageSerializer
{
    /// <summary>
    /// Checks if the specified version is supported.
    /// </summary>
    /// <param name="version">Version to check</param>
    /// <returns>If the version is supported, return true, otherwise false</returns>
    public bool IsSupported(int version) => version == 1;

    #region Serialize
    public byte[] Serialize(List<PackageItem> items)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            // Form the manifest
            List<(string name, int location, int length)> manifest = new List<(string name, int location, int length)>();
            List<byte> data = new List<byte>();
            foreach (var item in items)
            {
                manifest.Add((item.name, data.Count(), item.data.Length));
                data.AddRange(item.data);
            }

            // Write the manifest
            foreach (var (name, location, length) in manifest)
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                memoryStream.Write(BitConverter.GetBytes(nameBytes.Length), 0, 4);
                memoryStream.Write(nameBytes, 0, nameBytes.Length);
                memoryStream.Write(BitConverter.GetBytes(location), 0, 4);
                memoryStream.Write(BitConverter.GetBytes(length), 0, 4);
            }

            // Write the data
            memoryStream.Write(data.ToArray(), 0, data.Count());

            return memoryStream.ToArray();
        }
    }
    #endregion

    #region Deserialize
    public List<PackageItem> Deserialize(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            // Read the manifest length
            byte[] manifestLengthBytes = new byte[4];
            memoryStream.Read(manifestLengthBytes, 0, 4);
            int manifestLength = BitConverter.ToInt32(manifestLengthBytes, 0);

            // Read the manifest
            byte[] manifest = new byte[manifestLength];
            memoryStream.Read(manifest, 0, manifestLength);

            // Deserialize the manifest
            var manifestItems = DeserializeManifest(manifest);

            // Read the data
            byte[] dataBytes = new byte[memoryStream.Length - memoryStream.Position];
            memoryStream.Read(dataBytes, 0, dataBytes.Length);

            // Deserialize the data
            var dataItems = DeserializeData(dataBytes, manifestItems);

            List<PackageItem> packageItems = new List<PackageItem>();
            for (int i = 0; i < manifestItems.Count; i++)
            {
                var (name, location, length) = manifestItems[i];
                var itemData = dataItems[i];
                packageItems.Add(new PackageItem(name, itemData));
            }

            return packageItems;
        }
    }

    List<(string name, int location, int length)> DeserializeManifest(byte[] manifest)
    {
        List<(string name, int location, int length)> items = new List<(string name, int location, int length)>();

        using (MemoryStream memoryStream = new MemoryStream(manifest))
        {
            // Read to the end
            while (memoryStream.Position < memoryStream.Length)
            {
                // Read the name length
                byte[] nameLengthBytes = new byte[4];
                memoryStream.Read(nameLengthBytes, 0, 4);
                int nameLength = BitConverter.ToInt32(nameLengthBytes, 0);

                // Read the name
                byte[] nameBytes = new byte[nameLength];
                memoryStream.Read(nameBytes, 0, nameLength);
                string name = Encoding.UTF8.GetString(nameBytes);

                // Read the location
                byte[] locationBytes = new byte[4];
                memoryStream.Read(locationBytes, 0, 4);
                int location = BitConverter.ToInt32(locationBytes, 0);

                // Read the length
                byte[] lengthBytes = new byte[4];
                memoryStream.Read(lengthBytes, 0, 4);
                int length = BitConverter.ToInt32(lengthBytes, 0);

                items.Add((name, location, length));
            }
        }

        return items;
    }

    List<byte[]> DeserializeData(byte[] data, List<(string name, int location, int length)> manifestItems)
    {
        List<byte[]> dataItems = new List<byte[]>();

        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            foreach (var item in manifestItems)
            {
                byte[] fileData = new byte[item.length];
                memoryStream.Read(fileData, 0, item.length);
                dataItems.Add(fileData);
            }
        }

        return dataItems;
    }
    #endregion
}
