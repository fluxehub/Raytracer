module Raytracer.Rendering.Renderer

open Raytracer.RenderMonitor
open Raytracer.Rendering
open Raytracing.Rendering
open Vector
open SkiaSharp

let rand = System.Random()

let swap x y (a: _ array) =
    let tmp = a.[x]
    a.[x] <- a.[y]
    a.[y] <- tmp

let shuffle a = Array.iteri (fun i _ -> a |> swap i (rand.Next(i, Array.length a))) a

let clearPoint (x, y) (surface: SKBitmap) =
    surface.SetPixel (x, y, SKColors.Black)

let rayColor (ray: Ray) =
    let normalizedDirection = Vec3.normalize ray.Direction
    let t = 0.5 * (normalizedDirection.Y + 1.0)
    (1.0 - t) * { X = 1.0; Y = 1.0; Z = 1.0 } + t * { X = 0.5; Y = 0.7; Z = 1.0 }

let renderPixel (x, y) (surface: SKBitmap) (camera: Camera) (monitor: RenderMonitor) =
    if monitor.GetState() <> Stopping then
        let width = surface.Width
        let height = surface.Height
        
        let u = (double x) / double (width - 1)
        let v = (double y) / double (height - 1)
        let ray =
            { Origin = camera.Origin
              Direction = camera.LowerLeftCorner + u * camera.Horizontal + v * camera.Vertical }
        
        let color: Color = rayColor ray
        
        surface.SetPixel (x, height - 1 - y, Color.toSKColor color)
        monitor.AddRendered ()
        
let getRenderPoints (surface: SKBitmap) =
    let width = surface.Width
    let height = surface.Height

    let points = [| for j in 0..(height - 1) do for i in 0..(width - 1) do (i, j) |]
    shuffle points
    points

let clear (surface: SKBitmap) =
    Array.iter (fun (x, y) -> clearPoint (x, y) surface) (getRenderPoints surface)