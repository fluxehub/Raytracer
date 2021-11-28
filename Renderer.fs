module Raytracer.Rendering.Renderer

open System.Threading
open Raytracer
open Raytracer.RenderMonitor
open SkiaSharp

let rand = new System.Random()

let swap x y (a: _ array) =
    let tmp = a.[x]
    a.[x] <- a.[y]
    a.[y] <- tmp

let shuffle a = Array.iteri (fun i _ -> a |> swap i (rand.Next(i, Array.length a))) a

let clearPoint (x, y) (surface: SKBitmap) =
    surface.SetPixel (x, y, SKColors.Black)

let renderPixel (x, y) (surface: SKBitmap) (monitor: RenderMonitor) =
    if monitor.State = Stopping then
        ()
    else
        let width = surface.Width
        let height = surface.Height
        
        let r = (double x) / double (width - 1)
        let g = (double y) / double (height - 1)
        let b = 0.25
        
        let ir = byte (r * 255.999)
        let ig = byte (g * 255.999)
        let ib = byte (b * 255.999)
        
        let color = SKColor (ir, ig, ib, 0xFFuy)
        surface.SetPixel (x, height - 1 - y, color)
        Thread.Sleep 1
        ()

let getRenderPoints (surface: SKBitmap) =
    let width = surface.Width
    let height = surface.Height

    let points = [| for j in 0..(height - 1) do for i in 0..(width - 1) do (i, j) |]
    shuffle points
    points

let clear (surface: SKBitmap) =
    Array.iter (fun (x, y) -> clearPoint (x, y) surface) (getRenderPoints surface)