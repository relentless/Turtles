module Turtle.Tests.InterpreterTests

open Xunit
open FsUnit.Xunit
open Turtle.AST
open Turtle.Interpreter

let X = fst
let Y = snd

[<Fact>]
let ``forward results in line`` () =
    [Forward(10.0)] 
    |> execute {X=0.0; Y=0.0; Direction=0.0} 
    |> should equal [(0.0,0.0),(0.0,10.0)] 

[<Fact>]
let ``multiple commands result in multiple lines`` () =
    [Forward(10.0);Forward(15.0)] 
    |> execute {X=0.0; Y=0.0; Direction=0.0} 
    |> should equal [
        (0.0,0.0),(0.0,10.0)
        (0.0,10.0),(0.0,25.0)] 

[<Fact>]
let ``turning doesn't add a line`` () =
    [Turn(90.0);Forward(10.0)] 
    |> execute {X=0.0; Y=0.0; Direction=0.0} 
    |> List.length
    |> should equal 1

[<Fact>]
let ``turning moves in the right direction`` () =
    let endPosition =
        [Turn(90.0);Forward(10.0)] 
        |> execute {X=0.0; Y=0.0; Direction=0.0} 
        |> List.head |> snd

    endPosition |> X |> should equal 10.0
    endPosition |> Y |> should (equalWithin 0.01) 0.0