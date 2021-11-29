module Raytracer.RenderMonitor

type RenderState =
    | Running
    | Stopping
    | Stopped

type RenderMonitor() =
    let mutable state = Stopped
    let mutable progress = 0.0
    
    member this.Start () =
        state <- Running
        progress <- 0.0
    
    member this.Stop () =
        state <- Stopping
    
    member this.Finish () =
        state <- Stopped
        
    member this.GetState() =
        state