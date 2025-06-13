using System.IO.Compression;
using System.Text;
using HyperPackageLib.Serializer;

namespace HyperPackageLib;

/*
Structure Of HyperPackage

- Magic Number: "hpkg" => 4 bytes
- Version: int
- Others...(Different for each version, See each PackageSerializer version)

*/

public class HyperPackage
{
    public static List<IPackageSerializer> Serializers { get; set; } = new List<IPackageSerializer>
    {
        new PackageSerializerV1()
    };

    public HyperPackage() { }

    public HyperPackage(int version)
    {
        Version = version;
    }

    public HyperPackage(byte[] data)
    {
        Load(data);
    }

    public HyperPackage(List<PackageItem> items, int version)
    {
        Items = items;
        Version = version;
    }

    public List<PackageItem> Items { get; set; } = new List<PackageItem>();

    public int Version { get; private set; } = 1;

    public void Load(byte[] data)
    {
        // Deserialize the data into PackageItem objects
        // and add them to the Items list.

        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            // Read the magic number
            byte[] magicNumber = new byte[4];
            memoryStream.Read(magicNumber, 0, 4);
            if (Encoding.ASCII.GetString(magicNumber) != "hpkg")
            {
                throw new InvalidDataException("Invalid magic number");
            }

            // Read the version
            byte[] versionBytes = new byte[4];
            memoryStream.Read(versionBytes, 0, 4);
            int version = BitConverter.ToInt32(versionBytes, 0);

            // Read the body
            byte[] body = new byte[memoryStream.Length - memoryStream.Position];
            memoryStream.Read(body, 0, body.Length);

            // Check for a supported serializer
            foreach (var serializer in Serializers)
            {
                // If supported, use the serializer
                if (serializer.IsSupported(version))
                {
                    Items = serializer.Deserialize(body);
                    break;
                }
            }
        }
    }

    public void LoadFromFile(string path) => Load(File.ReadAllBytes(path));

    public byte[] Pack()
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            // Write the magic number
            byte[] magicNumber = Encoding.ASCII.GetBytes("hpkg");
            memoryStream.Write(magicNumber, 0, magicNumber.Length);

            // Write the version
            byte[] versionBytes = BitConverter.GetBytes(Version);
            memoryStream.Write(versionBytes, 0, versionBytes.Length);

            foreach (var serializer in Serializers)
            {
                if (serializer.IsSupported(Version))
                {
                    byte[] serializedData = serializer.Serialize(Items);
                    memoryStream.Write(serializedData, 0, serializedData.Length);
                    break;
                }
            }

            return memoryStream.ToArray();
        }
    }

    public void PackToFile(string path) => File.WriteAllBytes(path, Pack());
}
