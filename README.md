# AlwaysOn Profiling playground

This repository contains a playground application used for
exploratory testing of
[AlwaysOn Profiling for Splunk APM](https://docs.splunk.com/Observability/profiling/intro-profiling.html).

This applications:

- uses `async` code in lots of places (threads and continuations are everwhere)
- makes CPU bound transactions called `Bogo.Sort`
- makes network I/O bound transactions called `Http.Request`
- makes network I/O bound transactions called `Http.Handler`
