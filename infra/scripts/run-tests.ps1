param (
    [Parameter(Mandatory = $true)]
    [string]$composeFile,
    [Parameter(Mandatory = $true)]
    [string]$dbContainer
)

function ThrowOnError($message) {
    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) {
        throw $message
    }
}

docker-compose -f "$composeFile" up --build --detach "$dbContainer"
ThrowOnError "Failed to start the database container"

New-Item -ItemType Directory -Force -Path ./test-results

dotnet add ./src/Tests/EnsyNet.DataAccess.EntityFramework.Tests package JetBrains.dotCover.CommandLineTools.linux-x64 --version 2023.3.3 --package-directory ./nuget

dotnet build ./src/Tests/EnsyNet.DataAccess.EntityFramework.Tests/EnsyNet.DataAccess.EntityFramework.Tests.csproj

./nuget/jetbrains.dotcover.commandlinetools.linux-x64/2023.3.3/tools/dotCover.sh dotnet --Filters="-:module=Humanizer;-:module=vstest.console" --Output="./test-results/coverage.dcvr" -- test ./src/EnsyNet.sln --no-restore --no-build
ThrowOnError "Failed to run tests"

./nuget/jetbrains.dotcover.commandlinetools.linux-x64/2023.3.3/tools/dotCover.sh report --Source="./test-results/coverage.dcvr" --Output="./test-results/coverage.xml" --ReportType="DetailedXML" --SourcesSearchPaths="./src/EnsyNet." --ExcludeFileMasks="./src/Tests/**"

# dotnet add ./src/Tests/EnsyNet.DataAccess.EntityFramework.Tests package ReportGenerator --version 5.2.2 --package-directory ./nuget

# dotnet ./nuget/reportgenerator/5.2.2/tools/net8.0/ReportGenerator.dll -reports:./test-results/coverage.xml -targetdir:./test-results -reporttypes:MarkdownSummaryGithub
