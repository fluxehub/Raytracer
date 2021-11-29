namespace Raytracing.Rendering

open Raytracer.Rendering.Vector

type Camera =
    { ViewportHeight: double
      ViewportWidth: double
      FocalLength: double
      Origin: Point3
      Horizontal: Vec3
      Vertical: Vec3
      LowerLeftCorner: Vec3 }

module Camera =
    let create height width focalLength origin =
        let horizontal = Vec3.create width 0.0 0.0
        let vertical = Vec3.create 0.0 height 0.0
        let lowerLeftCorner = origin - horizontal / 2.0 - vertical / 2.0 - Vec3.create 0.0 0.0 focalLength
        
        { ViewportWidth = width
          ViewportHeight = height
          FocalLength = focalLength
          Origin = origin
          Horizontal = horizontal
          Vertical = vertical
          LowerLeftCorner = lowerLeftCorner }
