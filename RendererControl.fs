module Raytracer.RendererControl

open System.Threading
open Avalonia.Layout
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Raytracer
open SkiaSharp
open Viewport
open Raytracer.Mutex
open Raytracer.Rendering

type State = {
    running: bool
    viewportWidth: int
    viewportHeight: int
    surfaceMutex: SurfaceMutex
    cancellationToken: CancellationTokenSource
}

let init =
    { running = false
      viewportWidth = 1024
      viewportHeight = 512
      surfaceMutex = SurfaceMutex(new SKBitmap (1024, 512, SKColorType.Bgra8888, SKAlphaType.Unpremul))
      cancellationToken = new CancellationTokenSource () }

type Msg = Start | Stop

let render surface =
    async {
        Renderer.render surface
        printfn "Done!"
    }

let update (msg: Msg) (state: State) : State =
    match msg with
    | Start ->
        state.surfaceMutex.Lock () |> ignore
        
        let surface = state.surfaceMutex.Surface
        surface.Dispose ()
        
        let surface = new SKBitmap (1024, 512, SKColorType.Bgra8888, SKAlphaType.Unpremul)
        state.surfaceMutex.Surface <- surface
        render surface |> Async.Start
        state.surfaceMutex.Unlock ()
        
        { state with running = true }
    | Stop ->
        { state with running = false; }

let view (state: State) dispatch =
    DockPanel.create [
        DockPanel.children [
            StackPanel.create [
                StackPanel.dock Dock.Top
                StackPanel.spacing 8.0
                StackPanel.margin (8.0, 5.0)
                StackPanel.orientation Orientation.Horizontal
                StackPanel.children [
                    Button.create [
                        Button.classes [ "Primary" ]
                        Button.content "Start"
                        Button.isEnabled (not state.running)
                        Button.onClick (fun _ -> dispatch Start)
                    ]
                    
                    Button.create [
                        Button.content "Stop"
                        Button.isEnabled state.running
                        Button.onClick (fun _ -> dispatch Stop)
                    ]
                ]
            ]
            
            Border.create [
                Border.borderBrush "Black" 
                Border.borderThickness 1.0
                Border.height (float state.viewportHeight)
                Border.width (float state.viewportWidth)
                Border.child (Viewport.create [
                    Viewport.surface state.surfaceMutex
                ])
            ]
        ]
    ]