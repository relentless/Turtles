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
    
    let rec exec codeToExec turtle lines colour procs =
        match codeToExec with
        | [] -> lines
        | currentInstruction::rest ->
            match currentInstruction with
            | Turn(angle) -> 
                exec rest { turtle with Direction = (turtle.Direction + angle) } lines colour procs
            | Forward(distance) -> 
                let newX, newY = newPosition turtle.X turtle.Y turtle.Direction distance
                let line = { StartPoint = {X=turtle.X;Y=turtle.Y}; EndPoint = {X=newX;Y=newY}; Colour=colour }
                exec rest { turtle with X = newX; Y = newY } (line::lines) colour procs
            | Repeat(count, commands) -> 
                let flattenedCommands = commands |> List.replicate count |> List.concat
                exec (flattenedCommands@rest) turtle lines colour procs
            | SetColour(newColour) ->
                exec rest turtle lines newColour procs
            | Procedure(name, commands) -> 
                exec rest turtle lines colour (procs |> Map.add name commands)
            | Call(name) -> 
                if not (procs |> Map.containsKey name) then failwith (sprintf "procedure '%s' not found" name)
                let procCommands = (procs.[name])
                exec (procCommands@rest) turtle lines colour procs
            | Variable(_) -> 
                exec rest turtle lines colour procs

    exec code startTurtle [] defaultColour Map.empty |> List.rev