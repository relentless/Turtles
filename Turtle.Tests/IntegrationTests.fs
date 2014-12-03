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
        {StartPoint={X=0.0;Y=0.0}; EndPoint={X=0.0;Y=10.0}; Colour="default"}
        {StartPoint={X=0.0;Y=10.0}; EndPoint={X=0.0;Y=5.0}; Colour="default"}
        {StartPoint={X=0.0;Y=5.0}; EndPoint={X=0.0;Y=15.0}; Colour="default"}
        {StartPoint={X=0.0;Y=15.0}; EndPoint={X=0.0;Y=10.0}; Colour="default"}]

[<Fact>]
let ``using variables is cool`` () =
    @"let x be 10
    forward x"
    |> parse
    |> execute {X=0.0; Y=0.0; Direction=0.0}
    |> should equal [
        {StartPoint={X=0.0;Y=0.0}; EndPoint={X=0.0;Y=10.0}; Colour="default"}]
