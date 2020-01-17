using devDept.Geometry;

using System;

using FFEyeshot.Common;

namespace FFEyeshot.Entity
{
    public class PointT :devDept.Geometry.Point3D, Common.INotifyTransformation
    {
        public event TransformationEventHandler OnTransforming;

        public PointT(): base()
        {

        }

        public PointT(double x, double y, double z): base(x, y, z)
        {

        }

        public PointT(devDept.Geometry.Point3D another): base(another)
        {

        }

        public PointT(PointT another): base(another.X, another.Y, another.Z)
        {

        }

        public override void TransformBy(Transformation xform)
        {
            OnTransforming?.Invoke(this, new TransformationEventArgs(xform));
            base.TransformBy(xform);
        }
    }
}
