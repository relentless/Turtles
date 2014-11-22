module Turtle.Tests.InterpreterTests

open Xunit
open FsUnit.Xunit
open Turtle.AST
open Turtle.Interpreter

let endPoint line = line.EndPoint
let lastLine lines =
    lines
    |> List.rev
    |> List.head

[<Fact>]
let ``forward results in line`` () =
    [Forward(10.0)] 
    |> execute {X=0.0; Y=0.0; Direction=0.0} 
    |> lastLine 
    |> endPoint
    |> should equal {X=0.0;Y=10.0}

[<Fact>]
let ``multiple commands result in multiple lines`` () =
    [Forward(10.0);Forward(15.0)] 
    |> execute {X=0.0; Y=0.0; Direction=0.0} 
    |> should equal [
        {StartPoint={X=0.0;Y=0.0}; EndPoint={X=0.0;Y=10.0}; Colour=defaultColour}
        {StartPoint={X=0.0;Y=10.0}; EndPoint={X=0.0;Y=25.0}; Colour=defaultColour}] 

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
        |> lastLine
        |> endPoint

    endPosition.X |> should equal 10.0
    endPosition.Y |> should (equalWithin 0.01) 0.0

[<Fact>]
let ``calling a procedure executes its commands`` () =
    [Procedure("go",[Forward(1.0);Forward(2.0)]);Call("go")] 
    |> execute {X=0.0; Y=0.0; Direction=0.0} 
    |> lastLine
    |> endPoint
    |> should equal {X=0.0;Y=3.0}