using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;
using devDept.Graphics;
using devDept.Eyeshot.Entities;
using System.Windows.Input;

namespace FFEyeshot.ViewLay.ThreeD
{
    public class SnapPoint : Point3D
    {
        public ViewPort3D.SnapType Type;

        public SnapPoint()
            : base()
        {
            Type = ViewPort3D.SnapType.NONE;
        }

        public SnapPoint(Point3D point3D, ViewPort3D.SnapType SnapType) : base(point3D.X, point3D.Y, point3D.Z)
        {
            this.Type = SnapType;
        }

        public override string ToString()
        {
            return base.ToString() + " | " + Type;
        }
    }

    public class SnapEventArgs : EventArgs
    {
        public SnapEventArgs()
        {

        }
    }

    public delegate void SnapEventHandler(SnapEventArgs e);

    public partial class ViewPort3D
    {
        #region State

        private bool _isSnapEnable = true;

        public bool SnapEnable
        {
            get { return _isSnapEnable; }
            set {
                if (value != _isSnapEnable)
                {
                    OnSnapStateChanging(_isSnapEnable);
                    PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs("SnapEnable"));
                    _isSnapEnable = value;
                    OnSnapStateChanged(value);
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SnapEnable"));
                }
            }
        }
        private void OnSnapStateChanging(bool oldState)
        {

        }

        private void OnSnapStateChanged(bool newState)
        {

        }
       
        #endregion

        public enum SnapType
        {
            NONE,
            POINT,
            END,
            MID,
            CENTER,
            QUAD
        }

        public enum SnapMode
        {
            Wireframe,
            Solid
        }
        private SnapPoint FindClosestPoint(SnapPoint[] snapPoints)
        {
            double minDist = double.MaxValue;

            int i = 0;
            int index = 0;

            foreach (SnapPoint vertex in snapPoints)
            {
                Point3D vertexScreen = WorldToScreen(vertex);
                Point2D currentScreen = new Point2D(CurrentLocation.X, Size.Height - CurrentLocation.Y);

                double dist = Point2D.Distance(vertexScreen, currentScreen);

                if (dist < minDist)
                {
                    index = i;
                    minDist = dist;
                }

                i++;
            }

            SnapPoint snap = (SnapPoint)snapPoints.GetValue(index);
            //DisplaySnappedVertex(snap);

            return snap;
        }

        public List<SnapType> CurrentSnapTypes = new List<SnapType>() { SnapType.END, SnapType.CENTER, SnapType.MID };
        public List<SnapMode> CurrentSnapModes = new List<SnapMode>() { SnapMode.Wireframe, SnapMode.Solid };

        public static double snapSymbolSize { get; set; } = 20;

        public event SnapEventHandler Snapping;

        private void DisplaySnappedVertex(SnapPoint snap)
        {
            renderContext.SetLineSize(2);
            // white color
            renderContext.SetColorWireframe(System.Drawing.Color.Yellow);
            renderContext.SetState(depthStencilStateType.DepthTestOff);

            Point2D onScreen = WorldToScreen(snap);

            switch (snap.Type)
            {
                case SnapType.POINT:
                    DrawCircle(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    DrawCross(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapType.CENTER:
                    DrawCircle(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapType.END:
                    DrawQuad(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapType.MID:
                    DrawTriangle(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapType.QUAD:
                    renderContext.SetLineSize(3.0f);
                    DrawRhombus(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
            }

            renderContext.SetLineSize(1);
        }

        public void DrawCross(System.Drawing.Point onScreen)
        {
            double dim1 = onScreen.X + (snapSymbolSize / 2);
            double dim2 = onScreen.Y + (snapSymbolSize / 2);
            double dim3 = onScreen.X - (snapSymbolSize / 2);
            double dim4 = onScreen.Y - (snapSymbolSize / 2);

            Point3D topLeftVertex = new Point3D(dim3, dim2);
            Point3D topRightVertex = new Point3D(dim1, dim2);
            Point3D bottomRightVertex = new Point3D(dim1, dim4);
            Point3D bottomLeftVertex = new Point3D(dim3, dim4);

            renderContext.DrawLines(
                new Point3D[]
                {
                    bottomLeftVertex,
                    topRightVertex,

                    topLeftVertex,
                    bottomRightVertex,

                });
        }

        public void DrawCircle(System.Drawing.Point onScreen)
        {
            double radius = snapSymbolSize / 2;

            double x2 = 0, y2 = 0;

            List<Point3D> pts = new List<Point3D>();

            for (int angle = 0; angle < 360; angle += 10)
            {
                double rad_angle = Utility.DegToRad(angle);

                x2 = onScreen.X + radius * Math.Cos(rad_angle);
                y2 = onScreen.Y + radius * Math.Sin(rad_angle);

                Point3D circlePoint = new Point3D(x2, y2);
                pts.Add(circlePoint);
            }

            renderContext.DrawLineLoop(pts.ToArray());
        }

        public void DrawQuad(System.Drawing.Point onScreen)
        {
            double dim1 = onScreen.X + (snapSymbolSize / 2);
            double dim2 = onScreen.Y + (snapSymbolSize / 2);
            double dim3 = onScreen.X - (snapSymbolSize / 2);
            double dim4 = onScreen.Y - (snapSymbolSize / 2);

            Point3D topLeftVertex = new Point3D(dim3, dim2);
            Point3D topRightVertex = new Point3D(dim1, dim2);
            Point3D bottomRightVertex = new Point3D(dim1, dim4);
            Point3D bottomLeftVertex = new Point3D(dim3, dim4);

            renderContext.DrawLineLoop(new Point3D[]
            {
                bottomLeftVertex,
                bottomRightVertex,
                topRightVertex,
                topLeftVertex
            });
        }

        void DrawTriangle(System.Drawing.Point onScreen)
        {
            double dim1 = onScreen.X + (snapSymbolSize / 2);
            double dim2 = onScreen.Y + (snapSymbolSize / 2);
            double dim3 = onScreen.X - (snapSymbolSize / 2);
            double dim4 = onScreen.Y - (snapSymbolSize / 2);
            double dim5 = onScreen.X;

            Point3D topCenter = new Point3D(dim5, dim2);

            Point3D bottomRightVertex = new Point3D(dim1, dim4);
            Point3D bottomLeftVertex = new Point3D(dim3, dim4);

            renderContext.DrawLineLoop(new Point3D[]
            {
                bottomLeftVertex,
                bottomRightVertex,
                topCenter
            });
        }

        void DrawRhombus(System.Drawing.Point onScreen)
        {
            double dim1 = onScreen.X + (snapSymbolSize / 1.5);
            double dim2 = onScreen.Y + (snapSymbolSize / 1.5);
            double dim3 = onScreen.X - (snapSymbolSize / 1.5);
            double dim4 = onScreen.Y - (snapSymbolSize / 1.5);

            Point3D topVertex = new Point3D(onScreen.X, dim2);
            Point3D bottomVertex = new Point3D(onScreen.X, dim4);
            Point3D rightVertex = new Point3D(dim1, onScreen.Y);
            Point3D leftVertex = new Point3D(dim3, onScreen.Y);

            renderContext.DrawLineLoop(new Point3D[]
            {
                bottomVertex,
                rightVertex,
                topVertex,
                leftVertex,
            });
        }

        public SnapPoint[] GetSnapPoints()
        {
            //changed PickBoxSize to define a range for display snapPoints
            int oldSize = PickBoxSize;
            PickBoxSize = 10;

            PickBoxSize = oldSize;

            List<SnapPoint> snapPoints = new List<SnapPoint>();

            if (CurrentIndex != -1)
            {
                Entity ent = Entities[CurrentIndex];

                if (ent is Profiles.ProfileBase)
                {
                    Profiles.ProfileBase p = (Profiles.ProfileBase)ent;
                    foreach (var snapMode in CurrentSnapModes)
                    {
                        switch (snapMode)
                        {
                            case SnapMode.Wireframe:
                                snapPoints.Add(new SnapPoint(p.PI, SnapType.END));
                                snapPoints.Add(new SnapPoint((p.PI + p.PJ) / 2, SnapType.MID));
                                snapPoints.Add(new SnapPoint(p.PJ, SnapType.END));
                                break;
                            //case SnapMode.Solid:
                            //    var temp = p.GetSnapPoint();
                            //    temp.ForEach(item => snapPoints.Add(new SnapPoint(item, SnapType.QUAD)));
                            //    break;
                            default:
                                break;
                        }
                    }
                }

                else if (ent is devDept.Eyeshot.Entities.Point)
                {
                    devDept.Eyeshot.Entities.Point point = (devDept.Eyeshot.Entities.Point)ent;

                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.POINT:
                                Point3D point3d = point.Vertices[0];
                                snapPoints.Add(new SnapPoint(point3d, SnapType.POINT));
                                break;
                        }
                    }
                }
                else if (ent is Line) //line
                {
                    Line line = (Line)ent;

                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                snapPoints.Add(new SnapPoint(line.StartPoint, SnapType.END));
                                snapPoints.Add(new SnapPoint(line.EndPoint, SnapType.END));
                                break;
                            case SnapType.MID:
                                snapPoints.Add(new SnapPoint(line.MidPoint, SnapType.MID));
                                break;
                        }
                    }
                }
                else if (ent is LinearPath)//polyline
                {
                    LinearPath polyline = (LinearPath)ent;
                    List<SnapPoint> polyLineSnapPoints = new List<SnapPoint>();

                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                foreach (Point3D point in polyline.Vertices)
                                    polyLineSnapPoints.Add(new SnapPoint(point, SnapType.END));
                                snapPoints.AddRange(polyLineSnapPoints);
                                break;
                        }
                    }
                }
                else if (ent is CompositeCurve)//composite
                {
                    CompositeCurve composite = (CompositeCurve)ent;
                    List<SnapPoint> polyLineSnapPoints = new List<SnapPoint>();

                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                foreach (ICurve curveSeg in composite.CurveList)
                                    polyLineSnapPoints.Add(new SnapPoint(curveSeg.EndPoint, SnapType.END));
                                polyLineSnapPoints.Add(new SnapPoint(composite.CurveList[0].StartPoint, SnapType.END));
                                snapPoints.AddRange(polyLineSnapPoints);
                                break;
                        }
                    }
                }
                else if (ent is Arc) //Arc
                {
                    Arc arc = (Arc)ent;

                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(arc.StartPoint, SnapType.END),
                                                           new SnapPoint(arc.EndPoint, SnapType.END) });
                                break;
                            case SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(arc.MidPoint, SnapType.MID) });
                                break;
                            case SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(arc.Center, SnapType.CENTER) });
                                break;
                        }
                    }
                }
                else if (ent is Circle) //Circle
                {
                    Circle circle = (Circle)ent;

                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(circle.EndPoint, SnapType.END) });
                                break;
                            case SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(circle.PointAt(circle.Domain.Mid), SnapType.MID) });
                                break;
                            case SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(circle.Center, SnapType.CENTER) });
                                break;
                            case SnapType.QUAD:
                                Point3D quad1 = new Point3D(circle.Center.X, circle.Center.Y + circle.Radius);
                                Point3D quad2 = new Point3D(circle.Center.X + circle.Radius, circle.Center.Y);
                                Point3D quad3 = new Point3D(circle.Center.X, circle.Center.Y - circle.Radius);
                                Point3D quad4 = new Point3D(circle.Center.X - circle.Radius, circle.Center.Y);

                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(quad1, SnapType.QUAD),
                                                           new SnapPoint(quad2, SnapType.QUAD),
                                                           new SnapPoint(quad3, SnapType.QUAD),
                                                           new SnapPoint(quad4, SnapType.QUAD)});
                                break;
                        }
                    }
                }

                else if (ent is Curve) // Spline
                {
                    Curve curve = (Curve)ent;
                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] {new SnapPoint(curve.StartPoint, SnapType.END),
                                                          new SnapPoint(curve.EndPoint, SnapType.END)});
                                break;
                            case SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(curve.PointAt(0.5), SnapType.MID) });
                                break;
                        }
                    }
                }

                else if (ent is EllipticalArc) //Elliptical Arc
                {
                    EllipticalArc elArc = (EllipticalArc)ent;
                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] {new SnapPoint(elArc.StartPoint, SnapType.END),
                                                          new SnapPoint(elArc.EndPoint, SnapType.END)});
                                break;
                            case SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(elArc.Center, SnapType.CENTER) });
                                break;
                        }
                    }
                }
                else if (ent is Ellipse) //Ellipse
                {
                    Ellipse ellipse = (Ellipse)ent;
                    foreach (var activeObjectSnap in CurrentSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(ellipse.EndPoint, SnapType.END) });
                                break;
                            case SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(ellipse.PointAt(ellipse.Domain.Mid), SnapType.MID) });
                                break;
                            case SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(ellipse.Center, SnapType.CENTER) });
                                break;
                        }
                    }
                }

                //else if (ent is Entities.NSphere) //Mesh
                //{
                //    Entities.NSphere mesh = (Entities.NSphere)ent;
                //    foreach (var activeObjectSnap in CurrentSnapTypes)
                //    {
                //        snapPoints.Add(new SnapPoint(mesh.center, SnapType.END));

                //        break;
                //    }
                //}
            }

            return snapPoints.ToArray();
        }

        private void SetCurrentPoint()
        {
            var SnapPoints = this.GetSnapPoints();

            if (SnapPoints.Length > 0)
            {
                SnapPoint p = FindClosestPoint(SnapPoints);

                this.CurrentPoint = p;
            }

            else
            {
                if (this.Snapping != null)
                {
                    this.Snapping(new SnapEventArgs());
                }

                else
                {
                    ScreenToPlane(CurrentLocation, this.Camera.NearPlane, out CurrentPoint);
                }
            }
        }

        #region Mouse
        
        protected void OnMouseMove_Snapping(MouseEventArgs e)
        {
            this.SetCurrentPoint();
        }

        #endregion
    }
}
