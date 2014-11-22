module Turtle.Parser

open FParsec
open Turtle.AST

let pforward = (pstring "forward" <|> pstring "fd") >>. spaces1 >>. pfloat |>> (fun x -> Forward(x))
let pleft = (pstring "left" <|> pstring "lt") >>. spaces1 >>. pfloat |>> (fun x -> Turn(-x))
let pright = (pstring "right" <|> pstring "rt") >>. spaces1 >>. pfloat |>> (fun x -> Turn(x))
let pcolour = pstring "set-colour" >>. spaces >>. pstring "(" >>. spaces >>. charsTillString ")" true 10 |>> (fun colour -> SetColour(colour.Trim()))

let prepeat,prepeatImpl = createParserForwardedToRef()
let pproc,pprocImpl = createParserForwardedToRef()
let pcall,pcallImpl = createParserForwardedToRef()

let pcommand = pforward <|> pleft <|> pright <|> prepeat <|> pcolour <|> pproc <|> pcall
let pcommandlist = many1 (pcommand .>> spaces)
let pblock = pstring "[" >>. pcommandlist .>> pstring "]"

do prepeatImpl := (pstring "repeat" <|> pstring "rpt") >>. spaces1 >>. pfloat .>> spaces .>>. pblock
                    |>> (fun (x,commands) -> Repeat(int x, commands))

let procs = ref []

let createProcedureCallParser = function
    | Procedure(name,_) -> pstring name .>> spaces |>> (fun procName -> Call(procName))
    | _ -> failwith "That's not a procedure"

let updateCallsParser() = 
    do pcallImpl := choice (!procs |> List.map createProcedureCallParser)

updateCallsParser()

let addToProceduresList proc =
    procs := proc::!procs

do pprocImpl := 
    pstring "to" >>. spaces1 
    >>. manySatisfy isLetter
    .>> spaces .>>. pcommandlist
    .>> spaces .>> pstring "end"
    |>> (fun (name,commands) -> 
            let newProc = Procedure(name,commands)
            newProc |> addToProceduresList
            updateCallsParser()
            newProc)

let parse code =
    match run pcommandlist code with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)
