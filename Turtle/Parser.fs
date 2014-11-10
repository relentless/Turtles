module Turtle.Parser

open FParsec
open Turtle.AST

let pforward = pstring "forward" >>. spaces1 >>. pfloat |>> (fun x -> Forward(x))
let pleft = pstring "left" >>. spaces1 >>. pfloat |>> (fun x -> Turn(-x))
let pright = pstring "right" >>. spaces1 >>. pfloat |>> (fun x -> Turn(x))

let prepeat,prepeatImpl = createParserForwardedToRef()

let pcommand = pforward <|> pleft <|> pright <|> prepeat
let pblock = pstring "[" >>. many1 (pcommand .>> spaces) .>> pstring "]"

do prepeatImpl := pstring "repeat" >>. spaces1 >>. pfloat .>> spaces .>>. pblock
                    |>> (fun (x,commands) -> Repeat(int x, commands))

let parse code =
    match run pcommand code with
    | Success(result, _, _) -> [result]
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)
