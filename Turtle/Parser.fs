module Turtle.Parser

open FParsec
open Turtle.AST

let vars = ref []
let pvarcall,pvarcallImpl = createParserForwardedToRef()

let pvalue = pfloat |>> (fun x -> x)
//let pvariable = (pstring "x") |>> (fun _ -> 10.0)
let pvariableorvalue = (pvalue <|> pvarcall)

let pforward = (pstring "forward" <|> pstring "fd") >>. spaces1 >>. pvariableorvalue |>> (fun x -> Forward(x))
let pleft = (pstring "left" <|> pstring "lt") >>. spaces1 >>. pvariableorvalue |>> (fun x -> Turn(-x))
let pright = (pstring "right" <|> pstring "rt") >>. spaces1 >>. pvariableorvalue |>> (fun x -> Turn(x))
let pcolour = pstring "set-colour" >>. spaces >>. pstring "(" >>. spaces >>. charsTillString ")" true 10 |>> (fun colour -> SetColour(colour.Trim()))

let prepeat,prepeatImpl = createParserForwardedToRef()
let pproc,pprocImpl = createParserForwardedToRef()
let pcall,pcallImpl = createParserForwardedToRef()
let pvardeclaration,pvardeclarationImpl = createParserForwardedToRef()

let pcomment = pchar '#' >>. restOfLine true
let pcommand = spaces >>. (pforward <|> pleft <|> pright <|> prepeat <|> pcolour <|> pproc <|> pcall <|> pvardeclaration)
let pcommandcomment = optional pcomment >>. pcommand .>> optional pcomment
let pcommandlist = many1 (pcommandcomment .>> spaces)

let pblock = pstring "[" >>. pcommandlist .>> pstring "]"

do prepeatImpl := (pstring "repeat" <|> pstring "rpt") >>. spaces1 >>. pfloat .>> spaces .>>. pblock
                    |>> (fun (x,commands) -> Repeat(int x, commands))

// procedure calls
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

let addToVariablesList var =
    vars := var::!vars

let createVariableCallParser = function
    | Variable(name,value) -> pstring name |>> (fun _ -> value)
    | _ -> failwith "That's not a variable"

let updateVarDeclarationParser() =
    do pvarcallImpl := choice (!vars |> List.map createVariableCallParser)

updateVarDeclarationParser()

do pvardeclarationImpl :=
    pstring "let" 
    >>. spaces1
    >>. manySatisfy isLetter
    .>> spaces1
    .>> pstring "be"
    .>> spaces1
    .>>. pfloat
    |>> (fun (name,value) ->
        let newVar = Variable(name,value)
        newVar |> addToVariablesList
        updateVarDeclarationParser()
        newVar)

let parse code =
    match run pcommandlist code with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)
