﻿using System;
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
            var projected = me.Project(pnt);
            var ret = me.PointAt(projected);

            return ret;
        }
    }
}
