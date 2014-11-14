module Turtle.Parser

open FParsec
open Turtle.AST

let pforward = (pstring "forward" <|> pstring "fd") >>. spaces1 >>. pfloat |>> (fun x -> Forward(x))
let pleft = (pstring "left" <|> pstring "lt") >>. spaces1 >>. pfloat |>> (fun x -> Turn(-x))
let pright = (pstring "right" <|> pstring "rt") >>. spaces1 >>. pfloat |>> (fun x -> Turn(x))

let prepeat,prepeatImpl = createParserForwardedToRef()

let pcommand = pforward <|> pleft <|> pright <|> prepeat
let pcommandlist = many1 (pcommand .>> spaces)
let pblock = pstring "[" >>. pcommandlist .>> pstring "]"

do prepeatImpl := (pstring "repeat" <|> pstring "rpt") >>. spaces1 >>. pfloat .>> spaces .>>. pblock
                    |>> (fun (x,commands) -> Repeat(int x, commands))

let parse code =
    match run pcommandlist code with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)
