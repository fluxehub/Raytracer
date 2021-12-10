module Raytracer.Viewport

open Avalonia
open Avalonia.FuncUI.Builder
open Avalonia.Controls
open Avalonia.Rendering.SceneGraph
open Avalonia.Skia
open Avalonia.Threading
open Raytracer.Mutex
open SkiaSharp

type Viewport() =
    inherit Control(ClipToBounds = true)
    
    let mutable viewport = Rect ()
    let mutable surfaceMutex = Unchecked.defaultof<SurfaceMutex>
    
    member this.SurfaceMutex
        with get () = surfaceMutex
        and set value =
            this.SetAndRaise(Viewport.SurfaceMutexProperty, &surfaceMutex, value) |> ignore
            viewport <- Rect (Size (float surfaceMutex.Surface.Width, float surfaceMutex.Surface.Height))
    
    static member SurfaceMutexProperty: DirectProperty<Viewport, SurfaceMutex> =
        AvaloniaProperty.RegisterDirect<Viewport, SurfaceMutex>(
            nameof Unchecked.defaultof<Viewport>.SurfaceMutex,
            (fun o -> o.SurfaceMutex),
            (fun o v -> o.SurfaceMutex <- v))
        
    override this.Render(context) =
        let dest = this.Bounds.CenterRect(viewport)
        context.Custom(new DrawRender(dest, surfaceMutex))
        Dispatcher.UIThread.InvokeAsync(this.InvalidateVisual, DispatcherPriority.Background) |> ignore

and DrawRender(bounds: Rect, surfaceMutex: SurfaceMutex) =
    interface ICustomDrawOperation with
        member this.get_Bounds() = bounds
        member this.HitTest _ = false
        member this.Equals _ = false
        member this.Render(context) =
            let canvas = (context :?> ISkiaDrawingContextImpl).SkCanvas

            let dest = SKRect.Create(bounds.X |> float32, bounds.Y |> float32, bounds.Width |> float32, bounds.Height |> float32)
            surfaceMutex.Lock () |> ignore
            canvas.DrawBitmap(surfaceMutex.Surface, dest)
            surfaceMutex.Unlock ()
            
        member this.Dispose () = ()
        

type Viewport with
    static member create attrs =
        ViewBuilder.Create<Viewport>(attrs)
        
    static member surface<'t when 't :> Viewport>(value: SurfaceMutex) =
        AttrBuilder<'t>.CreateProperty<SurfaceMutex>(Viewport.SurfaceMutexProperty, value, ValueNone)
    