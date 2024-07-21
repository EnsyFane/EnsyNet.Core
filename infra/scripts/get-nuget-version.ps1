param(
    [Parameter(Mandatory=$false)]
    [string]$nugetVersionOverride,
    [Parameter(Mandatory=$true)]
    [string]$isMain,
    [Parameter(Mandatory=$true)]
    [string]$slnDirectory
)

if ($nugetVersionOverride) {
    $versionRegex = '^[0-9]+\.[0-9]+\.[0-9]+(-.{1,15})?$'
    Write-Host "Detected NuGet version override: $nugetVersionOverride"

    if ($nugetVersionOverride -notmatch $versionRegex) {
        Write-Error "NuGet version override '$nugetVersionOverride' is invalid. It needs to match the regex: $versionRegex."
    }

    $version = $nugetVersionOverride
} else {
    $xml = [Xml] (Get-Content $slnDirectory/Directory.Packages.props)
    $baseVersion = $xml.Project.PropertyGroup.Version | Out-String
    $baseVersion = $baseVersion.Trim()
    $date = "$([DateTime]::UtcNow.ToString('yyMMdd').TrimStart('0')).$([DateTime]::UtcNow.ToString('HHmmss').TrimStart('0'))"
    if ($isMain -eq 'true') {
        $version = "$baseVersion-main.$date"
    } else {
        $version = "$baseVersion-dev.$date"
    }
}

Write-Host "New NuGet version: $version"
echo "nuget_version=$version" >> $env:GITHUB_ENV
