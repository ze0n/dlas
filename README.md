# dlas

**Debug like a scientist.**

dlas instruments a quantitative .NET application at runtime and shows the values it reports as live charts. A small .NET library records scalars and vectors while the process runs. A Python dashboard, started alongside it, reads those records and draws them.

The two sides do not talk to each other directly. Both use a local MongoDB database named `dlas`. Each debug session is its own collection. The dashboard polls that database about once a second and redraws the selected series.

```
.NET application                MongoDB                     Python dashboard
Dlas.Report(...).Send()  --->   database: dlas         --->  Dash + Plotly
                                collection: <session>         histogram, timeseries, scatter
```

## What you can report

| Call | Stored as | Drawn as |
| --- | --- | --- |
| `Dlas.Report(double, id)` | scalar | histogram, timeseries, or scatter vs time |
| `Dlas.Report(int, id)` | scalar | same as double |
| `Dlas.Report(IEnumerable<double>, id)` | vector | 3D scatter of index, time, and value |

Each record also stores when and where it was taken: machine name, process id, thread id, timestamp, session id, and a stack trace.

`AsTimeSeries()` and `AsHistogram()` attach a chart hint to the record. The dashboard currently lets you pick the chart in the UI; it does not yet read that hint.

## Prerequisites

- MongoDB listening on `mongodb://localhost:27017`
- .NET SDK that can build `netstandard2.0` (the sample test project targets `netcoreapp3.1`)
- Python 3 with the packages in `src/debug-host/dlas/requirements.txt`

## Run the dashboard

From `src/debug-host/dlas`:

```bash
pip install -r requirements.txt
python dlas.py
```

Dash serves the UI on its default port, `8050`. Open that address in a browser.

The home page lists sessions (MongoDB collection names). Open a session, pick a reported variable, then choose a chart:

- **Histogram** — distribution of scalar values
- **Timeseries** — value against timestamp, with lines and markers
- **Scatterplot vs time** — value against timestamp for scalars; a 3D scatter of timestamp, index, and value for vectors

**Clean** drops every session collection in the `dlas` database.

`settings.py` defines `COLLECTOR_PORT` (`8821`) and `DASHBOARD_PORT` (`8822`). Those values are not wired into the app yet.

## Instrument a .NET application

Reference the library project:

`src/debug-agents/net/Dlas.Agent/Dlas.Agent/Debug.Like.A.Scientist.csproj`

Namespace: `Debug.Like.A.Scientist`. The first `Report` call opens a session and connects to MongoDB. Later calls with the same id append to that series. `Send()` blocks until the write finishes or the timeout in `ApiSettings` elapses (one minute by default). Failures are swallowed when `ApiSettings.Silent` is true, which is the default.

```csharp
using Debug.Like.A.Scientist;

for (int i = 0; i < 300; i++)
{
    double a = /* some quantity */;
    Dlas.Report(a, "A")
        .AsTimeSeries()
        .Send();

    double b = Math.Sin(i / 3.0);
    Dlas.Report(b, "B")
        .AsHistogram()
        .Send();

    double[] path = /* a vector that changes over time */;
    Dlas.Report(path, "V")
        .Send();
}
```

`id` is the name shown in the dashboard. If you pass null or whitespace, dlas generates a GUID. An optional `instance` string can distinguish repeated uses of the same id.

Async send:

```csharp
await Dlas.Report(a, "A").AsTimeSeries().SendAsync(cancellationToken);
```

A session id looks like `yyyy-MM-dd-HH-mm-ss-<guid>`. To keep writing into an existing session:

```csharp
await Dlas.SessionManager.OpenSession(cancellationToken, existingSessionId);
```

`Dlas.SessionManager.CloseCurrentSession` disconnects the client. The next report opens a new session.

The test project under `src/debug-agents/net/Dlas.Agent/Dlas.Agent.Tests` is a live producer: it reports two scalars and an evolving vector of length 100, once per second, for 300 iterations. Run it with MongoDB and the dashboard already up.

## Layout

```
src/debug-agents/net/Dlas.Agent/   .NET library and NUnit sample
src/debug-host/dlas/               Dash dashboard
```

The library targets `netstandard2.0` and depends on `MongoDB.Driver`. `NetMQ` is referenced but unused; transport today is MongoDB only.

`Series` and `DataFrame` types exist in the library. Reporters for them are not implemented, and the dashboard only renders `ScalarReporter` and `VectorReporter`.
