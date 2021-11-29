namespace Raytracer.Rendering

open Raytracer.Rendering
open Vector

type Ray =
    { Origin: Point3
      Direction: Vec3 }
    
module Ray =
    let at t ray =
        ray.Origin + ray.Direction * t