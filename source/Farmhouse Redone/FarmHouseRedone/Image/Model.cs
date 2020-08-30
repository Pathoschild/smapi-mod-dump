using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Image
{
    public class Model
    {
        public double radius;
        public Vector3 position;

        public Model(double radius, Vector3 position)
        {
            this.radius = radius;
            this.position = position;
        }

        public Model(double radius, Vector2 position)
        {
            this.radius = radius;
            this.position = new Vector3(position.X, position.Y, 0);
        }

        public bool rayHits(Vector2 rayPosition)
        {
            double distanceToVolumePoint = Math.Sqrt(Math.Pow(rayPosition.X - position.X, 2) + Math.Pow(rayPosition.Y - position.Y, 2));
            return distanceToVolumePoint <= radius;
        }

        public int getLuminenceDiffuse(Vector2 renderRay, Vector3 sunRay)
        {
            Vector3 positionOnSphere = getSurfacePoint(renderRay);

            //return (int)((-positionOnSphere.Y / (radius * 2)) * 255) + 128;

            //return (int)((positionOnSphere.Z / (radius * 2)) * 255) + 128;

            double angleY = Math.Atan((positionOnSphere.Z) / (-positionOnSphere.Y));

            return (int)(((angleY + (Math.PI / 2)) / Math.PI) * 255);
        }

        //Gets the surface angle of a point on the surface of a volume sphere
        public void getSurfaceAngle(Vector2 renderRay)
        {
            Vector3 positionOnSphere = getSurfacePoint(renderRay);

        }

        public Vector3 getSurfacePoint(Vector2 renderRay)
        {

            //minorRadius represents the radius of the circle drawn on the surface of the sphere found at X and drawn along Y.
            double minorRadius = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(renderRay.X - position.X, 2));
            double z = Math.Sqrt(Math.Pow(minorRadius, 2) - Math.Pow(renderRay.Y - position.Y, 2));

            return new Vector3(renderRay.X - position.X, renderRay.Y - position.Y, (float)z);
        }

        public double getHeight(Vector2 renderRay)
        {
            double minorRadius = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(renderRay.X - position.X, 2));
            return Math.Sqrt(Math.Pow(minorRadius, 2) - Math.Pow(renderRay.Y - position.Y, 2));
        }
    }
}
