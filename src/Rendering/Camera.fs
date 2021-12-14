namespace Raytracer.Rendering

open Raytracer.Rendering

type Camera =
    { ViewportHeight: double
      ViewportWidth: double
      FocalLength: double
      Origin: Point3
      Horizontal: Vector3
      Vertical: Vector3
      LowerLeftCorner: Vector3 }

module Camera =
    let create height width focalLength (origin: Point3) =
        let horizontal = Vector3.create width 0.0 0.0
        let vertical = Vector3.create 0.0 height 0.0

        let lowerLeftCorner =
            origin
            - horizontal / 2.0
            - vertical / 2.0
            - Vector3.create 0.0 0.0 focalLength

        { ViewportWidth = width
          ViewportHeight = height
          FocalLength = focalLength
          Origin = origin
          Horizontal = horizontal
          Vertical = vertical
          LowerLeftCorner = lowerLeftCorner }

    let getRay (u: float, v: float) camera : Ray =
        { Origin = camera.Origin
          Direction =
            camera.LowerLeftCorner
            + u * camera.Horizontal
            + v * camera.Vertical
            - camera.Origin }
