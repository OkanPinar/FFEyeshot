using devDept.Geometry;

using System;

using FFEyeshot.Common;

namespace FFEyeshot.Entity
{
    public class PointT :devDept.Geometry.Point3D, Common.INotifyTransformation
    {
        public event TransformingEventHandler OnTransforming;
        public event TransformedEventHandler OnTransformed;

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
            var old = new PointT(this);
            OnTransforming?.Invoke(this, new TransformingEventArgs(xform));
            base.TransformBy(xform);
            OnTransformed?.Invoke(this, new TransformedEventArgs(old));
        }

        public void NotifyTransformation(object sender, TransformingEventArgs e)
        {
            if (sender != this)
            {
                this.TransformBy(e.TData);
            }
        }
    }
}
