namespace Raytracer

open System
open System.Threading
open SkiaSharp

type SurfaceLock(surface: SKBitmap) =
    static let mutex = new Mutex()
    member val Surface = surface with get, set
    
    member this.Lock() = mutex.WaitOne()
    member this.Unlock() = mutex.ReleaseMutex()
    