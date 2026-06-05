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

> Placeholder: add sample client output here.

## Releases

Tagged releases are built on GitHub Actions for Windows. Each release publishes a single zip archive that contains:

- `WcfNetworkTester.Client/` with the client executable and dependencies
- `WcfNetworkTester.Server/` with the server executable and dependencies

Unzip the archive on a Windows machine with .NET Framework 4.8 installed, then run the `.exe` files from the extracted folders.

## Versioning

Version numbers are derived from Git tags with [MinVer](https://github.com/adamralph/minver). Push a tag such as `v1.2.3` to trigger the release workflow and stamp the built assemblies with that version information.
