module Raytracer.RenderMonitor

open Elmish
open System.Threading

type RenderState =
    | Running
    | Stopping
    | Stopped

type RenderMonitor() =
    let mutable state = Stopped
    let mutable rendered = 0
    static let mutex = new Mutex()
    let mutable total = 0
    let mutable dispatch = Unchecked.defaultof<Dispatch<float>>

    member this.SetDispatch newDispatch = dispatch <- newDispatch

    member this.AddRendered() =
        mutex.WaitOne() |> ignore
        rendered <- rendered + 1
        mutex.ReleaseMutex()

        dispatch
        <| System.Math.Round(((double rendered) / (double total) * 100.0), 2)

    member this.SetTotal value = total <- value

    member this.Start() =
        state <- Running
        rendered <- 0
        dispatch 0.0

    member this.Stop() = state <- Stopping

    member this.Finish() = state <- Stopped

    member this.GetState() = state
