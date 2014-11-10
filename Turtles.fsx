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

let parse code =
    match run pcommand code with
    | Success(result, _, _) -> [result]
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)

parse "repeat 3 [left 30 repeat 5 [forward 50 right 30]]"

// INTERPRETER
type Turtle = { X: int; Y: int; Direction: int }
let startTurtle = { X=500; Y=500; Direction=0 }

let execute code =
    
    let rec exec codeToExec currentTurtle points =
        //printfn "Code: %A\nTurtle: %A" codeToExec currentTurtle
        match codeToExec with
        | [] -> currentTurtle,points
        | currentInstruction::rest ->
                match currentInstruction with
                | Turn(angle) -> 
                    exec rest { currentTurtle with Direction = (currentTurtle.Direction + angle) } points
                | Forward(distance) -> 
                    exec rest { currentTurtle with Y = currentTurtle.Y + distance } ((currentTurtle.X, currentTurtle.Y + distance)::points)
                | Repeat(count, commands) -> 
                    let flattenedCommands = commands |> List.replicate count |> List.concat
                    exec (flattenedCommands@rest) currentTurtle points

    exec code startTurtle [(startTurtle.X, startTurtle.Y)]

"repeat 3 [forward 5 repeat 4 [forward 1]]"
|> parse
|> execute
