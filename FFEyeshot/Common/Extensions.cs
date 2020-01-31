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

        /*public static Point3D AveragePoint(this Entity.PointT[] pnts)
        {
            var averagePoint = Point3D.Origin;
            foreach (var point in pnts)
            {
                averagePoint += point;
            }

            averagePoint /= pnts.Length;

            return averagePoint;
        }*/

        public static Point3D AveragePoint(this Point3D[] pnts)
        {
            var averagePoint = Point3D.Origin;
            foreach (var point in pnts)
            {
                averagePoint += point;
            }

            averagePoint /= pnts.Length;

            return averagePoint;
        }

        public static Point3D[] offsetRegion(this Point3D[] pnts, Vector3D surfaceNormal,double[] offset)
        {
            var averagePoint = pnts.AveragePoint();

            var offsetLines = new Line[pnts.Length];
            var intersectionPoints = new Point3D[pnts.Length];

            for (int i = 0; i < pnts.Length; i++)
            {
                var pi = pnts[i];
                var pj = pnts[i == pnts.Length - 1 ? 0 : i + 1];

                var v1 = new Vector3D(pi, pj);
                v1.Normalize();

                var line = new Line(pi, pj);
                var lineOffset = (Line)line.Offset(offset[i], surfaceNormal);

                var dist1 = averagePoint.DistanceTo(pi);
                var dist2 = averagePoint.DistanceTo(lineOffset.StartPoint);

                if (dist2 > dist1)
                {
                    lineOffset = (Line)line.Offset(-offset[i], surfaceNormal);
                }
                lineOffset.StartPoint.TransformBy(new Translation(v1 * -lineOffset.Length() * 0.3));
                lineOffset.EndPoint.TransformBy(new Translation(v1 * lineOffset.Length() * 0.3));
                offsetLines[i] = lineOffset;
            }

            for (int i = 0; i < pnts.Length; i++)
            {
                var linei = offsetLines[i == 0 ? pnts.Length - 1 : i - 1];
                var linej = offsetLines[i];
                var intersectionPoint = linei.IntersectWith(linej)[0];
                intersectionPoints[i] = intersectionPoint;
            }

            return intersectionPoints;
        }

        public static bool IsEventHandlerRegistered(this EventHandler me, Delegate prospectiveHandler)
        {
            if (me != null)
            {
                foreach (Delegate existingHandler in me.GetInvocationList())
                {
                    if (existingHandler == prospectiveHandler)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
