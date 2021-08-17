module FsToolkit.ErrorHandling.Tests.AsyncSeq

open System
open FSharp.Control
open FsToolkit.ErrorHandling
open Xunit
open FsUnit
open FsUnit.Xunit

[<Fact>]
let ``simple case 1`` () =
  let xs =
    asyncSeq {
      1
      2
      3
    }

  let actual =
    asyncResult {
      let mutable sum = 0

      for x in xs do
        sum <- sum + x

      return sum
    }
    |> Async.RunSynchronously

  let expected = Ok 6

  actual |> should equal expected

[<Fact>]
let ``simple case 2`` () =
  let xs =
    asyncSeq {
      Ok 1
      Ok 2
      Error "oh no"
      failwith "Evaluated too far"
    }

  let actual =
    asyncResult {
      let mutable sum = 0

      for x in xs do
        sum <- sum + x

      return sum
    }
    |> Async.RunSynchronously

  let expected : Result<int, _> = Error "oh no"

  actual |> should equal expected
