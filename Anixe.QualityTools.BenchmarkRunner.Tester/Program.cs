var runnerConfig = new Anixe.QualityTools.Benchmark.BenchmarkRunnerConfig { DisplaySubmenuOfMethodsInClass = true };
var runner = new Anixe.QualityTools.Benchmark.BenchmarkRunner("This is a testing app", runnerConfig);
runner.Run(args);