using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using FFEyeshot.Common;

namespace FFEyeshot.Entity
{
    /*
    [Serializable]
    public class ProfilePosition: INotifyPropertyChanged
    {
        public Plane lcrMiddle { get; set; }
        public Plane lcrLeft { get; set; }
        public Plane lcrRight { get; set; }
        public Plane depthMiddle { get; set; }
        public Plane depthFront { get; set; }
        public Plane depthBehind { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    */

    /*
    [Serializable]
    public class Profile: INotifyEntityChanged
    {
        #region Props
        private Region _crossSection;
        /// <summary>
        /// Cross-section of the profile.
        /// </summary>
        public Region CrossSection
        {
            get { return _crossSection; }
            set { _crossSection = value; }
        }
        /// <summary>
        /// Cross-section width. Calculated from x value of the boundary box
        /// </summary>
        public double SectionWidth { get; private set; }
        /// <summary>
        /// Cross-section height. Calculated from y value of the boundary box
        /// </summary>
        public double SectionHeight { get; private set; }

        #region Postion Properties
        /// <summary>
        /// Position plane descriptor
        /// </summary>
        public ProfilePosition Position { get; set; } = new ProfilePosition();
        /// <summary>
        /// Current selected depth plane getter according to Depth property.
        /// </summary>
        private Plane _currentDepthPlane
        {
            get
            {
                switch (Depth)
                {
                    case AtDepth.BEHIND:
                        return Position.depthBehind;
                    case AtDepth.MIDDLE:
                        return Position.depthMiddle;
                    case AtDepth.FRONT:
                        return Position.depthFront;
                    default:
                        return null;
                }
            }
        }
        /// <summary>
        /// Current selected depth plane setter for change notification.
        /// </summary>
        public Plane CurrentDepthPlane
        {
            get { return _currentDepthPlane; }
            set
            {
                if (value != _currentDepthPlane)
                {
                    var projected = value.ProjectAt(pAllign);
                    var t = new Translation(new Vector3D(pAllign, projected));
                    pAllign.TransformBy(t);
                    View.TransformBy(t);
                    NotifyEntityChanged();
                }
            }
        }

        /// <summary>
        /// Current selected lcr plane getter according to LCR property.
        /// </summary>
        public Plane _currentLCRPlane
        {
            get
            {
                switch (LCR)
                {
                    case OnLCR.MIDDLE:
                        return Position.lcrMiddle;
                    case OnLCR.LEFT:
                        return Position.lcrLeft;
                    case OnLCR.RIGHT:
                        return Position.lcrRight;
                    default:
                        return null;
                }
            }
        }
        /// <summary>
        /// Current selected lcr plane setter for change notification.
        /// </summary>
        public Plane CurrentLCRPlane
        {
            get { return _currentLCRPlane; }
            set {
                if (_currentLCRPlane != value)
                {
                    var t = new Translation(new Vector3D(pAllign, value.ProjectAt(pAllign)));
                    pAllign.TransformBy(t);
                    View.TransformBy(t);
                    NotifyEntityChanged();
                }                
            }
        }

        /// <summary>
        /// Private field for lcr property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private OnLCR _lcr = OnLCR.MIDDLE;
        /// <summary>
        /// (L)eft-(C)enter-(R)ight property. 
        /// Describe of the position of cross-section at V3 direction.
        /// </summary>
        public OnLCR LCR
        {
            get { return _lcr; }
            set
            {
                if (_lcr != value)
                {
                    switch (value)
                    {
                        case OnLCR.MIDDLE:
                            CurrentLCRPlane = this.Position.lcrMiddle;
                            break;
                        case OnLCR.LEFT:
                            CurrentLCRPlane = this.Position.lcrLeft;
                            break;
                        case OnLCR.RIGHT:
                            CurrentLCRPlane = this.Position.lcrRight;
                            break;
                        default:
                            break;
                    }
                    _lcr = value;
                }
            }
        }

        /// <summary>
        /// Private field for depth property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private AtDepth _depth = AtDepth.MIDDLE;
        /// <summary>
        /// Describe of the position of the cross-section at V2 direction
        /// </summary>
        public AtDepth Depth
        {
            get { return _depth; }
            set
            {
                if (value != _depth)
                {
                    switch (value)
                    {
                        case AtDepth.MIDDLE:
                            this.CurrentDepthPlane = Position.depthMiddle;
                            break;
                        case AtDepth.FRONT:
                            this.CurrentDepthPlane = Position.depthFront;
                            break;
                        case AtDepth.BEHIND:
                            this.CurrentDepthPlane = Position.depthBehind;
                            break;
                        default:
                            break;
                    }
                    _depth = value;
                }
            }
        }

        /// <summary>
        /// Private field for rotation property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private OnRotation _rotation = OnRotation.TOP;
        /// <summary>
        /// Describe of the orientation of the cross-section
        /// </summary>
        public OnRotation Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation != value)
                {
                    switch (value)
                    {
                        case OnRotation.TOP:
                            SectionRotation = 0.0 + RotationOffset;
                            break;
                        case OnRotation.FRONT:
                            SectionRotation = 90 + RotationOffset;
                            break;
                        case OnRotation.BACK:
                            SectionRotation = 270 + RotationOffset;
                            break;
                        case OnRotation.BELOW:
                            SectionRotation = 180 + RotationOffset;
                            break;
                        default:
                            break;
                    }
                    _rotation = value;
                    InitPlanes();
                    RefreshView();
                }
            }
        }

        /// <summary>
        /// Private field for lcrOffset property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private double _lcrOffset = 0.0;
        /// <summary>
        /// Offset amount for LCR property. 
        /// In left and middle option cross-section is moving direction at V3.
        /// In rigth option cross section is moving direction at -V3.
        /// </summary>
        public double LCROffset
        {
            get
            {
                return _lcrOffset;
            }

            set
            {
                if (_lcrOffset != value)
                {
                    Position.lcrLeft.Translate((value - _lcrOffset) * Position.lcrLeft.AxisZ);
                    Position.lcrMiddle.Translate((value - _lcrOffset) * Position.lcrMiddle.AxisZ);
                    Position.lcrRight.Translate((value - _lcrOffset) * Position.lcrRight.AxisZ);
                    var t = new Translation(new Vector3D(pAllign, CurrentLCRPlane.ProjectAt(pAllign)));
                    pAllign.TransformBy(t);
                    this.View.TransformBy(t);
                    NotifyEntityChanged();
                    _lcrOffset = value;
                }
            }
        }

        /// <summary>
        ///  Private field for depth offset property.
        ///  TODO: Add a default value provider for this property.
        /// </summary>
        private double _depthOffset = 0.0;
        /// <summary>
        /// Offset amount for Depth property. 
        /// In Front and Middle option cross-section is moving direction at V2.
        /// In Behind option cross section is moving direction at -V2.
        /// </summary>
        public double DepthOffset
        {
            get
            {
                return _depthOffset;
            }

            set
            {
                if (_depthOffset != value)
                {
                    Position.depthBehind.Translate((value - _depthOffset) * Position.depthBehind.AxisZ);
                    Position.depthMiddle.Translate((value - _depthOffset) * Position.depthMiddle.AxisZ);
                    Position.depthFront.Translate((value - _depthOffset) * Position.depthFront.AxisZ);
                    var t = new Translation(new Vector3D(pAllign, CurrentDepthPlane.ProjectAt(pAllign)));
                    pAllign.TransformBy(t);
                    this.View.TransformBy(t);
                    NotifyEntityChanged();
                    _depthOffset = value;
                }
            }
        }

        /// <summary>
        /// Private field for rotation offset property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private double _rotationOffset = 0;
        /// <summary>
        /// Offset amount of the Rotation property.
        /// It rotate cross-section around V1
        /// </summary>
        public double RotationOffset
        {
            get
            {
                return _rotationOffset;
            }

            set
            {
                if (_rotationOffset != value)
                {
                    Transformation t = new Transformation();
                    double angle = Math.PI * (value - _rotationOffset) / 180.0;
                    t.Rotation(angle, V1, pAllign);
                    V2_curr.TransformBy(t);
                    V3_curr.TransformBy(t);
                    //View.TransformBy(t);
                    //NotifyEntityChanged();
                    this.SectionRotation += (value - _rotationOffset);
                    _rotationOffset = value;
                    InitPlanes();
                }
            }
        }

        /// <summary>
        /// Storage for value of the rotation information of the cross-section.
        /// </summary>
        private double _sectionRotation = 0;
        /// <summary>
        /// Adjustment for proper rotation around V1 vector.
        /// </summary>
        public double SectionRotation
        {
            get { return _sectionRotation; }
            set
            {
                if (_sectionRotation != value)
                {
                    var t = new Transformation();
                    var angle = Math.PI * (value - _sectionRotation) / 180;
                    t.Rotation(angle, V1, pAllign);
                    View.TransformBy(t);
                    NotifyEntityChanged();
                    _sectionRotation = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Start point of the profile
        /// </summary>
        public Point3D StartPoint { get; private set; }
        /// <summary>
        /// End point of the profile
        /// </summary>
        public Point3D EndPoint { get; private set; }

        /// <summary>
        /// Middle point of the referance line
        /// </summary>
        public Point3D MiddlePoint { get; private set; }

        /// <summary>
        /// Align point for the cross-section
        /// TODO: Notify transportation shoul be implemented.
        /// </summary>
        public PointT pAllign { get; private set; }

        /// <summary>
        /// Direction vector of the profile
        /// </summary>
        public Vector3D V1 { get; private set; }
        /// <summary>
        /// Default weak axis vector of the profile
        /// </summary>
        public Vector3D V2 { get; private set; }
        /// <summary>
        /// Default strong axis vector of the profile
        /// </summary>
        public Vector3D V3 { get; private set; }

        /// <summary>
        /// Rotated weak axis vector of the profile
        /// </summary>
        public Vector3D V2_curr { get; private set; }
        /// <summary>
        /// Rotated weak axis vector of the profile
        /// </summary>
        public Vector3D V3_curr { get; private set; }

        /// <summary>
        /// Descriptor line of the profile
        /// </summary>
        public Line ReferanceLine { get; private set; }

        /// <summary>
        /// Solid view of the cross-section
        /// </summary>
        public HybridSolid View { get; private set; }

        #endregion

        #region Events

        public event EntityChangingEventHandler OnEntityChanging;

        #endregion

        #region Ctors

        public Profile()
        {

        }

        public Profile(devDept.Eyeshot.Entities.Region crossSection, devDept.Geometry.Segment3D refLine)
        {
            this._crossSection = crossSection;
            this.StartPoint = new PointT(refLine.P0);
            this.EndPoint = new PointT(refLine.P1);
            this.ReferanceLine = new Line(refLine);
            this.InitVectors();
            this.InitView();
            this.InitPlanes();
        }
        
        #endregion

        /// <summary>
        /// Initilizer of the direction vector of a profile according to SAP2000 algorithm
        /// There are two case for describe a vector set of a frame
        /// <list type="bullet">
        ///     <listheader>
        ///         <description>Cases:</description>
        ///     </listheader>
        ///     <item>
        ///        <description>Case 1:</description>
        ///        If a frame is not vertical(verticallitaly is measured with abs of dot product of V1 and <c>k</c> enough close to 1
        ///        At that case V3 = V1 x <c>k</c> then V2 = V3 x V1
        ///     </item>
        ///     <item>
        ///         <description>Case 2:</description>
        ///         If a frame is verticall then
        ///         V2 = <c>i</c>
        ///         V3 = V1 x V2
        ///     </item>
        /// </list>
        /// </summary>
        private void InitVectors()
        {
            var vectors = this.GetInitVectors();

            this.V1 = vectors[0];
            this.V2 = vectors[1];
            this.V3 = vectors[2];

            this.V2_curr = vectors[1];
            this.V3_curr = vectors[2];

            this.pAllign = new PointT(EndPoint);
        }

        public Vector3D[] GetInitVectors()
        {
            Vector3D v1, v2, v3;
            v1 = new devDept.Geometry.Vector3D(StartPoint, EndPoint);
            v1.Normalize();
            this.MiddlePoint = (StartPoint + EndPoint) / 2.0;

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
            
            return new Vector3D[]{ v1, v2, v3};
        }
    
        private void InitView()
        {
            var length = ReferanceLine.Length();

            var transformation = new Transformation();
            transformation.Rotation(Point3D.Origin, Vector3D.AxisZ, Vector3D.AxisY, -1.0 * Vector3D.AxisX, StartPoint, V1, V2, V3);

            CrossSection.Regen(0.01);

            this.SectionWidth = CrossSection.BoxSize.X;
            this.SectionHeight = CrossSection.BoxSize.Y;

            Point3D bboxMid = (CrossSection.BoxMax + CrossSection.BoxMin) / 2.0;
            CrossSection.Translate(-bboxMid.X, -bboxMid.Y, bboxMid.Z);

            View = CrossSection.ExtrudeAsSolid<HybridSolid>(length * Vector3D.AxisZ, 0.01);
            
            var wirePnts = new List<Point3D>();

            #region Local Axis of the Frame
            wirePnts.Add((Point3D)Point3D.Origin.Clone());
            wirePnts.Add(new Point3D(0, 0, length));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length));
            wirePnts.Add(new Point3D(0, 300, 0.5 * length));

            wirePnts.Add(new Point3D(0, 300, 0.5 * length));
            wirePnts.Add(new Point3D(100, 200, 0.5 * length));

            wirePnts.Add(new Point3D(0, 300, 0.5 * length));
            wirePnts.Add(new Point3D(-100, 200, 0.5 * length));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length));
            wirePnts.Add(new Point3D(-300, 0, 0.5 * length));

            wirePnts.Add(new Point3D(-300, 0, 0.5 * length));
            wirePnts.Add(new Point3D(-200, 100, 0.5 * length));

            wirePnts.Add(new Point3D(-300, 0, 0.5 * length));
            wirePnts.Add(new Point3D(-200, -100, 0.5 * length));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length));
            wirePnts.Add(new Point3D(0, 0, 0.5 * length + 300));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length + 300));
            wirePnts.Add(new Point3D(100, 0, 0.5 * length + 200));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length + 300));
            wirePnts.Add(new Point3D(-100, 0, 0.5 * length + 200));

            wirePnts.ForEach(item => item.TransformBy(transformation));

            #endregion

            View.wireVertices = wirePnts.ToArray();
            
            View.TransformBy(transformation);

            View.Parent = this;
        }

        public void InitPlanes()
        {
            var vectors = this.GetInitVectors();

            switch (Rotation)
            {
                case OnRotation.TOP:
                case OnRotation.BELOW:
                    {
                        this.Position.lcrLeft = new Plane(EndPoint + V3 * SectionWidth * .5 + V3 * _lcrOffset, V3_curr);
                        this.Position.lcrMiddle = new Plane(EndPoint + V3 * _lcrOffset, V3_curr);
                        this.Position.lcrRight = new Plane(EndPoint - V3 * SectionWidth * .5 + V3 * _lcrOffset, -1.0 * V3_curr);

                        this.Position.depthBehind = new Plane(EndPoint - V2 * SectionHeight * .5 - V2 * _depthOffset, -1.0 * V2_curr);
                        this.Position.depthMiddle = new Plane(EndPoint + V2 * _depthOffset, V2_curr);
                        this.Position.depthFront = new Plane(EndPoint + V2 * SectionHeight * .5 + V2 * _depthOffset, V2_curr);
                    }
                    break;
                case OnRotation.FRONT:
                case OnRotation.BACK:
                    {
                        this.Position.lcrLeft = new Plane(EndPoint + V3 * SectionHeight * .5 + V3 * _lcrOffset, V3_curr);
                        this.Position.lcrMiddle = new Plane(EndPoint + V3 * _lcrOffset, V3_curr);
                        this.Position.lcrRight = new Plane(EndPoint - V3 * SectionHeight * .5 + V3 * _lcrOffset, -1.0 * V3_curr);

                        this.Position.depthBehind = new Plane(EndPoint - V2 * SectionWidth * .5 + V2 * _depthOffset, -1.0 * V2_curr);
                        this.Position.depthMiddle = new Plane(EndPoint + V2 * _lcrOffset, V2_curr);
                        this.Position.depthFront = new Plane(EndPoint + V2 * SectionWidth * .5 + V2 * _depthOffset, V2_curr);
                    }
                    break;
                default:
                    break;
            }
        }

        public void RefreshView()
        {
            var v_lcr = new Vector3D(pAllign, _currentLCRPlane.ProjectAt(pAllign));
            var v_depth = new Vector3D(pAllign, _currentDepthPlane.ProjectAt(pAllign));

            var t = new Translation(v_lcr + v_depth);
            pAllign.TransformBy(t);
            View.TransformBy(t);
            NotifyEntityChanged();
        }

        public void NotifyTransformation(object sender, TransformationEventArgs data)
        {
            throw new NotImplementedException();
            //this.OnTransforming?.Invoke()
        }
        
        public void NotifyEntityChanged()
        {
            OnEntityChanging?.Invoke(this.View, null);
        }
    
    }
    */

    

    [Serializable]
    public class Profile2: INotifyEntityChanged
    {
        #region Properties

        private Region _crossSection;
        /// <summary>
        /// Cross-section of the profile.
        /// </summary>
        public Region CrossSection
        {
            get { return _crossSection; }
            set { _crossSection = value; }
        }

        /// <summary>
        /// Direction vector of the profile
        /// </summary>
        public Vector3D V1 { get; private set; }
        /// <summary>
        /// Default weak axis vector of the profile
        /// </summary>
        public Vector3D V2 { get; private set; }
        /// <summary>
        /// Default strong axis vector of the profile
        /// </summary>
        public Vector3D V3 { get; private set; }
        /// <summary>
        /// Rotated weak axis vector of the profile
        /// </summary>
        public Vector3D V2_curr { get; private set; }
        /// <summary>
        /// Rotated weak axis vector of the profile
        /// </summary>
        public Vector3D V3_curr { get; private set; }

        /// <summary>
        /// Start point of the profile
        /// </summary>
        public PointT StartPoint { get; private set; }
        /// <summary>
        /// End point of the profile
        /// </summary>
        public PointT EndPoint { get; private set; }
        /// <summary>
        /// Middle point of the referance line
        /// </summary>
        public Point3D MiddlePoint { get; private set; }

        /// <summary>
        /// Descriptor line of the profile
        /// </summary>
        public Line ReferanceLine { get; private set; }

        /// <summary>
        /// Solid view of the cross-section
        /// </summary>
        public FFSolid View { get; private set; }

        private ProfilePosition2 _position;

        public event EntityChangedEventHandler OnEntityChanged;

        [Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ExpandableObject]
        public ProfilePosition2 Position {
            get { return _position; }
            set
            {
                if (value != _position)
                {
                    _position.Parent = null;
                    _position.pAllign.OnTransformed -= PAllingTransforming;
                    //value.Parent = this;
                }
            }
        }

        /// <summary>
        /// Cross-section width. Calculated from x value of the boundary box
        /// </summary>
        public double SectionWidth { get; private set; }
        /// <summary>
        /// Cross-section height. Calculated from y value of the boundary box
        /// </summary>
        public double SectionHeight { get; private set; }

        #endregion


        public Profile2()
        {

        }

        public Profile2(Region crossSection, PointT startPoint, PointT endPoint)
        {
            _crossSection = crossSection;
            StartPoint = startPoint;
            EndPoint = endPoint;

            _position = new ProfilePosition2();
            //_position.Parent = this;
            ReferanceLine = new Line(startPoint, endPoint);

            InitializeVectors();
            InitializeView();

            _position.InitializePlanes();

            StartPoint.OnTransformed += NotifyPointChanged;
            EndPoint.OnTransformed += NotifyPointChanged;
        }

        private void PAllingTransforming(object sender, TransformingEventArgs e)
        {
            this.View.TransformBy(e.TData);
            this.NotifyEntityChanged();
        }

        private void InitializeView()
        {
            var length = Vector3D.Distance(this.StartPoint, this.EndPoint);

            var transformation = new Transformation();
            transformation.Rotation(Point3D.Origin, Vector3D.AxisZ, Vector3D.AxisY, -1.0 * Vector3D.AxisX, StartPoint, V1, V2, V3);

            CrossSection.Regen(0.01);

            this.SectionWidth = CrossSection.BoxSize.X;
            this.SectionHeight = CrossSection.BoxSize.Y;

            Point3D bboxMid = (CrossSection.BoxMax + CrossSection.BoxMin) / 2.0;
            CrossSection.Translate(-bboxMid.X, -bboxMid.Y, bboxMid.Z);

            View = CrossSection.ExtrudeAsSolid<FFSolid>(length * Vector3D.AxisZ, 0.01);
            View.Rotate(Position.SectionRotation.ToRadian(), Vector3D.AxisZ);

            var wirePnts = new List<Point3D>();

            #region Local Axis of the Frame
            wirePnts.Add((Point3D)Point3D.Origin.Clone());
            wirePnts.Add(new Point3D(0, 0, length));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length));
            wirePnts.Add(new Point3D(0, 300, 0.5 * length));

            wirePnts.Add(new Point3D(0, 300, 0.5 * length));
            wirePnts.Add(new Point3D(100, 200, 0.5 * length));

            wirePnts.Add(new Point3D(0, 300, 0.5 * length));
            wirePnts.Add(new Point3D(-100, 200, 0.5 * length));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length));
            wirePnts.Add(new Point3D(-300, 0, 0.5 * length));

            wirePnts.Add(new Point3D(-300, 0, 0.5 * length));
            wirePnts.Add(new Point3D(-200, 100, 0.5 * length));

            wirePnts.Add(new Point3D(-300, 0, 0.5 * length));
            wirePnts.Add(new Point3D(-200, -100, 0.5 * length));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length));
            wirePnts.Add(new Point3D(0, 0, 0.5 * length + 300));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length + 300));
            wirePnts.Add(new Point3D(100, 0, 0.5 * length + 200));

            wirePnts.Add(new Point3D(0, 0, 0.5 * length + 300));
            wirePnts.Add(new Point3D(-100, 0, 0.5 * length + 200));

            wirePnts.ForEach(item => item.TransformBy(transformation));

            #endregion

            View.wireVertices = wirePnts.ToArray();

            View.TransformBy(transformation);

            View.Parent = this;
        }
        
        public Vector3D[] GetInitVectors()
        {
            Vector3D v1, v2, v3;
            v1 = new Vector3D(StartPoint, EndPoint);
            v1.Normalize();
            MiddlePoint = (StartPoint + EndPoint) / 2.0;

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

        private void InitializeVectors()
        {
            var vectors = this.GetInitVectors();

            V1 = vectors[0];
            V2 = vectors[1];
            V3 = vectors[2];

            var currentV2V3Transfromation = new Transformation();
            currentV2V3Transfromation.Rotation(Position.RotationOffset.ToRadian(), this.V1);

            V2_curr = new Vector3D(V2.X, V2.Y, V2.Z);
            V2_curr.TransformBy(currentV2V3Transfromation);
            V3_curr = new Vector3D(V3.X, V3.Y, V3.Z);
            V3_curr.TransformBy(currentV2V3Transfromation);

            _position.pAllign = new PointT(EndPoint);
            _position.pAllign.OnTransformed += PAllingTransforming;
        }

        public void RefreshView()
        {
            var v_lcr = new Vector3D(Position.pAllign, Position.CurrentLCRPlane.ProjectAt(Position.pAllign));
            var v_depth = new Vector3D(Position.pAllign, Position.CurrentDepthPlane.ProjectAt(Position.pAllign));

            var t = new Translation(v_lcr + v_depth);
            Position.pAllign.TransformBy(t);
        }

        public void RefreshRotation()
        {
            /*if (Position.Rotation != OnRotation.TOP)
            {
                string aa = "";
            }
            var temp_rot = Position.SectionRotation - Position.RotationOffset;

            Position.SectionRotation = Position.RotationOffset;
            Position.SectionRotation = temp_rot;*/
            }
        
        public void NotifyEntityChanged()
        {
            OnEntityChanged?.Invoke(this.View, null);
        }

        private void NotifyPointChanged(object sender, TransformingEventArgs e)
        {
            View.Parent = null;
            View.Dispose();
            View = null;
            InitializeVectors();
            Position.InitializePlanes();
            InitializeView();
            View.RegenMode = regenType.RegenAndCompile;
            View.Regen(0.1);
            RefreshView();
            RefreshRotation();
            NotifyEntityChanged();
        }
    }
}
