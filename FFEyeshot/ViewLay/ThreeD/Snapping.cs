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
        public SnapState.SnapType Type;

        public SnapPoint()
            : base()
        {
            Type = SnapState.SnapType.NONE;
        }

        public SnapPoint(Point3D point3D, SnapState.SnapType SnapType) : base(point3D.X, point3D.Y, point3D.Z)
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

    public class SnapState
    {
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

        public List<SnapType> EnabledSnapTypes { get; set; } = new List<SnapType>() { SnapType.END, SnapType.CENTER, SnapType.MID };
        
        public List<SnapMode> EnabledSnapModes { get; set; } = new List<SnapMode>() { SnapMode.Wireframe, SnapMode.Solid };

        public event EventHandler SnapStateChanged;
        
        public void AddType(SnapType snapType)
        {
            if (!EnabledSnapTypes.Contains(snapType))
            {
                EnabledSnapTypes.Add(snapType);
                SnapStateChanged?.Invoke(this, null);
            }
        }

        public void RemoveType(SnapType snapType)
        {
            if (EnabledSnapTypes.Contains(snapType))
            {
                EnabledSnapTypes.Remove(snapType);
                SnapStateChanged?.Invoke(this, null);
            }
        }

        public void AddMode(SnapMode snapMode)
        {
            if (!EnabledSnapModes.Contains(snapMode))
            {
                EnabledSnapModes.Add(snapMode);
                SnapStateChanged?.Invoke(this, null);
            }
        }

        public void RemoveMode(SnapMode snapMode)
        {
            if (EnabledSnapModes.Contains(snapMode))
            {
                EnabledSnapModes.Remove(snapMode);
                SnapStateChanged?.Invoke(this, null);
            }
        }
    }

    public partial class ViewPort3D
    {
        #region State

        public SnapState StateSnap { get; set; } = new SnapState();

        private bool _isSnapEnable = true;

        public bool IsSnapEnable
        {
            get { return _isSnapEnable; }
            set {
                if (value != _isSnapEnable)
                {
                    OnSnapEnableChanging(_isSnapEnable);
                    PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs("SnapEnable"));
                    _isSnapEnable = value;
                    OnSnapEnableChanged(value);
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("SnapEnable"));
                }
            }
        }
        private void OnSnapEnableChanging(bool oldState)
        {

        }

        private void OnSnapEnableChanged(bool newState)
        {

        }
       
        #endregion
               
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
                case SnapState.SnapType.POINT:
                    DrawCircle(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    DrawCross(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapState.SnapType.CENTER:
                    DrawCircle(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapState.SnapType.END:
                    DrawQuad(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapState.SnapType.MID:
                    DrawTriangle(new System.Drawing.Point((int)onScreen.X, (int)(onScreen.Y)));
                    break;
                case SnapState.SnapType.QUAD:
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
                devDept.Eyeshot.Entities.Entity ent = Entities[CurrentIndex];

                if (ent is Entity.IFFEntity ffEntity)
                {
                    snapPoints.AddRange(ffEntity.GetSnapPoints(StateSnap));
                }

                else if (ent is Point point)
                {
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.POINT:
                                Point3D point3d = point.Vertices[0];
                                snapPoints.Add(new SnapPoint(point3d, SnapState.SnapType.POINT));
                                break;
                        }
                    }
                }
                else if (ent is Line line) //line
                {
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                snapPoints.Add(new SnapPoint(line.StartPoint, SnapState.SnapType.END));
                                snapPoints.Add(new SnapPoint(line.EndPoint, SnapState.SnapType.END));
                                break;
                            case SnapState.SnapType.MID:
                                snapPoints.Add(new SnapPoint(line.MidPoint, SnapState.SnapType.MID));
                                break;
                        }
                    }
                }
                else if (ent is LinearPath polyline)//polyline
                {
                    List<SnapPoint> polyLineSnapPoints = new List<SnapPoint>();

                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                foreach (var ver in polyline.Vertices)
                                    polyLineSnapPoints.Add(new SnapPoint(ver, SnapState.SnapType.END));
                                snapPoints.AddRange(polyLineSnapPoints);
                                break;
                        }
                    }
                }
                else if (ent is CompositeCurve composite)//composite
                {
                    List<SnapPoint> polyLineSnapPoints = new List<SnapPoint>();

                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                foreach (ICurve curveSeg in composite.CurveList)
                                    polyLineSnapPoints.Add(new SnapPoint(curveSeg.EndPoint, SnapState.SnapType.END));
                                polyLineSnapPoints.Add(new SnapPoint(composite.CurveList[0].StartPoint, SnapState.SnapType.END));
                                snapPoints.AddRange(polyLineSnapPoints);
                                break;
                        }
                    }
                }
                else if (ent is Arc arc) //Arc
                {
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(arc.StartPoint, SnapState.SnapType.END),
                                                           new SnapPoint(arc.EndPoint, SnapState.SnapType.END) });
                                break;
                            case SnapState.SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(arc.MidPoint, SnapState.SnapType.MID) });
                                break;
                            case SnapState.SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(arc.Center, SnapState.SnapType.CENTER) });
                                break;
                        }
                    }
                }
                else if (ent is Circle circle) //Circle
                {
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(circle.EndPoint, SnapState.SnapType.END) });
                                break;
                            case SnapState.SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(circle.PointAt(circle.Domain.Mid), SnapState.SnapType.MID) });
                                break;
                            case SnapState.SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(circle.Center, SnapState.SnapType.CENTER) });
                                break;
                            case SnapState.SnapType.QUAD:
                                Point3D quad1 = new Point3D(circle.Center.X, circle.Center.Y + circle.Radius);
                                Point3D quad2 = new Point3D(circle.Center.X + circle.Radius, circle.Center.Y);
                                Point3D quad3 = new Point3D(circle.Center.X, circle.Center.Y - circle.Radius);
                                Point3D quad4 = new Point3D(circle.Center.X - circle.Radius, circle.Center.Y);

                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(quad1, SnapState.SnapType.QUAD),
                                                           new SnapPoint(quad2, SnapState.SnapType.QUAD),
                                                           new SnapPoint(quad3, SnapState.SnapType.QUAD),
                                                           new SnapPoint(quad4, SnapState.SnapType.QUAD)});
                                break;
                        }
                    }
                }

                else if (ent is Curve curve) // Spline
                {
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] {new SnapPoint(curve.StartPoint, SnapState.SnapType.END),
                                                          new SnapPoint(curve.EndPoint, SnapState.SnapType.END)});
                                break;
                            case SnapState.SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(curve.PointAt(0.5), SnapState.SnapType.MID) });
                                break;
                        }
                    }
                }

                else if (ent is EllipticalArc) //Elliptical Arc
                {
                    EllipticalArc elArc = (EllipticalArc)ent;
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] {new SnapPoint(elArc.StartPoint, SnapState.SnapType.END),
                                                          new SnapPoint(elArc.EndPoint, SnapState.SnapType.END)});
                                break;
                            case SnapState.SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(elArc.Center, SnapState.SnapType.CENTER) });
                                break;
                        }
                    }
                }
                else if (ent is Ellipse) //Ellipse
                {
                    Ellipse ellipse = (Ellipse)ent;
                    foreach (var activeObjectSnap in StateSnap.EnabledSnapTypes)
                    {
                        switch (activeObjectSnap)
                        {
                            case SnapState.SnapType.END:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(ellipse.EndPoint, SnapState.SnapType.END) });
                                break;
                            case SnapState.SnapType.MID:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(ellipse.PointAt(ellipse.Domain.Mid), SnapState.SnapType.MID) });
                                break;
                            case SnapState.SnapType.CENTER:
                                snapPoints.AddRange(new SnapPoint[] { new SnapPoint(ellipse.Center, SnapState.SnapType.CENTER) });
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
                _SwapBufferRequired = true;
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
    
        protected void DrawOverlay_Snapping(DrawSceneParams data)
        {
            if (CurrentPoint is SnapPoint point)
            {
                DisplaySnappedVertex(point);
            }
        }
    
    }
}
