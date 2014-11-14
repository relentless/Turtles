module Turtle.Tests.IntegrationTests

open Xunit
open FsUnit.Xunit
open Turtle.AST
open Turtle.Parser
open Turtle.Interpreter

let X = fst
let Y = snd

[<Fact>]
let ``text converted to lines`` () =
    "repeat 2 [forward 10 forward -5]"
    |> parse
    |> execute {X=0.0; Y=0.0; Direction=0.0}
    |> should equal [
        (0.0,0.0),(0.0,10.0)
        (0.0,10.0),(0.0,5.0)
        (0.0,5.0),(0.0,15.0)
        (0.0,15.0),(0.0,10.0)]


