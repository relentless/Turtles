module Turtle.Interpreter

open Turtle.AST

type Turtle = { X: float; Y: float; Direction: float }

let execute startTurtle code =

    let newPosition x y angle distance =
        let radians = angle * System.Math.PI / 180.0
        (x + distance * cos radians, y + distance * sin radians)
    
    let rec exec codeToExec turtle lines =
        match codeToExec with
        | [] -> lines
        | currentInstruction::rest ->
            match currentInstruction with
            | Turn(angle) -> 
                exec rest { turtle with Direction = (turtle.Direction + angle) } lines
            | Forward(distance) -> 
                let newX, newY = newPosition turtle.X turtle.Y turtle.Direction distance
                let line = ((turtle.X,turtle.Y),((newX, newY)))
                exec rest { turtle with X = newX; Y = newY } (line::lines)
            | Repeat(count, commands) -> 
                let flattenedCommands = commands |> List.replicate count |> List.concat
                exec (flattenedCommands@rest) turtle lines

    exec code startTurtle []