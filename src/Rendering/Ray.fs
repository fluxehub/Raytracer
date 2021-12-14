namespace Raytracer.Rendering

open Raytracer.Rendering

type Ray = { Origin: Point3; Direction: Vector3 }

module Ray =
    let at (t: float) ray = ray.Origin + ray.Direction * t
