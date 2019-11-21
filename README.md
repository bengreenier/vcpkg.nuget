# vcpkg.nuget

> 🚨🚨 THIS PROJECT IS NOT MAINTAINED ANYMORE, DO NOT USE IT 🚨🚨

[vcpkg](https://github.com/microsoft/vcpkg) wrapped in [nuget](https://nuget.org)-y goodness 📦

![build status](https://b3ngr33ni3r.visualstudio.com/_apis/public/build/definitions/947d98de-244b-4cdb-a49a-4b232d942edc/3/badge)

![example gif](.github/example.gif)

I needed a project local (not system wide) way to easily consume [vcpkg](https://github.com/microsoft/vcpkg) that worked with no dependencies. This project aims to be that solution, distributed via [nuget](https://www.nuget.org/packages/Vcpkg.Nuget/).

__Note: This only supports vcpkg running as part of msbuild, and therefore requires windows.__

## How to use

> See [vcpkg.nuget-example](https://github.com/bengreenier/vcpkg.nuget-example) for an example! :sparkles:

This package relies on `VcpkgPackage` msbuild tasks, that leverage the following format:

```xml
<VcpkgPackage Include="packageName"/>
```

Where `packageName` refers to the [vcpkg port](https://github.com/Microsoft/vcpkg/tree/master/ports) you wish to install.

By default the task invoking vcpkg has a generous default timeout of ten minutes. If your package builds run longer than that, override the `VcpkgTaskTimeout` property with a higher value:

```xml
  <PropertyGroup>
    <!-- task may run for up to twenty minutes -->
    <VcpkgTaskTimeout>1200000</VcpkgTaskTimeout>
  </PropertyGroup>
```

### Using Visual Studio

* Install the [nuget package](https://www.nuget.org/packages/Vcpkg.Nuget/)
* Find the relevant ports you wish to install [here](https://github.com/Microsoft/vcpkg/tree/master/ports)
* Add `VcpkgPackage` elements to your project file
* Building will ensure ports are built and installed

### Using the shell

* Install the [nuget package](https://www.nuget.org/packages/Vcpkg.Nuget/)
* Import our build configuration in your project file:

```xml
<Import Project="packages\Vcpkg.Nuget.1.0.0-beta\build\Vcpkg.Nuget.props" Condition="Exists('packages\Vcpkg.Nuget.1.0.0-beta\build\Vcpkg.Nuget.props')" />
```

* Find the relevant ports you wish to install [here](https://github.com/Microsoft/vcpkg/tree/master/ports)
* Add `VcpkgPackage` elements to your project file
* Building will ensure ports are built and installed

### Overriding Triplets

> Note: This approach is documented [in the vcpkg docs](https://github.com/Microsoft/vcpkg/blob/master/docs/users/integration.md#with-msbuild) as well. We're duplicating that info here as it's common scenario for folks consuming this library.

To override the automatically chosen [triplet](https://github.com/Microsoft/vcpkg/blob/master/docs/users/triplets.md), you can specify the MSBuild property `VcpkgTriplet` in your `.vcxproj`. We recommend adding this to the `Globals` PropertyGroup.
```xml
<PropertyGroup Label="Globals">
  <!-- .... -->
  <VcpkgTriplet Condition="'$(Platform)'=='Win32'">x86-windows-static</VcpkgTriplet>
  <VcpkgTriplet Condition="'$(Platform)'=='x64'">x64-windows-static</VcpkgTriplet>
</PropertyGroup>
```

## Contributing

CI Builds and Release managements takes place in [a private VSTS instance](https://b3ngr33ni3r.visualstudio.com/vcpkg.nuget). If you feel you need access (if you're a core contributor) please [open an issue](https://github.com/bengreenier/vcpkg.nuget/issues/new) against [@bengreenier](https://github.com/bengreenier).

* Be nice and open to feedback.

Unless this project gets more traction, this is the only requirement to contribute.

## License

MIT
