using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class SphereCollider : Collider
    {
        public float Radius { get; set; }

        public override bool Collides(Collider other, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;
                if ((Transform.Position - collider.Transform.Position).LengthSquared() < Math.Pow(Radius + collider.Radius, 2))
                {
                    normal = Vector3.Normalize
                        (Transform.Position - collider.Transform.Position);
                    return true;
                }
            }
            else if (other is BoxCollider) return other.Collides(this, out normal);

            return base.Collides(other, out normal);
        }

        public bool SweptCollides(Collider other, Vector3 otherLastPosition, Vector3 lastPosition, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;

                Vector3 vp = Transform.Position - lastPosition;
                Vector3 vq = collider.Transform.Position - otherLastPosition;

                Vector3 A = lastPosition - otherLastPosition;
                Vector3 B = vp - vq;

                float a = Vector3.Dot(B, B);
                float b = 2 * Vector3.Dot(A, B);
                float c = -((Vector3.Dot(A, B) * Vector3.Dot(A, B)) / Vector3.Dot(B, B));
                float disc = b * b - 4 * a * c; // discriminant (b^2 – 4ac)

                if (disc >= 0)
                {
                    float t = (-Vector3.Dot(A, B) - (float)Math.Sqrt(disc)) / Vector3.Dot(B, B);
                    Vector3 p = lastPosition + t * vp;
                    Vector3 q = otherLastPosition + t * vq;
                    Vector3 intersect = Vector3.Lerp(p, q, this.Radius / (this.Radius + collider.Radius));
                    normal = Vector3.Normalize(p - q);
                    return true;
                }
            }

            else if (other is BoxCollider)
                return other.Collides(this, out normal);
            return base.Collides(other, out normal);
        }

        public override float? Intersects(Ray ray)
        {
            Matrix worldInv = Matrix.Invert(Transform.World);
            ray.Position = Vector3.Transform(ray.Position, worldInv);
            ray.Direction = Vector3.TransformNormal(ray.Direction, worldInv);
            float length = ray.Direction.Length();
            ray.Direction /= length; // same as normalization
            float? p = new BoundingSphere(Vector3.Zero, Radius).Intersects(ray);
            if (p != null)
                return (float)p * length;
            return null;
        }
    }
}
