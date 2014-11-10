module Turtle.AST

type Command =
    | Forward of float
    | Turn of float
    | Repeat of int * Command list
