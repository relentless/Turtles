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

// PARSER
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

parse "repeat 3 [left 30 repeat 5 [forward 50 right 30]]"

// INTERPRETER
type Turtle = { X: float; Y: float; Direction: float }

let execute startTurtle code =

    let newPosition x y angle distance =
        let radians = angle * System.Math.PI / 180.0
        (x + distance * cos radians, y + distance * sin radians)
    
    let rec exec codeToExec currentTurtle points =
        match codeToExec with
        | [] -> points
        | currentInstruction::rest ->
            match currentInstruction with
            | Turn(angle) -> 
                exec rest { currentTurtle with Direction = (currentTurtle.Direction + angle) } points
            | Forward(distance) -> 
                let newX, newY = newPosition currentTurtle.X currentTurtle.Y currentTurtle.Direction distance
                exec rest { currentTurtle with X = newX; Y = newY } ((newX, newY)::points)
            | Repeat(count, commands) -> 
                let flattenedCommands = commands |> List.replicate count |> List.concat
                exec (flattenedCommands@rest) currentTurtle points

    exec code startTurtle [(startTurtle.X, startTurtle.Y)]

// DISPLAY

let width,height = 800,600

let display points =
    let form = new Form (Text="Turtles", Width=width, Height=height)
    let image = new Bitmap(width, height)
    let picture = new PictureBox(Dock=DockStyle.Fill, Image=image)
    do  form.Controls.Add(picture)
    let pen = new Pen(Color.Red)
    let drawLine (x1,y1) (x2,y2) =
        use graphics = Graphics.FromImage(image)
        graphics.DrawLine(pen,int x1,int y1,int x2, int y2)
    let rec draw = function
        | point1::point2::rest -> 
            drawLine point1 point2
            draw (point2::rest)
        | _ -> []

    draw points |> ignore

    form.ShowDialog() |> ignore

//"repeat 4 [forward 50 repeat 3 [right 30 forward 30]]"
"repeat 3 [forward 80 repeat 20 [right 30 forward 30]]"
|> parse
|> execute { X=(float width)/2.0; Y=(float height)/2.0; Direction=0.0 }
|> display