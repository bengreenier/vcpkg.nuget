# vcpkg.nuget

[vcpkg](https://github.com/microsoft/vcpkg) wrapped in [nuget](https://nuget.org)-y goodness 📦

![build status](https://b3ngr33ni3r.visualstudio.com/_apis/public/build/definitions/947d98de-244b-4cdb-a49a-4b232d942edc/3/badge)

![example gif](.github/example.gif)

I needed a project local (not system wide) way to easily consume [vcpkg](https://github.com/microsoft/vcpkg) that worked with no dependencies. This project aims to be that solution, distributed via [nuget](https://www.nuget.org/packages/Vcpkg.Nuget/).

__Note: This only supports vcpkg running as part of msbuild, and therefore requires windows.__

## How to use

This package relies on `VcpkgPackage` msbuild tasks, that leverage the following format:

```xml
<VcpkgPackage Include="packageName"/>
```

Where `packageName` refers to the [vcpkg port](https://github.com/Microsoft/vcpkg/tree/master/ports) you wish to install.

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

## Walkthrough

> Coming soon. Upvote [this issue](https://github.com/bengreenier/vcpkg.nuget/issues/3) if you'd like to see it sooner.

## Contributing

CI Builds and Release managements takes place in [a private VSTS instance](https://b3ngr33ni3r.visualstudio.com/vcpkg.nuget). If you feel you need access (if you're a core contributor) please [open an issue](https://github.com/bengreenier/vcpkg.nuget/issues/new) against [@bengreenier](https://github.com/bengreenier).

* Be nice and open to feedback.

Unless this project gets more traction, this is the only requirement to contribute.

## License

MIT