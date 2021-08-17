# FsToolkit.ErrorHandling.AsyncSeq

Allows usage of `AsyncSeq<'t>` and `AsyncSeq<Result<'t, 'e>>` in `Async<Result<'t, 'e>>` builders.

It also introduces a new computation expression, `asyncSeqResult` which allows you to write async sequences that might fail (`AsyncSeq<Result<'t, 'e>>`).

Install with Paket:

```bash
dotnet paket add FsToolkit.ErrorHandling.AsyncSeq
```

In a script:

```fsharp
#r "nuget: FSharp.Control.AsyncSeq"
#r "nuget: FsToolkit.ErrorHandling"
#r "nuget: FsToolkit.ErrorHandling.AsyncSeq"

open FSharp.Control
open FsToolkit.ErrorHandling

let xs =
  asyncSeq {
    1
    2
    3
  }

asyncResult {
  let mutable sum = 0

  for x in xs do
    sum <- sum + x

  return sum
}
|> Async.RunSynchronously
|> printfn "%A"
```
