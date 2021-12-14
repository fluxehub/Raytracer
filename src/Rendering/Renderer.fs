module Raytracer.Rendering.Renderer

open System
open System.Runtime.InteropServices
open System.Security.Cryptography
open Raytracer.RenderMonitor
open Raytracer.Rendering
open SkiaSharp

let rand = System.Random()

let swap x y (a: _ array) =
    let tmp = a.[x]
    a.[x] <- a.[y]
    a.[y] <- tmp

let shuffle a =
    Array.iteri (fun i _ -> a |> swap i (rand.Next(i, Array.length a))) a

let hitSphere (center: Point3) radius (ray: Ray) =
    let oc = ray.Origin - center
    let a = Vector3.lengthSquared ray.Direction
    let halfB = Vector3.dot oc ray.Direction
    let c = (Vector3.lengthSquared oc) - radius ** 2.0
    let d = halfB ** 2.0 - a * c
    if d < 0.0 then
        -1.0
    else
        -halfB - sqrt(d) / a

let drawBackground ray =
    let normalizedDirection = Vector3.normalize ray.Direction
    let t = 0.5 * (normalizedDirection.Y + 1.0)
    Vector3.lerp (Vector3.create 1.0 1.0 1.0) (Vector3.create 0.5 0.7 1.0) t

let rayColor entities (ray: Ray): Color =
    match Entity.hitList ray 0.0 infinity entities with
    | Some hit -> 0.5 * (hit.Normal + Vector3.create 1.0 1.0 1.0)
    | None -> drawBackground ray
let renderSample entities camera (u, v) =
    Camera.getRay (u, v) camera |> rayColor entities

let renderPixel (surface: SKBitmap) (camera: Camera) (monitor: RenderMonitor) (x, y) =
    async {
        if monitor.GetState() <> Stopping then
            let random = Random()

            let width = surface.Width |> float
            let height = surface.Height |> float

            let sampleCount = 50

            let entities =
                [ Sphere(Vector3.create 0.0 0.0 -1.0, 0.5)
                  Sphere(Vector3.create 0.0 -100.5 -1.0, 100.0) ]

            let samples =
                [ for _ in 0 .. sampleCount - 1 do
                      let u =
                          ((float x) + random.NextDouble()) / (width - 1.0)
                          |> float

                      let v =
                          ((float y) + random.NextDouble()) / (height - 1.0)
                          |> float

                      (u, v) ]

            let color =
                samples
                |> List.map (renderSample entities camera)
                |> List.fold (+) (Vector3.create 0.0 0.0 0.0)

            color
            |> Color.write (x, (int height) - y - 1) surface sampleCount

            monitor.AddRendered()
    }

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
