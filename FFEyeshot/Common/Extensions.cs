using System;
using System.Collections.Generic;
using System.Text;
using devDept.Geometry;

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
    }
}
