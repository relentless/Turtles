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

[<Fact>]
let ``procedure definition and call parsed correctly`` () =
    parse "to go forward 1 end go" |> should equal [Procedure("go",[Forward(1.0)]);Call("go")]

[<Fact>]
let ``text between # and the end of the line is ignored`` () =
    parse @"#hi mum
forward 1 # comment 2
right 90" |> should equal [Forward(1.0);Turn(90.0)]

[<Fact>]
let ``empty lines are ignored`` () =
    parse @"
forward 1" |> should equal [Forward(1.0)]

[<Fact>]
let ``spaces before commands are ignored`` () =
    parse @"  forward 1
    repeat 3 [  right 5]" |> should equal [Forward(1.0);Repeat(3,[Turn(5.0)])]


// let x be 10
// forward x

[<Fact>]
let ``variable declaration`` () =
    parse "let x be 10" |> should equal [Variable("x",10.0)]


[<Fact>]
let ``variables can be used`` () =
    parse @"let x be 10 
    forward x" |> should equal [Variable("x",10.0);Forward(10.0)]