module Lib.Colorize

open Spectre.Console

type ColorScheme = {
    LabelColor: Color
    ValueColor: Color
    HeaderColor: Color
}

let getColorScheme (distroId: string) : ColorScheme =
    match distroId.ToLower() with
    | "fedora" -> 
        { LabelColor = Color.Blue
          ValueColor = Color.White
          HeaderColor = Color.Blue }
    
    | "ubuntu" -> 
        { LabelColor = Color.Orange1
          ValueColor = Color.White
          HeaderColor = Color.Orange1 }
    
    | "debian" -> 
        { LabelColor = Color.Red
          ValueColor = Color.White
          HeaderColor = Color.Red }
    
    | "arch" -> 
        { LabelColor = Color.Blue
          ValueColor = Color.White
          HeaderColor = Color.Blue }
    
    | "linux mint" | "mint" -> 
        { LabelColor = Color.Green
          ValueColor = Color.White
          HeaderColor = Color.Green }
    
    | "nixos" -> 
        { LabelColor = Color.Blue
          ValueColor = Color.White
          HeaderColor = Color.Blue }
    
    | "alpine" -> 
        { LabelColor = Color.Blue
          ValueColor = Color.White
          HeaderColor = Color.Blue }
    
    | "windows 10" | "windows 11" | "windows" -> 
        { LabelColor = Color.Blue
          ValueColor = Color.White
          HeaderColor = Color.Blue }
    
    | _ -> // Default
        { LabelColor = Color.HotPink
          ValueColor = Color.White
          HeaderColor = Color.HotPink }
