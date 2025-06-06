using HyperPackageManager;

namespace hpkg;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            HyperPackage package = new HyperPackage(1);

            package.Items.Add(new PackageItem("/home/willow-tree/hpkgtest/111.txt"));
            package.Items.Add(new PackageItem("/home/willow-tree/hpkgtest/222.txt"));

            package.Save("/home/willow-tree/hpkgtest/package.hpkg");

            Console.WriteLine("Package saved successfully.");

            HyperPackage readPackage = new HyperPackage(1);
            readPackage.Load("/home/willow-tree/hpkgtest/package.hpkg");

            Console.WriteLine("Package loaded successfully.");

            foreach (var item in readPackage.Items)
            {
                Console.WriteLine($"Saving {item.name}, size {item.data.Length}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
