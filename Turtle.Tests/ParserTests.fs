module Turtle.Tests.ParserTests

open Xunit
open FsUnit.Xunit
open Turtle.AST
open Turtle.Parser

[<Fact>]
let ``forward parsed correctly`` () =
    parse "forward 10" |> should equal [Forward(10.0)]

[<Fact>]
let ``left parsed correctly to negative turn`` () =
    parse "left 1" |> should equal [Turn(-1.0)]

[<Fact>]
let ``right parsed correctly to positive turn`` () =
    parse "right 100" |> should equal [Turn(100.0)]

[<Fact>]
let ``muptiple commands parsed correctly into list`` () =
    parse "right 90 forward 50" |> should equal [Turn(90.0);Forward(50.0)]
    
[<Fact>]
let ``repeat parsed correctly to tree of commands`` () =
    parse "repeat 3 [forward 10]" |> should equal [Repeat(3,[Forward(10.0)])]
    
[<Fact>]
let ``shortened commands parsed correctly`` () =
    parse "fd 10" |> should equal [Forward(10.0)]
    parse "lt 1" |> should equal [Turn(-1.0)]
    parse "rt 100" |> should equal [Turn(100.0)]
    parse "rpt 3 [fd 10]" |> should equal [Repeat(3,[Forward(10.0)])]