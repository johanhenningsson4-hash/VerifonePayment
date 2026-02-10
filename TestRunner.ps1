# PowerShell script to run MSTest tests
Write-Host "Building the test project..."
msbuild VerifonePayment.Test\VerifonePayment.Test.csproj /p:Configuration=Debug

Write-Host "Searching for test assemblies..."
$testAssembly = Get-ChildItem -Path "VerifonePayment.Test\bin\Debug" -Filter "*.dll" | Where-Object { $_.Name -like "*Test*" }

if ($testAssembly) {
    Write-Host "Found test assembly: $($testAssembly.FullName)"
    Write-Host "Running tests with VSTest..."
    
    # Try with specific test adapter path
    & vstest.console.exe $testAssembly.FullName /TestAdapterPath:"packages\MSTest.TestAdapter.3.1.1\build\_common" /logger:console
} else {
    Write-Host "No test assembly found. The project might be configured as an executable."
    Write-Host "Trying to run the executable to see if it contains test functionality..."
    
    $exeFile = "VerifonePayment.Test\bin\Debug\VerifonePayment.Test.exe"
    if (Test-Path $exeFile) {
        Write-Host "Found executable: $exeFile"
        Write-Host "Note: This appears to be a console application rather than a test library."
        Write-Host "For proper testing, the project should be configured as a library with OutputType=Library"
    }
}