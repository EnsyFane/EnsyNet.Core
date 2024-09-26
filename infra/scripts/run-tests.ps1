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

docker compose -f "$composeFile" up --build --detach "$dbContainer"
ThrowOnError "Failed to start the database container"

New-Item -ItemType Directory -Force -Path ./test-results

dotnet add ./src/Core/Tests/EnsyNet.DataAccess.EntityFramework.Tests package JetBrains.dotCover.CommandLineTools.linux-x64 --version 2023.3.3 --package-directory ./nuget

dotnet sonarscanner begin -o:"stefan-tataran" -k:"EnsyFane_EnsyNet.Core" -d:sonar.host.url="https://sonarcloud.io" -d:sonar.token="$env:SONAR_TOKEN" -d:sonar.cs.dotcover.reportsPaths="./code-coverage/integration-tests/coverage.html" -d:sonar.coverage.exclusions="**/Tests/**,**/Sample/**,**/code-coverage/**" -d:sonar.exclusions="**/.vs/**,**/*.sln"
ThrowOnError "Failed to start Sonar Scanner session"

dotnet build ./src/Core/Tests/EnsyNet.DataAccess.EntityFramework.Tests/EnsyNet.DataAccess.EntityFramework.Tests.csproj
ThrowOnError "Failed to build"

./nuget/jetbrains.dotcover.commandlinetools.linux-x64/2023.3.3/tools/dotCover.sh dotnet --Filters="-:module=Humanizer;-:module=vstest.console" --Output="./test-results/coverage.dcvr" -- test ./src/Core/EnsyNet.sln --no-restore --no-build
ThrowOnError "Failed to run tests"

./nuget/jetbrains.dotcover.commandlinetools.linux-x64/2023.3.3/tools/dotCover.sh report --Source="./test-results/coverage.dcvr" --Output="./code-coverage/integration-tests/coverage.html" --ReportType="HTML" --SourcesSearchPaths="./src/Core/EnsyNet." --ExcludeFileMasks="./src/Core/Tests/**"
ThrowOnError "Failed to generate code coverage report"

dotnet sonarscanner end -d:sonar.token="$env:SONAR_TOKEN"
ThrowOnError "Failed to end Sonar Scanner session"