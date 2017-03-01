[CmdletBinding()]
param (
    [ValidateSet("debug", "release")]
    [Alias('c')]
    [string]$Configuration,
    [ValidateSet("release","rtm", "rc", "rc1", "rc2", "rc3", "rc4", "beta", "beta1", "beta2", "final", "xprivate", "zlocal")]
    [Alias('l')]
    [string]$ReleaseLabel = 'zlocal',
    [Alias('n')]
    [int]$BuildNumber,
    [Alias('mspfx')]
    [string]$MSPFXPath,
    [Alias('nugetpfx')]
    [string]$NuGetPFXPath,
    [Alias('s14')]
    [switch]$SkipVS14,
    [Alias('s15')]
    [switch]$SkipVS15,
    [Alias('f')]
    [switch]$Fast,
    [switch]$CI
)

. "$PSScriptRoot\build\common.ps1"

Invoke-BuildStep 'Publishing NuGet.Clients packages - VS14 Toolset' {
        Publish-ClientsPackages $Configuration $ReleaseLabel $BuildNumber -ToolsetVersion 14 -KeyFile $MSPFXPath -CI:$CI
    } `
    -skip:($Fast -or $SkipVS14) `
    -ev +BuildErrors

Invoke-BuildStep 'Publishing the VS14 EndToEnd test package' {
        param($Configuration)
        $EndToEndScript = Join-Path $PSScriptRoot scripts\cibuild\CreateEndToEndTestPackage.ps1 -Resolve
        $OutDir = Join-Path $Artifacts VS14
        & $EndToEndScript -c $Configuration -tv 14 -out $OutDir
    } `
    -args $Configuration `
    -skip:($Fast -or $SkipVS14) `
    -ev +BuildErrors

Invoke-BuildStep 'Publishing the VS15 EndToEnd test package' {
        param($Configuration)
        $EndToEndScript = Join-Path $PSScriptRoot scripts\cibuild\CreateEndToEndTestPackage.ps1 -Resolve
        $OutDir = Join-Path $Artifacts VS15
        & $EndToEndScript -c $Configuration -tv 15 -out $OutDir
    } `
    -args $Configuration `
    -skip:($Fast -or $SkipVS15) `
    -ev +BuildErrors

Trace-Log ('-' * 60)