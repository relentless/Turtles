module Turtle.AST

type Command =
    | Forward of float
    | Turn of float
    | Repeat of int * Command list
    | SetColour of string
    | Procedure of string * Command list
    | Call of string
