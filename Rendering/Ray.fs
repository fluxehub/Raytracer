namespace Raytracer.Rendering

open Raytracer.Rendering
open Vector
open MathSharp

type Ray =
    { Origin: Point3
      Direction: Vector3 }
    
module Ray =
    let at t ray =
        ray.Origin + ray.Direction * t