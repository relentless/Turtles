#r @"packages\FParsec.1.0.1\lib\net40-client\FParsecCS.dll"
#r @"packages\FParsec.1.0.1\lib\net40-client\FParsec.dll"

#r "System.Drawing.dll"
#r "System.Windows.Forms.dll"

open FParsec
open System.Windows.Forms
open System.Drawing

// AST
type Command =
    | Forward of float
    | Turn of float
    | Repeat of int * Command list
    | Procedure of Proc
    | Call of string
and Proc = { Name: string; Body: Command list }

// PARSER
let pforward = pstring "forward" >>. spaces1 >>. pfloat |>> (fun x -> Forward(x))
let pleft = pstring "left" >>. spaces1 >>. pfloat |>> (fun x -> Turn(-x))
let pright = pstring "right" >>. spaces1 >>. pfloat |>> (fun x -> Turn(x))

let pcall = pstring "do" >>. spaces >>. pstring "(" >>. spaces >>. manySatisfy isLetter .>> spaces .>> pstring ")" |>> (fun name -> Call(name))

let prepeat,prepeatImpl = createParserForwardedToRef()
let pproc,pprocImpl = createParserForwardedToRef()

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
    |>> (fun (name,commands) -> Procedure( {Name=name; Body=commands}))

let parse code =
    match run pcommandlist code with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith ("Parse error: " + errorMsg)

parse "repeat 3 [left 30 repeat 5 [forward 50 right 30]]"

// INTERPRETER
type Turtle = { X: float; Y: float; Direction: float }

type Point = { X: float; Y:float }

type Line = { StartPoint: Point; EndPoint: Point }

type Environment = { Lines: Line list; Turtle: Turtle; Procedures: Proc list }

let execute startTurtle code =

    let newPosition x y angle distance =
        let radians = angle * System.Math.PI / 180.0
        (x + distance * cos radians, y + distance * sin radians)
    
    let rec exec codeToExec env =
        match codeToExec with
        | [] -> env.Lines
        | currentInstruction::rest ->
            match currentInstruction with
            | Turn(angle) -> 
                exec rest {env with Turtle = { env.Turtle with Direction = (env.Turtle.Direction + angle) } }
            | Forward(distance) -> 
                let newX, newY = newPosition env.Turtle.X env.Turtle.Y env.Turtle.Direction distance
                let line = { StartPoint = {X=env.Turtle.X;Y=env.Turtle.Y}; EndPoint = {X=newX;Y=newY} }
                exec rest {env with Turtle = { env.Turtle with X = newX; Y = newY}; Lines=line::env.Lines }
                //exec rest { turtle with X = newX; Y = newY } (line::lines) procs
            | Repeat(count, commands) -> 
                let flattenedCommands = commands |> List.replicate count |> List.concat
                exec (flattenedCommands@rest) env
            | Procedure({Name=name; Body=commands}) -> 
                exec rest {env with Procedures=({Name=name;Body=commands}::env.Procedures)}
            | Call(name) -> 
                if not (env.Procedures |> List.exists (fun proc -> proc.Name=name)) then failwith (sprintf "procedure '%s' not found" name)
                let procCommands = (env.Procedures |> List.find (fun proc -> proc.Name=name)).Body
                exec (procCommands@rest) env

    exec code {Turtle=startTurtle;Lines=[];Procedures=[]}

// DISPLAY

let width,height = 800,600

let display lines =
    let form = new Form (Text="Turtles", Width=width, Height=height)
    let image = new Bitmap(width, height)
    let picture = new PictureBox(Dock=DockStyle.Fill, Image=image)
    do  form.Controls.Add(picture)
    let pen = new Pen(Color.Red)

    let drawLine {StartPoint={X=x1;Y=y1}; EndPoint={X=x2;Y=y2}} =
        use graphics = Graphics.FromImage(image)

        graphics.DrawLine(pen,int x1,int y1,int x2, int y2)

    lines |> List.iter drawLine

    form.ShowDialog() |> ignore

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
