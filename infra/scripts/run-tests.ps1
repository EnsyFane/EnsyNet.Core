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

dotnet tool install --global dotnet-coverage
ThrowOnError "Failed to install dotnet-coverage"

dotnet sonarscanner begin -o:"ensyinc" -k:"EnsyInc_EnsyNet.Core" -d:sonar.host.url="https://sonarcloud.io" -d:sonar.token="$env:SONAR_TOKEN" -d:sonar.cs.vscoveragexml.reportsPaths="./test-results/coverage.xml" -d:sonar.coverage.exclusions="**/Tests/**,**/Sample/**" -d:sonar.exclusions="**/.vs/**,**/*.slnx"
ThrowOnError "Failed to start Sonar Scanner session"

dotnet build ./src/Core/Tests/EnsyNet.DataAccess.EntityFramework.Tests/EnsyNet.DataAccess.EntityFramework.Tests.csproj
ThrowOnError "Failed to build"

dotnet-coverage collect -f xml -o ./test-results/coverage.xml -- dotnet test ./src/Core/EnsyNet.slnx --no-restore --no-build
ThrowOnError "Failed to run tests and collect coverage"

dotnet sonarscanner end -d:sonar.token="$env:SONAR_TOKEN"
ThrowOnError "Failed to end Sonar Scanner session"