# WCF Network Tester

WCF Network Tester is a Windows-oriented client/server utility for measuring WCF request and response performance across three serialization modes:

- Standard XML
- GZip-compressed XML
- ProtoBuf

The server exposes one endpoint for each encoding, and the client runs timed request/response matrix tests against those endpoints to help compare payload size and serialization overhead.

## What the tool does

- Hosts WCF endpoints at `/xml`, `/gzip`, and `/protobuf`
- Tests 16 request/response size combinations per encoding
- Supports four payload sizes: 1 KB, 100 KB, 1 MB, and 20 MB
- Repeats each test case 12 times, with 2 warm-up runs and 10 measured runs
- Can run a single encoding suite or all 48 test combinations in one pass
- Shows connectivity checks and a formatted results table with average, min, max, and standard deviation timings

## Repository layout

| Path | Purpose |
| --- | --- |
| `src/WcfNetworkTester.Server` | Console host for the WCF service endpoints |
| `src/WcfNetworkTester.Client` | Interactive console client that runs the test matrix |
| `src/WcfNetworkTester.Common` | Shared WCF binding helpers, including custom GZip encoding |
| `src/WcfNetworkTester.Contracts` | Service contracts and request/response models |
| `.github/workflows` | CI build and tagged release automation |

## Prerequisites

- .NET SDK 10 or newer to work with the `.slnx` solution format
- Windows with .NET Framework 4.8 installed to run the packaged binaries

## Build the solution

From the repository root:

```powershell
dotnet restore WCFNetworkTester.slnx
dotnet build WCFNetworkTester.slnx
dotnet test WCFNetworkTester.slnx
```

## Run the server

The server binds to `localhost:8080` by default.

```powershell
dotnet run --project src/WcfNetworkTester.Server/WcfNetworkTester.Server.csproj -- --host localhost --port 8080
```

Arguments:

- `--host <name>` or `-h <name>`: override the host name to bind to
- `--port <number>` or `-p <number>`: override the listening port
- `--host=<name>` and `--port=<number>` are also supported
- `-h=<name>` and `-p=<number>` are also supported

Using the example command above, the server exposes:

- `http://localhost:8080/xml`
- `http://localhost:8080/gzip`
- `http://localhost:8080/protobuf`

## Run the client

Start the client after the server is already running:

```powershell
dotnet run --project src/WcfNetworkTester.Client/WcfNetworkTester.Client.csproj -- --host localhost --port 8080
```

Arguments:

- `--host <name>`: target server host, default `localhost`
- `--port <number>` or `-p <number>`: target server port, default `8080`
- `--port=<number>` and `-p=<number>` are also supported

The client performs a connectivity check first, then shows a menu for:

1. Standard XML
2. GZip XML
3. ProtoBuf
4. Run All Tests
5. Exit

Each request/response size combination is executed 12 times. The first 2 runs are warm-up runs, and the remaining 10 runs are used to calculate:

- Average duration
- Minimum duration
- Maximum duration
- Standard deviation

Selecting **Run All Tests** executes all three encodings for a total of 48 combinations.

## Sample output

```bash

  ╔══════════════════════════════════════════╗
  ║       WCF Network Tester — Client        ║
  ╚══════════════════════════════════════════╝
  Server: localhost:8080

  Checking connectivity to server...

    [Standard XML]  http://localhost:8080/xml  ... OK
    [GZip XML    ]  http://localhost:8080/gzip  ... OK
    [ProtoBuf    ]  http://localhost:8080/protobuf  ... OK

  ┌─────────────────────────────┐
  │        Select a Test        │
  ├─────────────────────────────┤
  │  [1] Standard XML           │
  │  [2] GZip XML               │
  │  [3] ProtoBuf               │
  │  [4] Run All Tests          │
  │  [5] Exit                   │
  └─────────────────────────────┘
  Choose option: 4

  Running all tests (48 combinations across 3 encodings × 12 runs, 2 warm-up)...

  ─── Standard XML ───
    Running [Standard XML] 1 KB    → 1 KB    ...  avg       0.6 ms
    Running [Standard XML] 1 KB    → 100 KB  ...  avg       1.8 ms
    Running [Standard XML] 1 KB    → 1 MB    ...  avg       5.5 ms
    Running [Standard XML] 1 KB    → 20 MB   ...  avg     102.5 ms
    Running [Standard XML] 100 KB  → 1 KB    ...  avg       1.6 ms
    Running [Standard XML] 100 KB  → 100 KB  ...  avg       1.5 ms
    Running [Standard XML] 100 KB  → 1 MB    ...  avg      12.3 ms
    Running [Standard XML] 100 KB  → 20 MB   ...  avg     102.6 ms
    Running [Standard XML] 1 MB    → 1 KB    ...  avg       5.2 ms
    Running [Standard XML] 1 MB    → 100 KB  ...  avg       5.4 ms
    Running [Standard XML] 1 MB    → 1 MB    ...  avg      11.7 ms
    Running [Standard XML] 1 MB    → 20 MB   ...  avg     108.9 ms
    Running [Standard XML] 20 MB   → 1 KB    ...  avg      87.5 ms
    Running [Standard XML] 20 MB   → 100 KB  ...  avg      90.9 ms
    Running [Standard XML] 20 MB   → 1 MB    ...  avg      89.3 ms
    Running [Standard XML] 20 MB   → 20 MB   ...  avg     206.6 ms

  ─── GZip XML ───
    Running [GZip XML    ] 1 KB    → 1 KB    ...  avg       0.7 ms
    Running [GZip XML    ] 1 KB    → 100 KB  ...  avg       2.6 ms
    Running [GZip XML    ] 1 KB    → 1 MB    ...  avg      19.7 ms
    Running [GZip XML    ] 1 KB    → 20 MB   ...  avg     364.8 ms
    Running [GZip XML    ] 100 KB  → 1 KB    ...  avg       6.3 ms
    Running [GZip XML    ] 100 KB  → 100 KB  ...  avg       4.3 ms
    Running [GZip XML    ] 100 KB  → 1 MB    ...  avg      21.3 ms
    Running [GZip XML    ] 100 KB  → 20 MB   ...  avg     371.1 ms
    Running [GZip XML    ] 1 MB    → 1 KB    ...  avg      22.2 ms
    Running [GZip XML    ] 1 MB    → 100 KB  ...  avg      19.5 ms
    Running [GZip XML    ] 1 MB    → 1 MB    ...  avg      37.1 ms
    Running [GZip XML    ] 1 MB    → 20 MB   ...  avg     376.3 ms
    Running [GZip XML    ] 20 MB   → 1 KB    ...  avg     403.8 ms
    Running [GZip XML    ] 20 MB   → 100 KB  ...  avg     410.8 ms
    Running [GZip XML    ] 20 MB   → 1 MB    ...  avg     434.3 ms
    Running [GZip XML    ] 20 MB   → 20 MB   ...  avg     812.3 ms

  ─── ProtoBuf ───
    Running [ProtoBuf    ] 1 KB    → 1 KB    ...  avg       0.7 ms
    Running [ProtoBuf    ] 1 KB    → 100 KB  ...  avg       1.1 ms
    Running [ProtoBuf    ] 1 KB    → 1 MB    ...  avg       9.5 ms
    Running [ProtoBuf    ] 1 KB    → 20 MB   ...  avg     165.3 ms
    Running [ProtoBuf    ] 100 KB  → 1 KB    ...  avg       2.2 ms
    Running [ProtoBuf    ] 100 KB  → 100 KB  ...  avg       3.1 ms
    Running [ProtoBuf    ] 100 KB  → 1 MB    ...  avg      11.0 ms
    Running [ProtoBuf    ] 100 KB  → 20 MB   ...  avg     173.6 ms
    Running [ProtoBuf    ] 1 MB    → 1 KB    ...  avg      12.0 ms
    Running [ProtoBuf    ] 1 MB    → 100 KB  ...  avg      12.1 ms
    Running [ProtoBuf    ] 1 MB    → 1 MB    ...  avg      22.8 ms
    Running [ProtoBuf    ] 1 MB    → 20 MB   ...  avg     159.3 ms
    Running [ProtoBuf    ] 20 MB   → 1 KB    ...  avg     126.6 ms
    Running [ProtoBuf    ] 20 MB   → 100 KB  ...  avg     124.9 ms
    Running [ProtoBuf    ] 20 MB   → 1 MB    ...  avg     129.8 ms
    Running [ProtoBuf    ] 20 MB   → 20 MB   ...  avg     273.7 ms


  ╔════════════════╦══════════════╦═══════════════╦══════════════╦══════════════╦══════════════╦══════════════╦════════════════════╗
  ║                                   WCF Network Performance Test Results (12 runs, 2 warm-up)                                    ║
  ╠════════════════╦══════════════╦═══════════════╦══════════════╦══════════════╦══════════════╦══════════════╦════════════════════╣
  ║ Encoding       ║ Req Size     ║ Resp Size     ║      Average ║          Min ║          Max ║      Std Dev ║ Status             ║
  ╠════════════════╦══════════════╦═══════════════╦══════════════╦══════════════╦══════════════╦══════════════╦════════════════════╣
  ║ Standard XML   ║ 1 KB         ║ 1 KB          ║       0.6 ms ║       0.5 ms ║       0.8 ms ║       0.1 ms ║ OK                 ║
  ║ Standard XML   ║ 1 KB         ║ 100 KB        ║       1.8 ms ║       1.0 ms ║       8.2 ms ║       2.3 ms ║ OK                 ║
  ║ Standard XML   ║ 1 KB         ║ 1 MB          ║       5.5 ms ║       5.2 ms ║       6.4 ms ║       0.4 ms ║ OK                 ║
  ║ Standard XML   ║ 1 KB         ║ 20 MB         ║     102.5 ms ║      87.7 ms ║     125.7 ms ║      12.7 ms ║ OK                 ║
  ║ Standard XML   ║ 100 KB       ║ 1 KB          ║       1.6 ms ║       0.9 ms ║       4.7 ms ║       1.2 ms ║ OK                 ║
  ║ Standard XML   ║ 100 KB       ║ 100 KB        ║       1.5 ms ║       1.2 ms ║       2.3 ms ║       0.4 ms ║ OK                 ║
  ║ Standard XML   ║ 100 KB       ║ 1 MB          ║      12.3 ms ║       5.5 ms ║      36.1 ms ║       9.8 ms ║ OK                 ║
  ║ Standard XML   ║ 100 KB       ║ 20 MB         ║     102.6 ms ║      81.6 ms ║     135.1 ms ║      19.6 ms ║ OK                 ║
  ║ Standard XML   ║ 1 MB         ║ 1 KB          ║       5.2 ms ║       4.3 ms ║       7.8 ms ║       1.0 ms ║ OK                 ║
  ║ Standard XML   ║ 1 MB         ║ 100 KB        ║       5.4 ms ║       4.7 ms ║       6.7 ms ║       0.5 ms ║ OK                 ║
  ║ Standard XML   ║ 1 MB         ║ 1 MB          ║      11.7 ms ║       9.1 ms ║      23.8 ms ║       4.9 ms ║ OK                 ║
  ║ Standard XML   ║ 1 MB         ║ 20 MB         ║     108.9 ms ║      85.7 ms ║     148.2 ms ║      19.5 ms ║ OK                 ║
  ║ Standard XML   ║ 20 MB        ║ 1 KB          ║      87.5 ms ║      78.7 ms ║     105.3 ms ║      10.2 ms ║ OK                 ║
  ║ Standard XML   ║ 20 MB        ║ 100 KB        ║      90.9 ms ║      81.3 ms ║     114.8 ms ║      10.0 ms ║ OK                 ║
  ║ Standard XML   ║ 20 MB        ║ 1 MB          ║      89.3 ms ║      82.5 ms ║     113.4 ms ║       8.9 ms ║ OK                 ║
  ║ Standard XML   ║ 20 MB        ║ 20 MB         ║     206.6 ms ║     176.0 ms ║     256.9 ms ║      25.4 ms ║ OK                 ║
  ╠════════════════╬══════════════╬═══════════════╬══════════════╬══════════════╬══════════════╬══════════════╬════════════════════╣
  ║ GZip XML       ║ 1 KB         ║ 1 KB          ║       0.7 ms ║       0.6 ms ║       1.2 ms ║       0.2 ms ║ OK                 ║
  ║ GZip XML       ║ 1 KB         ║ 100 KB        ║       2.6 ms ║       2.5 ms ║       2.9 ms ║       0.1 ms ║ OK                 ║
  ║ GZip XML       ║ 1 KB         ║ 1 MB          ║      19.7 ms ║      18.4 ms ║      23.0 ms ║       1.2 ms ║ OK                 ║
  ║ GZip XML       ║ 1 KB         ║ 20 MB         ║     364.8 ms ║     356.8 ms ║     389.4 ms ║      10.2 ms ║ OK                 ║
  ║ GZip XML       ║ 100 KB       ║ 1 KB          ║       6.3 ms ║       2.2 ms ║      37.6 ms ║      11.0 ms ║ OK                 ║
  ║ GZip XML       ║ 100 KB       ║ 100 KB        ║       4.3 ms ║       3.9 ms ║       5.1 ms ║       0.4 ms ║ OK                 ║
  ║ GZip XML       ║ 100 KB       ║ 1 MB          ║      21.3 ms ║      20.3 ms ║      22.5 ms ║       0.9 ms ║ OK                 ║
  ║ GZip XML       ║ 100 KB       ║ 20 MB         ║     371.1 ms ║     362.5 ms ║     383.6 ms ║       7.7 ms ║ OK                 ║
  ║ GZip XML       ║ 1 MB         ║ 1 KB          ║      22.2 ms ║      16.6 ms ║      45.6 ms ║       9.6 ms ║ OK                 ║
  ║ GZip XML       ║ 1 MB         ║ 100 KB        ║      19.5 ms ║      18.7 ms ║      21.0 ms ║       0.8 ms ║ OK                 ║
  ║ GZip XML       ║ 1 MB         ║ 1 MB          ║      37.1 ms ║      34.4 ms ║      40.0 ms ║       1.9 ms ║ OK                 ║
  ║ GZip XML       ║ 1 MB         ║ 20 MB         ║     376.3 ms ║     373.0 ms ║     379.5 ms ║       2.1 ms ║ OK                 ║
  ║ GZip XML       ║ 20 MB        ║ 1 KB          ║     403.8 ms ║     358.7 ms ║     445.2 ms ║      28.3 ms ║ OK                 ║
  ║ GZip XML       ║ 20 MB        ║ 100 KB        ║     410.8 ms ║     380.6 ms ║     508.4 ms ║      37.2 ms ║ OK                 ║
  ║ GZip XML       ║ 20 MB        ║ 1 MB          ║     434.3 ms ║     385.2 ms ║     510.7 ms ║      40.0 ms ║ OK                 ║
  ║ GZip XML       ║ 20 MB        ║ 20 MB         ║     812.3 ms ║     744.9 ms ║     924.4 ms ║      57.1 ms ║ OK                 ║
  ╠════════════════╬══════════════╬═══════════════╬══════════════╬══════════════╬══════════════╬══════════════╬════════════════════╣
  ║ ProtoBuf       ║ 1 KB         ║ 1 KB          ║       0.7 ms ║       0.4 ms ║       1.0 ms ║       0.2 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 KB         ║ 100 KB        ║       1.1 ms ║       0.9 ms ║       1.4 ms ║       0.2 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 KB         ║ 1 MB          ║       9.5 ms ║       5.5 ms ║      23.0 ms ║       6.6 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 KB         ║ 20 MB         ║     165.3 ms ║     122.1 ms ║     218.9 ms ║      30.3 ms ║ OK                 ║
  ║ ProtoBuf       ║ 100 KB       ║ 1 KB          ║       2.2 ms ║       1.2 ms ║       3.8 ms ║       0.8 ms ║ OK                 ║
  ║ ProtoBuf       ║ 100 KB       ║ 100 KB        ║       3.1 ms ║       2.0 ms ║       4.8 ms ║       0.9 ms ║ OK                 ║
  ║ ProtoBuf       ║ 100 KB       ║ 1 MB          ║      11.0 ms ║       6.6 ms ║      21.9 ms ║       4.9 ms ║ OK                 ║
  ║ ProtoBuf       ║ 100 KB       ║ 20 MB         ║     173.6 ms ║     109.0 ms ║     231.6 ms ║      41.5 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 MB         ║ 1 KB          ║      12.0 ms ║       6.8 ms ║      31.5 ms ║       9.2 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 MB         ║ 100 KB        ║      12.1 ms ║       5.9 ms ║      38.0 ms ║      10.1 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 MB         ║ 1 MB          ║      22.8 ms ║      11.6 ms ║      62.5 ms ║      16.7 ms ║ OK                 ║
  ║ ProtoBuf       ║ 1 MB         ║ 20 MB         ║     159.3 ms ║     123.5 ms ║     232.7 ms ║      32.4 ms ║ OK                 ║
  ║ ProtoBuf       ║ 20 MB        ║ 1 KB          ║     126.6 ms ║     109.0 ms ║     150.1 ms ║      13.3 ms ║ OK                 ║
  ║ ProtoBuf       ║ 20 MB        ║ 100 KB        ║     124.9 ms ║     109.0 ms ║     156.4 ms ║      14.7 ms ║ OK                 ║
  ║ ProtoBuf       ║ 20 MB        ║ 1 MB          ║     129.8 ms ║     104.9 ms ║     150.7 ms ║      18.1 ms ║ OK                 ║
  ║ ProtoBuf       ║ 20 MB        ║ 20 MB         ║     273.7 ms ║     220.7 ms ║     318.4 ms ║      35.0 ms ║ OK                 ║
  ╚════════════════╩══════════════╩═══════════════╩══════════════╩══════════════╩══════════════╩══════════════╩════════════════════╝

```

## Releases

Tagged releases are built on GitHub Actions for Windows. Each release publishes a single zip archive that contains:

- `WcfNetworkTester.Client/` with the client executable and dependencies
- `WcfNetworkTester.Server/` with the server executable and dependencies

Unzip the archive on a Windows machine with .NET Framework 4.8 installed, then run the `.exe` files from the extracted folders.

## Versioning

Version numbers are derived from Git tags with [MinVer](https://github.com/adamralph/minver). Push a tag such as `v1.2.3` to trigger the release workflow and stamp the built assemblies with that version information.
