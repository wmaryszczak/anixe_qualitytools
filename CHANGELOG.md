# Anixe.QualityTools CHANGELOG

1.5.2 - 2023-10-30
* Added TmpFile class to have temporary, unique, IDisposable file in the unit test

1.5.0 - 2022-09-29
* Added BenchmarkRunner constructor parameter being config that allows to enable submenu for methods in benchmark class

1.4.0
* Added excludePaths param to AreJsonObjectsSemanticallyEqual

1.3.1 - 2021-11-03
* Updated dependencies

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