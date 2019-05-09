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

## TestExample

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

1.2.0
* Added GraylogExporter
* Removed Excel analyse file generating after run BenchmarkRunner
* Bump BenchmarkDotNet version to 0.11.5