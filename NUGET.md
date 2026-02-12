# NuGet Package Build Instructions

## Prerequisites
- NuGet CLI installed and available in PATH
- Solution built in Release configuration

## Build Package
```bash
# Build the solution in Release mode
msbuild VerifonePayment.sln /p:Configuration=Release

# Create the NuGet package
nuget pack VerifonePayment.Lib.nuspec

# Alternative: Pack with specific output directory
nuget pack VerifonePayment.Lib.nuspec -OutputDirectory packages

# Publish to NuGet (replace YOUR_API_KEY with actual key)
# nuget push VerifonePayment.Lib.1.2.0.nupkg -ApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
```

## Package Contents
The package includes:
- VerifonePayment.Lib.dll (Main library)
- VerifonePayment.Lib.pdb (Debug symbols)
- Source code for reference
- README.md documentation
- LICENSE file

## Installation
```bash
# Install via Package Manager Console
Install-Package VerifonePayment.Lib

# Install via .NET CLI
dotnet add package VerifonePayment.Lib

# Install via PackageReference (in .csproj)
<PackageReference Include="VerifonePayment.Lib" Version="1.2.0" />
```