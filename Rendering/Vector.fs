module Raytracer.Rendering.Vector

open System
open SkiaSharp

type Vec3 =
    { X: double; Y: double; Z: double }
    
    static member (+) (a, b) =
        { X = a.X + b.X; Y = a.Y + b.Y; Z = a.Z + b.Z }
    
    static member (-) (a, b) =
        { X = a.X - b.X; Y = a.Y - b.Y; Z = a.Z - b.Z }
    
    static member (*) (a, b) =
        { X = a.X * b.X; Y = a.Y * b.Y; Z = a.Z * b.Z }
    
    static member (*) (a, b) =
        { X = a.X * b; Y = a.Y * b; Z = a.Z * b }
        
    static member (*) (b, a) =
        { X = a.X * b; Y = a.Y * b; Z = a.Z * b }
    
    static member (/) (a, b) =
        { X = a.X / b.X; Y = a.Y / b.Y; Z = a.Z / b.Z }
    
    static member (/) (a, b) =
        { X = a.X / b; Y = a.Y / b; Z = a.Z / b }
    
    static member (/) (b, a) =
        { X = a.X / b; Y = a.Y / b; Z = a.Z / b }
        
    member this.Item with get (i: int) =
        match i with
        | 0 -> this.X
        | 1 -> this.Y
        | 2 -> this.Z
        | _ -> raise <| IndexOutOfRangeException()

module Vec3 =
    let create x y z =
        { X = x; Y = y; Z = z }
        
    let negate v =
        v * -1
        
    let length_squared v =
        v.X ** 2.0 + v.Y ** 2.0 + v.Z ** 2.0
     
    let length v =
        length_squared v |> Math.Sqrt
        
    let dot u v =
        u.X * v.X + u.Y * v.Y + u.Z * v.Z
    
    let cross u v =
        { X = u.Y * v.Z - u.Z * v.Y; Y = u.Z * v.X - u.X * v.Z; Z = u.X * v.Y - u.Y * v.X }
    
    let normalize v =
        v / length v
        
type Point3 = Vec3

type Color = Vec3

module Color =
    let toSKColor (color: Color) =
        let scaled = color * 255.999
        SKColor (byte scaled.X, byte scaled.Y, byte scaled.Z)
        
        