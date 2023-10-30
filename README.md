# anixe_qualitytools

This is a set of helpers that can be useful in test projects.

Package contains:
- [AssertHelper](#asserthelper)
- [TestExamplePath](#testexamplepath)
- [TestExample](#testexample)
- [BenchmarkRunner](#benchmarkrunner)
- [GraylogExporter](#graylogexporter)
- [TmpFile](#tmpfile)

## AssertHelper

Contains methods to compare XML files, JSON files and collections.

## TestExamplePath

The class provide path to test project root directory regardless test framework and test executor (Visual Studio test explorer, vscode, dotnet cli)

## TestExample

Implementes the convention of loading test examples based on namespace of test class and test method.

## BenchmarkRunner

Console menu switcher of all benchmark methods in project

Example usage:

```cs
var runnerConfig = BenchmarkRunnerConfig { DisplaySubmenuOfMethodsInClass = true };
new BenchmarkRunner("My Application Tests", runnerConfig).Run(args, config);
```

## GraylogExporter

Custom BenchmarkDotNet Exporter that can be defined with IConfig. Sends result as gelf message via UDP protocol

Example usage:

```cs
IConfig config = ManualConfig.Create(DefaultConfig.Instance)
                   .With(new GrayLogExporter("Anixe.IO", "graylog-gelf.xxx.com", 5558))
```

## TmpFile

The purpose of this class is to have a file which has unique filename and is IDisposable so it can be used in the unit test as a resource with a lifecycle similar to IDisposable object

```cs
var path = string.Empty;
using (var file = TmpFile.Create(".", ".csv"))
{
  path = file.FullPath;
  Assert.Equal(".csv", Path.GetExtension(path));
  Assert.True(File.Exists(path));
  File.WriteAllText(file.FullPath, "test");
  var content = File.ReadAllText(file.FullPath);
  Assert.Equal("test", content);
}
Assert.False(File.Exists(path));
```