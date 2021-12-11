module Raytracer.Rendering.Renderer

open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
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

let shuffle a =
    Array.iteri (fun i _ -> a |> swap i (rand.Next(i, Array.length a))) a

let rayColor (ray: Ray) =
    let normalizedDirection = Vector3.normalize ray.Direction
    let t = 0.5 * (normalizedDirection.Y + 1.0)
    Vector3.lerp (Vector3.create 1.0 1.0 1.0) (Vector3.create 0.5 0.7 1.0) t

let renderPixel (x, y) (surface: SKBitmap) (camera: Camera) (monitor: RenderMonitor) =
    if monitor.GetState() = Stopping then
        ()
     
    let width = surface.Width
    let height = surface.Height

    let u = (double x) / double (width - 1)
    let v = (double y) / double (height - 1)

    let ray =
        { Origin = camera.Origin
          Direction =
            camera.LowerLeftCorner
            + u * camera.Horizontal
            + v * camera.Vertical }

    let color: Color = rayColor ray

    surface.SetPixel(x, height - 1 - y, Color.toSKColor color)
    monitor.AddRendered()

let getRenderPoints (surface: SKBitmap) =
    let width = surface.Width
    let height = surface.Height

    let points =
        [| for j in 0 .. (height - 1) do
               for i in 0 .. (width - 1) do
                   (i, j) |]

    shuffle points
    points

let clear (surface: SKBitmap) =
    let pixelPtr = surface.GetPixels()

    let cleared =
        [| for i in
               0 .. (surface.Width
                     * surface.Height
                     * surface.BytesPerPixel
                     - 1) do
               if i % 4 = 3 then 0xFFuy else 0x00uy |]

    Marshal.Copy(cleared, 0, pixelPtr, (Array.length cleared))
