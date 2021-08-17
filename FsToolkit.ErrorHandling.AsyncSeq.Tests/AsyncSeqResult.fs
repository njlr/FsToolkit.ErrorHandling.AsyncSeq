module FsToolkit.ErrorHandling.Tests.AsyncSeqResult

open System
open FSharp.Control
open FsToolkit.ErrorHandling
open Xunit
open FsUnit
open FsUnit.Xunit

[<Fact>]
let ``simple case 1`` () =
  let xs =
    asyncSeqResult {
      1
      2
      3
    }

  let actual =
    xs
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

  let expected = [ Ok 1; Ok 2; Ok 3 ]

  actual |> should equal expected

[<Fact>]
let ``simple case 2`` () =
  let xs =
    asyncSeqResult {
      1
      2
      3
      do! AsyncSeq.singleton (Error "fail")
      4
      failwith "Evaluated too far"
    }

  let actual =
    xs
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

  let expected = [ Ok 1; Ok 2; Ok 3; Error "fail" ]

  actual |> should equal expected

[<Fact>]
let ``simple case 3`` () =
  let xs =
    asyncSeqResult {
      let! x =
        async {
          return 10
        }

      x + 1
      x + 2
      x + 3
    }

  let actual =
    xs
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

  let expected = [ Ok 11; Ok 12; Ok 13 ]

  actual |> should equal expected

[<Fact>]
let ``simple case 4`` () =
  let xs =
    asyncSeqResult {
      1

      for x in [ 2; 3; 4 ] do
        x

      5
    }

  let actual =
    xs
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

  let expected = [ Ok 1; Ok 2; Ok 3; Ok 4; Ok 5 ]

  actual |> should equal expected

[<Fact>]
let ``simple case 5`` () =
  let xs =
    asyncSeqResult {
      for x in AsyncSeq.ofSeq [ "a"; "b"; "c" ] do
        x
    }

  let actual =
    xs
    |> AsyncSeq.toListAsync
    |> Async.RunSynchronously

  let expected = [ Ok "a"; Ok "b"; Ok "c" ]

  actual |> should equal expected
