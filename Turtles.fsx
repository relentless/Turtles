#r @"packages\FParsec.1.0.1\lib\net40-client\FParsecCS.dll"
#r @"packages\FParsec.1.0.1\lib\net40-client\FParsec.dll"

#r "System.Drawing.dll"
#r "System.Windows.Forms.dll"

open FParsec

// AST
type Command =
    | Forward of int
    | Turn of int
    | Repeat of int * Command list

// PARSER
let pforward = pstring "forward" >>. spaces1 >>. pfloat |>> (fun x -> Forward(int x))
let pleft = pstring "left" >>. spaces1 >>. pfloat |>> (fun x -> Turn(int -x))
let pright = pstring "right" >>. spaces1 >>. pfloat |>> (fun x -> Turn(int x))

let prepeat,prepeatImpl = createParserForwardedToRef()

let pcommand = pforward <|> pleft <|> pright <|> prepeat
let pblock = pstring "[" >>. many1 (pcommand .>> spaces) .>> pstring "]"

do prepeatImpl := pstring "repeat" >>. spaces1 >>. pfloat .>> spaces .>>. pblock
                    |>> (fun (x,commands) -> Repeat(int x, commands))

// test the parsers
let test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

test pforward "forward 6"
test pleft "left 6"
test pright "right 2"
test pblock "[left 3 forward 2]"
test pcommand "repeat 3 [left 30 repeat 5 [forward 50 right 30]]"

// INTERPRETER