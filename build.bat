REM if exist csharp-driver rmdir /S /Q csharp-driver
REM git clone https://github.com/datastax/csharp-driver.git --branch master --single-branch
if not exist csharp-driver git clone https://github.com/datastax/csharp-driver.git
cd csharp-driver
git pull origin
git checkout master
cd ..
msbuild /v:m /property:Configuration=Release csharp-driver\src\Cassandra.sln
msbuild /v:m /property:Configuration=Release PerformanceTest.sln
PerformanceTest\bin\Release\PerformanceTest.exe