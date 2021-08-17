namespace FsToolkit.ErrorHandling

open FSharp.Control
open FsToolkit.ErrorHandling

type AsyncSeqResultBuilder() =
  member this.Bind(m : AsyncSeq<Result<'t, 'e>>, f) =
    m
    |> AsyncSeq.collect (fun r ->
      match r with
      | Ok x -> f x
      | Error e -> AsyncSeq.singleton (Error e)
    )
    |> AsyncSeq.takeWhileInclusive Result.isOk

  member this.Bind(m : Async<'t>, f) =
    AsyncSeq.ofSeqAsync [ m ]
    |> AsyncSeq.collect f
    |> AsyncSeq.takeWhileInclusive Result.isOk

  member this.Yield(x) =
    x
    |> Ok
    |> AsyncSeq.singleton

  member this.Combine(a : AsyncSeq<Result<'t, 'e>>, b : AsyncSeq<Result<'t, 'e>>) =
    AsyncSeq.append a b
    |> AsyncSeq.takeWhileInclusive Result.isOk

  member this.For(xs : seq<'t>, f : 't -> AsyncSeq<Result<'u, 'e>>) : AsyncSeq<Result<'u, 'e>> =
    xs
    |> Seq.map f
    |> Seq.fold AsyncSeq.append AsyncSeq.empty
    |> AsyncSeq.takeWhileInclusive Result.isOk

  member this.For(xs : AsyncSeq<'t>, f : 't -> AsyncSeq<Result<'u, 'e>>) : AsyncSeq<Result<'u, 'e>> =
    xs
    |> AsyncSeq.collect f
    |> AsyncSeq.takeWhileInclusive Result.isOk

  member this.Zero() : AsyncSeq<Result<'t, 'e>> =
    AsyncSeq.empty

  member this.Delay(f : Unit -> AsyncSeq<Result<'t, 'e>>) : AsyncSeq<Result<'t, 'e>> =
    f ()

[<AutoOpen>]
module ComputationExpression =

  let asyncSeqResult = AsyncSeqResultBuilder()

module AsyncSeqResult =

  let returnError error =
    error
    |> Error
    |> AsyncSeq.singleton

  let map f xs =
    xs
    |> AsyncSeq.map (Result.map f)

  let mapError f xs =
    xs
    |> AsyncSeq.map (Result.mapError f)
