# SQL Formatter performance benchmarks

These benchmarks measure the public `SqlFormatter.Format` path, including tokenization, formatting, and result-string creation. `MemoryDiagnoser` reports allocated bytes and GC activity alongside elapsed time.

## Run

From the repository root, run:

```bash
dotnet run -c Release --project SQL.Formatter.Benchmarks -- --filter '*SqlFormatter*'
```

BenchmarkDotNet writes detailed results under `BenchmarkDotNet.Artifacts/results`. The run includes:

- `SqlFormatterBenchmark`: the original representative join/query case, repeated 1, 10, and 100 times so its measurements can be compared with the existing baseline.
- `SqlFormatterSizeBenchmark`: one call on SQL inputs near 64, 512, 4,096, and 16,384 characters to show how time and allocation scale with query size.

For comparable before/after numbers, use the same .NET runtime, build configuration, machine, and BenchmarkDotNet version. Avoid running another CPU-heavy job at the same time.

## Results from this workspace

The measurements below use BenchmarkDotNet 0.15.8 on .NET 10.0.15 and an AMD Ryzen 5 7500F. The representative-query baseline came from the benchmark output already present before the changes; the current figures were measured after them.

| Workload | Before | After |
| --- | ---: | ---: |
| Representative query, one call | 409.6 µs / 482.5 KB | 127.6 µs / 57.79 KB |
| SQL near 64 characters | — | 53.93 µs / 39.74 KB |
| SQL near 512 characters | — | 236.72 µs / 83.34 KB |
| SQL near 4,096 characters | — | 1,627.62 µs / 436.56 KB |
| SQL near 16,384 characters | — | 6,672.71 µs / 1,728.51 KB |

On the representative query, the measured call was about 69% faster and allocated about 88% less memory. The size-specific cases establish the current scaling profile; no pre-change size sweep was available for those inputs.
