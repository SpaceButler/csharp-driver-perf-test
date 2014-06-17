if exist csharp-driver rmdir /S /Q csharp-driver
git clone https://github.com/datastax/csharp-driver.git --branch master --single-branch
msbuild /v:m /property:Configuration=Release csharp-driver\src\Cassandra.sln
msbuild /v:m /property:Configuration=Release PerformanceTest.sln
PerformanceTest\bin\Release\PerformanceTest.exe