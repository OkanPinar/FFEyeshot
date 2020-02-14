using devDept.Geometry;

using System;
using System.Collections.Generic;

using FFEyeshot.Common;

namespace FFEyeshot.Entity
{
    /// <summary>
    /// Point3D with transformation notifications
    /// </summary>
    public class PointT :devDept.Geometry.Point3D, INotifyTransformation
    {

        /// <summary>
        /// Transformation event for entities. Required entities should be register that event.
        /// </summary>
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
            base.TransformBy(xform);
            OnTransformed?.Invoke(this, new TransformingEventArgs(xform));
        }
        
        public void NotifyTransformation(object sender, TransformingEventArgs e)
        {
            if (this != (PointT)sender)
            {
                this.TransformBy(e.TData);
            }
        }
    }

    public class PointTGroup: List<PointT>
    {
        public PointTGroup(): base()
        {

        }

        public PointTGroup(IEnumerable<PointT> collection): base(collection)
        {

        }

        public void TransformBy(Transformation xform)
        {
            for (int i = 0; i < Count - 1; i++)
            {
                var P = new Point3D(this[i].X, this[i].Y, this[i].Z);
                P.TransformBy(xform);
                this[i].X = P.X;
                this[i].Y = P.Y;
                this[i].Z = P.Z;
            }
            this[Count - 1].TransformBy(xform);
        }

        public Point3D Average()
        {
            return this.ToArray().AveragePoint();
        }
    }
}
