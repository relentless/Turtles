#r @"packages\FParsec.1.0.1\lib\net40-client\FParsecCS.dll"
#r @"packages\FParsec.1.0.1\lib\net40-client\FParsec.dll"

#r "System.Drawing.dll"
#r "System.Windows.Forms.dll"

open FParsec
open System.Windows.Forms
open System.Drawing

// to square
//   repeat 4 [forward 50 right 90]
// end

// AST
type Command =
    | Forward of float
    | Turn of float
    | Repeat of int * Command list
    | Procedure of string * Command list
    | Call of string

// PARSER
let pforward = pstring "forward" >>. spaces1 >>. pfloat |>> (fun x -> Forward(x))
let pleft = pstring "left" >>. spaces1 >>. pfloat |>> (fun x -> Turn(-x))
let pright = pstring "right" >>. spaces1 >>. pfloat |>> (fun x -> Turn(x))

//let pcolour = pstring "set-colour" >>. spaces >>. pstring "(" >>. spaces >>. charsTillString ")" true 10 |>> (fun colour -> SetColour(colour.Trim()))

let pcall = pstring "do" >>. spaces >>. pstring "(" >>. spaces >>. manySatisfy isLetter .>> spaces .>> pstring ")" |>> (fun name -> Call(name))

let prepeat,prepeatImpl = createParserForwardedToRef()
let pproc,pprocImpl = createParserForwardedToRef()
//let pcall,pcallImpl = createParserForwardedToRef()

let pcommand = pforward <|> pleft <|> pright <|> prepeat <|> pproc <|> pcall
let pcommandlist = many1 (pcommand .>> spaces)
let pblock = pstring "[" >>. pcommandlist .>> pstring "]"

do prepeatImpl := pstring "repeat" >>. spaces1 >>. pfloat .>> spaces .>>. pblock
                    |>> (fun (x,commands) -> Repeat(int x, commands))

do pprocImpl := 
    pstring "to" >>. spaces1 
    >>. manySatisfy isLetter
    .>> spaces .>>. pcommandlist
    .>> spaces .>> pstring "end"
    |>> (fun (name,commands) -> Procedure(name,commands))

let parse code =
    match run pcommandlist code with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)

parse "repeat 3 [left 30 repeat 5 [forward 50 right 30]]"

// INTERPRETER
type Turtle = { X: float; Y: float; Direction: float }

let execute startTurtle code =

    let newPosition x y angle distance =
        let radians = angle * System.Math.PI / 180.0
        (x + distance * cos radians, y + distance * sin radians)
    
    let rec exec codeToExec turtle lines procs =
        match codeToExec with
        | [] -> lines
        | currentInstruction::rest ->
            match currentInstruction with
            | Turn(angle) -> 
                exec rest { turtle with Direction = (turtle.Direction + angle) } lines procs
            | Forward(distance) -> 
                let newX, newY = newPosition turtle.X turtle.Y turtle.Direction distance
                let line = ((turtle.X,turtle.Y),((newX, newY)))
                exec rest { turtle with X = newX; Y = newY } (line::lines) procs
            | Repeat(count, commands) -> 
                let flattenedCommands = commands |> List.replicate count |> List.concat
                exec (flattenedCommands@rest) turtle lines procs
            | Procedure(name, commands) -> 
                exec rest turtle lines (procs |> Map.add name commands)
            | Call(name) -> 
                if not (procs |> Map.containsKey name) then failwith (sprintf "procedure '%s' not found" name)
                let procCommands = (procs.[name])
                exec (procCommands@rest) turtle lines procs

    exec code startTurtle [] Map.empty

// DISPLAY

let width,height = 800,600

let display lines =
    let form = new Form (Text="Turtles", Width=width, Height=height)
    let image = new Bitmap(width, height)
    let picture = new PictureBox(Dock=DockStyle.Fill, Image=image)
    do  form.Controls.Add(picture)
    let pen = new Pen(Color.Red)

    let drawLine ((x1,y1),(x2,y2)) =
        use graphics = Graphics.FromImage(image)
        graphics.DrawLine(pen,int x1,int y1,int x2, int y2)

    lines |> List.iter drawLine

    form.ShowDialog() |> ignore

//"repeat 4 [forward 50 repeat 3 [right 30 forward 30]]"
//"repeat 3 [forward 80 repeat 20 [right 30 forward 30]]"

//"repeat 4 [forward 50 right 90 repeat 4 [forward 5 right 180 forward 5 right 90]]"
@"to square
  repeat 4 [forward 50 right 90]
end

to line forward 100 end

do(line)
do( square )
do (line)
do(square)"
|> parse
|> execute { X=(float width)/2.0; Y=(float height)/2.0; Direction=0.0 }
|> display
