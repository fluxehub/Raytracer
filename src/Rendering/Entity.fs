namespace Raytracer.Rendering

open Raytracer.Rendering

type Center = Point3
type Radius = float

type HitResult =
    { P: Point3
      Normal: Vector3
      T: float
      FrontFace: bool }

type Entity = Sphere of Center * Radius

module Entity =
    let private createHitResult (ray: Ray) outwardNormal p t =
        let frontFace = (Vector3.dot ray.Direction outwardNormal) < 0.0
        let normal = if frontFace then outwardNormal else -outwardNormal
        { P = p; T = t; Normal = normal; FrontFace = frontFace }
        
    let hit (r: Ray) tMin tMax =
        function
        | Sphere (center, radius) ->
            let oc = r.Origin - center
            let a = Vector3.lengthSquared r.Direction
            let halfB = Vector3.dot oc r.Direction
            let c = (Vector3.lengthSquared oc) - radius ** 2.0

            let d = halfB ** 2.0 - a * c

            if (d < 0.0) then
                None
            else
                let sqrtD = sqrt d

                let roots =
                    [ (-halfB - sqrtD) / a
                      (-halfB + sqrtD) / a ]
                    |> List.filter (fun r -> r > tMin && r < tMax)

                if List.length roots = 0 then
                    None
                else
                    let t = roots.[0]
                    let p = r |> Ray.at t
                    let outwardNormal = (p - center) / radius
                    Some <| createHitResult r outwardNormal p t
    
    let hitList r tMin tMax list =
        let hits = List.map (hit r tMin tMax) list |> List.choose id
        
        if List.length hits = 0 then
            None
        else
            Some <| List.minBy (fun res -> res.T) hits