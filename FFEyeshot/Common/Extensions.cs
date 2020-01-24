using System;
using System.Collections.Generic;
using System.Text;
using devDept.Geometry;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;

namespace FFEyeshot.Common
{
    public static class Extensions
    {
        public static Point3D ProjectAt(this Plane me, Point3D pnt)
        {
            if (me is null)
            {
                throw new ArgumentNullException(nameof(me));
            }

            if (pnt is null)
            {
                throw new ArgumentNullException(nameof(pnt));
            }

            return me.PointAt(me.Project(pnt));
        }

        public static double ToRadian(this double me)
        {
            return Math.PI * me / 180.0;
        }

        public static Vector3D[] GetFrameVectors(this Vector3D me, Point3D pi, Point3D pj)
        {
            Vector3D v1, v2, v3;
            v1 = new Vector3D(pi, pj);
            v1.Normalize();

            if (Math.Abs(Vector3D.Dot(v1, Vector3D.AxisZ)) < 0.998)
            {
                v3 = Vector3D.Cross(v1, Vector3D.AxisZ);
                v3.Normalize();

                v2 = Vector3D.Cross(v3, v1);
                v2.Normalize();
            }

            else
            {
                v2 = Vector3D.AxisY;
                v3 = Vector3D.Cross(v1, v2);
                v3.Normalize();
            }

            return new Vector3D[] { v1, v2, v3 };
        }
    
        public static Region FromAutocad(this Region me, string path)
        {
            ReadAutodesk ra = new ReadAutodesk(path);
            ra.DoWork();
            var curves = new List<ICurve>();

            foreach (var entity in ra.Entities)
            {
                if (entity is ICurve curve)
                {
                    curves.Add(curve);
                }
            }

            return new Region(curves.ToArray());
        }
    }
}
