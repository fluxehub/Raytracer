namespace Raytracer.Rendering

open System
open System.Runtime.Intrinsics
open SkiaSharp
open MathSharp

type Vector3(v: Vector256<float>) =
    member val Vector = v with get, set

    new(x: float, y: float, z: float) = Vector3(Vector256.Create(x, y, z, 0.0))

    member this.X = this.Vector.GetElement 0

    member this.Y = this.Vector.GetElement 1

    member this.Z = this.Vector.GetElement 2

    static member (+)(a: Vector3, b: Vector3) = Vector3(Vector.Add(a.Vector, b.Vector))

    static member (-)(a: Vector3, b: Vector3) =
        Vector3(Vector.Subtract(a.Vector, b.Vector))
        
    static member (~-)(a: Vector3) =
        Vector3(Vector.Negate(a.Vector))

    static member (*)(a: Vector3, b: Vector3) =
        Vector3(Vector.Multiply(a.Vector, b.Vector))

    static member (*)(a: Vector3, b: float) = Vector3(Vector.Multiply(a.Vector, b))

    static member (*)(a: float, b: Vector3) = Vector3(Vector.Multiply(b.Vector, a))

    static member (/)(a: Vector3, b: Vector3) =
        Vector3(Vector.Divide(a.Vector, b.Vector))

    static member (/)(a: Vector3, b: float) = a * (1.0 / b)

    member this.Item
        with get (i: int) =
            match i with
            | 0 -> this.X
            | 1 -> this.Y
            | 2 -> this.Z
            | _ -> raise <| IndexOutOfRangeException()

module Vector3 =
    let create x y z = Vector3(x, y, z)

    let negate (v: Vector3) = v * -1.0

    let lengthSquared (v: Vector3) =
        Vector.LengthSquared3D v.Vector
        |> Vector256.ToScalar

    let length (v: Vector3) =
        Vector.Length3D v.Vector |> Vector256.ToScalar

    let dot (u: Vector3) (v: Vector3) =
        Vector.DotProduct3D(u.Vector, v.Vector)
        |> Vector256.ToScalar

    let cross (u: Vector3) (v: Vector3) =
        Vector3(Vector.CrossProduct3D(u.Vector, v.Vector))

    let normalize (v: Vector3) = Vector3(Vector.Normalize3D v.Vector)

    let lerp (u: Vector3) (v: Vector3) (weight: float) =
        Vector3(Vector.Lerp(u.Vector, v.Vector, weight))


type Point3 = Vector3

type Color = Vector3

module Color =
    let toSKColor (color: Color) =
        let scaled = color * 255.999
        SKColor(byte scaled.X, byte scaled.Y, byte scaled.Z)
    
    let write (x, y) (surface: SKBitmap) sampleCount (color: Color) =
        let scale = 1.0 / (sampleCount |> float)
        let scaledColor = scale * color
        
        surface.SetPixel(x, y, toSKColor scaledColor)
        
        