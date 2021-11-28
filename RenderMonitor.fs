module Raytracer.RenderMonitor

type RenderState =
    | Running
    | Stopping
    | Stopped

type RenderMonitor() =
    member val State: RenderState = Stopped with get, set
    member val Progress = 0.0 with get, set