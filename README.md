# anixe_qualitytools

This is a set of helpers that can be useful in test projects.

Package contains:
- [AssertHelper](#asserthelper)
- [TestExamplePath](#testexamplepath)
- [TestExample](#testexample)
- [BenchmarkRunner](#benchmarkrunner)
- [GraylogExporter](#graylogexporter)

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
new BenchmarkRunner("My Application Tests").Run(args, config);
```

## GraylogExporter

Custom BenchmarkDotNet Exporter that can be defined with IConfig. Sends result as gelf message via UDP protocol

Example usage:

```cs
IConfig config = ManualConfig.Create(DefaultConfig.Instance)
                   .With(new GrayLogExporter("Anixe.IO", "graylog-gelf.xxx.com", 5558))
```

# Changelog

1.2.3-1.2.4
* Add LoadTestFixture based on reflection for parameterless loading examples

1.2.2
* Add p field to graylog export


1.2.1
* Fix cannot use custom config in BenchmarkRunner in "all" mode

1.2.0
* Added GraylogExporter
* Removed Excel analyse file generating after run BenchmarkRunner
* Bump BenchmarkDotNet version to 0.11.5

1.4.0
* Added excludePaths param to AreJsonObjectsSemanticallyEqual