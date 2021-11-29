module Raytracer.Rendering.Renderer

open System.Threading
open Raytracer
open Raytracer.RenderMonitor
open Raytracer.Renderer
open Types
open SkiaSharp

let rand = new System.Random()

let swap x y (a: _ array) =
    let tmp = a.[x]
    a.[x] <- a.[y]
    a.[y] <- tmp

let shuffle a = Array.iteri (fun i _ -> a |> swap i (rand.Next(i, Array.length a))) a

let clearPoint (x, y) (surface: SKBitmap) =
    surface.SetPixel (x, y, SKColors.Black)

let renderPixel (x, y) i totalPoints (surface: SKBitmap) (monitor: RenderMonitor) =
    if monitor.GetState() <> Stopping then
        let width = surface.Width
        let height = surface.Height
        
        let color: Color = { X = (double x) / double (width - 1); Y = (double y) / double (height - 1); Z = 0.25 }
        
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