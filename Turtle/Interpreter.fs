module Turtle.Interpreter

open Turtle.AST

type Turtle = { X: float; Y: float; Direction: float }
type Point = { X: float; Y:float }
type Line = { StartPoint: Point; EndPoint: Point; Colour:string }

let defaultColour = "default"

let execute startTurtle code =

    let newPosition x y angle distance =
        let radians = angle * System.Math.PI / 180.0
        (x + distance * sin radians, y + distance * cos radians)
    
    let rec exec codeToExec turtle lines colour =
        match codeToExec with
        | [] -> lines
        | currentInstruction::rest ->
            match currentInstruction with
            | Turn(angle) -> 
                exec rest { turtle with Direction = (turtle.Direction + angle) } lines colour
            | Forward(distance) -> 
                let newX, newY = newPosition turtle.X turtle.Y turtle.Direction distance
                let line = { StartPoint = {X=turtle.X;Y=turtle.Y}; EndPoint = {X=newX;Y=newY}; Colour=colour }
                //let line = ((turtle.X,turtle.Y),((newX, newY)),colour)
                exec rest { turtle with X = newX; Y = newY } (line::lines) colour
            | Repeat(count, commands) -> 
                let flattenedCommands = commands |> List.replicate count |> List.concat
                exec (flattenedCommands@rest) turtle lines colour
            | SetColour(newColour) ->
                exec rest turtle lines newColour

    exec code startTurtle [] defaultColour |> List.rev