module Raytracer.RendererControl

open System.Threading
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Elmish
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
    progress: float
    renderMonitor: RenderMonitor
}

let init width height =
    { 
      viewportWidth = width
      viewportHeight = height
      surfaceMutex = SurfaceMutex(new SKBitmap (width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul))
      renderState = Stopped
      progress = 0.0
      renderMonitor = RenderMonitor () }

type Msg = Start | Stop | Finish | Progress of float

// TODO: Dispatch the stopped message
let render surface renderMonitor =
    async {
        let points = Renderer.getRenderPoints surface
        points
        |> Array.mapi (fun i (x, y) -> async { Renderer.renderPixel (x, y) i (Array.length points) surface renderMonitor } )
        |> Async.Parallel
        |> Async.Ignore
        |> Async.RunSynchronously
        
        renderMonitor.Finish()
        return Finish
    }

let update (msg: Msg) (state: State) =
    match msg with
    | Start ->
        state.surfaceMutex.Lock () |> ignore
        
        let surface = state.surfaceMutex.Surface
        surface.Dispose ()
        
        let surface = new SKBitmap (state.viewportWidth, state.viewportHeight, SKColorType.Bgra8888, SKAlphaType.Unpremul)
        state.surfaceMutex.Surface <- surface
        
        state.renderMonitor.Start ()
        
        // Clear the surface
        Renderer.clear surface
        
        state.surfaceMutex.Unlock ()
        
        state.renderMonitor.SetTotal <| state.viewportWidth * state.viewportHeight
        
        { state with renderState = Running }, Cmd.OfAsync.result (render surface state.renderMonitor)
    | Stop ->
        state.renderMonitor.Stop ()
        { state with renderState = Stopping }, Cmd.none
    | Finish ->
        { state with renderState = Stopped }, Cmd.none
    | Progress amount ->
        { state with progress = amount }, Cmd.none

let view (state: State) (dispatch: Dispatch<Msg>) =
    state.renderMonitor.SetDispatch (fun amount -> dispatch (Progress amount))
    DockPanel.create [
        DockPanel.children [
            StackPanel.create [
                StackPanel.dock Dock.Top
                StackPanel.spacing 8.0
                StackPanel.margin (8.0, 5.0)
                StackPanel.orientation Orientation.Horizontal
                StackPanel.children ([
                    Button.create [
                        Button.content "Start"
                        Button.isEnabled (state.renderState = Stopped)
                        Button.onClick (fun _ -> dispatch Start)
                    ]
                    
                    Button.create [
                        Button.content "Stop"
                        Button.isEnabled (state.renderState = Running)
                        Button.onClick (fun _ -> dispatch Stop)
                    ]
                ] @ [if state.renderState = Running then
                        ProgressBar.create [
                            ProgressBar.value state.progress
                        ]])
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