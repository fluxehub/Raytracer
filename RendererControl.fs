module Raytracer.RendererControl

open System.Threading
open Avalonia.Layout
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Raytracer
open Raytracer.RenderMonitor
open SkiaSharp
open Viewport
open Raytracer.Mutex
open Raytracer.Rendering

type State = {
    viewportWidth: int
    viewportHeight: int
    surfaceMutex: SurfaceMutex
    renderState: RenderState
    renderMonitor: RenderMonitor
}

let init =
    { 
      viewportWidth = 256
      viewportHeight = 256
      surfaceMutex = SurfaceMutex(new SKBitmap (256, 256, SKColorType.Bgra8888, SKAlphaType.Unpremul))
      renderState = Stopped
      renderMonitor = RenderMonitor () }

type Msg = Start | Stop

// TODO: Dispatch the stopped message
let render surface renderMonitor =
    async {
        let points = Renderer.getRenderPoints surface
        points
        |> Array.map (fun (x, y) -> async { Renderer.renderPixel (x, y) surface renderMonitor } )
        |> Async.Parallel
        |> Async.Ignore
        |> Async.RunSynchronously
        
        renderMonitor.State <- Stopped
    }

let update (msg: Msg) (state: State) : State =
    match msg with
    | Start ->
        state.surfaceMutex.Lock () |> ignore
        
        let surface = state.surfaceMutex.Surface
        surface.Dispose ()
        
        let surface = new SKBitmap (state.viewportWidth, state.viewportHeight, SKColorType.Bgra8888, SKAlphaType.Unpremul)
        state.surfaceMutex.Surface <- surface
        
        state.renderMonitor.State <- Running
        
        // Clear the surface
        Renderer.clear surface
        
        // Start rendering
        render surface state.renderMonitor
        |> Async.Start
        
        state.surfaceMutex.Unlock ()
        
        { state with renderState = Running }
    | Stop ->
        state.renderMonitor.State <- Stopping
        { state with renderState = Stopped }

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
                        Button.isEnabled (state.renderState = Stopped)
                        Button.onClick (fun _ -> dispatch Start)
                    ]
                    
                    Button.create [
                        Button.content "Stop"
                        Button.isEnabled (state.renderState = Running)
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