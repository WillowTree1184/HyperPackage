# Hyper Package

A simple solution for storing data and files in a unified package format.

## Overview

This project consists of two sub-projects:

1. `hpkg`: A command-line tool for managing Hyper Package files.
2. `HyperPackageLib`: A class library for manipulating Hyper Package data.

> [!TIP]
> A Hyper Package is not a file, but a data string that can be stored in any format.

## Using `HyperPackageLib`

### Creating a Package

```csharp
using HyperPackageLib;

var package = new HyperPackage(1);  // Instantiate a Hyper Package object, version 1.
package.Items.Add(new PackageItem("Example data", BitConverter.GetBytes("This is an example data.")));
package.Items.Add(new PackageItem("Example file", File.ReadAllBytes("./examplefile.txt")));
package.Pack(); // Pack to obtain the Hyper Package data.
```

To save your package directly to a file, use the following approach:

```csharp
package.PackToFile("./hyperpackage.hpkg");
```

### Reading a Package

```csharp
using HyperPackageLib;

var package = new HyperPackage(/* Hyper Package data */);
```

Alternatively, you can use:

```csharp
var package = new HyperPackage(1);
package.Load(/* Hyper Package data */);   // This will override package.Version.
Console.WriteLine(package.Version); // Print the current version.
```

If you want to read from a file, a more convenient method is:

```csharp
var package = new HyperPackage(1);
package.LoadFromFile("./hyperpackage.hpkg");
Console.WriteLine(package.Version);
```

### Managing the Items of a Package

You can add, remove, or access items in a package using the `Items` property, which is a `List<PackageItem>`.

#### Adding an item

```csharp
package.Items.Add(new PackageItem("Example", myByteArray));
```

#### Removing an item

```csharp
package.Items.RemoveAll(item => item.name == "Example");
```

#### Accessing an item

```csharp
var item = package.Items.FirstOrDefault(item => item.name == "Example");
if (item.data != null)
{
    // Use item.data as needed
}
```
