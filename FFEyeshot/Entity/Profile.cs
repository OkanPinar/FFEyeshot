using System;
using System.Collections.Generic;
using System.Linq;

using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Eyeshot;
using FFEyeshot.Common;
using System.ComponentModel;
using FFEyeshot.ViewLay.ThreeD;

namespace FFEyeshot.Entity
{
    [Serializable]
    public class ProfilePosition2 : INotifyPropertyChanged
    {
        #region Properties
        public Profile Parent { get; set; }
        public Plane lcrMiddle { get; set; }
        public Plane lcrLeft { get; set; }
        public Plane lcrRight { get; set; }
        public Plane depthMiddle { get; set; }
        public Plane depthFront { get; set; }
        public Plane depthBehind { get; set; }

        /// <summary>
        /// Align point for the cross-section
        /// TODO: Notify transportation shoul be implemented.
        /// </summary>
        public PointT pAllign { get; set; }

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
                        return depthBehind;
                    case AtDepth.MIDDLE:
                        return depthMiddle;
                    case AtDepth.FRONT:
                        return depthFront;
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
                        return lcrMiddle;
                    case OnLCR.LEFT:
                        return lcrLeft;
                    case OnLCR.RIGHT:
                        return lcrRight;
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
            set
            {
                if (_currentLCRPlane != value)
                {
                    var t = new Translation(new Vector3D(pAllign, value.ProjectAt(pAllign)));
                    pAllign.TransformBy(t);
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
                            CurrentLCRPlane = lcrMiddle;
                            break;
                        case OnLCR.LEFT:
                            CurrentLCRPlane = lcrLeft;
                            break;
                        case OnLCR.RIGHT:
                            CurrentLCRPlane = lcrRight;
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
                            this.CurrentDepthPlane = depthMiddle;
                            break;
                        case AtDepth.FRONT:
                            this.CurrentDepthPlane = depthFront;
                            break;
                        case AtDepth.BEHIND:
                            this.CurrentDepthPlane = depthBehind;
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
                    InitializePlanes();
                    //Parent.RefreshView();
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
                    lcrLeft.Translate((value - _lcrOffset) * lcrLeft.AxisZ);
                    lcrMiddle.Translate((value - _lcrOffset) * lcrMiddle.AxisZ);
                    lcrRight.Translate((value - _lcrOffset) * lcrRight.AxisZ);
                    var t = new Translation(new Vector3D(pAllign, CurrentLCRPlane.ProjectAt(pAllign)));
                    pAllign.TransformBy(t);
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
                    Vector3D diff = null;
                    switch (Depth)
                    {
                        case AtDepth.BEHIND:
                            diff = (value - _lcrOffset) * Parent.V2 * -1.0;
                            break;
                        case AtDepth.MIDDLE:
                        case AtDepth.FRONT:
                            diff = (value - _lcrOffset) * Parent.V2;
                            break;
                    }
                    depthBehind.Translate((value - _depthOffset) * Parent.V2 * -1.0);
                    depthMiddle.Translate((value - _depthOffset) * Parent.V2);
                    depthFront.Translate((value - _depthOffset) * Parent.V2);
                    var t = new Translation(new Vector3D(pAllign, CurrentDepthPlane.ProjectAt(pAllign)));
                    pAllign.TransformBy(t);
                    _depthOffset = value;
                }
            }
        }

        /// <summary>
        /// Private field for rotation offset property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private double _rotationOffset = 0.0;
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
                    t.Rotation(angle, Parent.V1, pAllign);
                    Parent.V2_curr.TransformBy(t);
                    Parent.V3_curr.TransformBy(t);
                    this.SectionRotation += (value - _rotationOffset);
                    _rotationOffset = value;
                    InitializePlanes();
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
                    t.Rotation(angle, Parent.V1, pAllign);
                    Parent.TransformBy(t);
                    //Parent.NotifyEntityChanged();
                    _sectionRotation = value;
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public ProfilePosition2()
        {

        }

        public ProfilePosition2(ProfilePosition2 other)
        {
            this.pAllign = new PointT(other.pAllign);
        }

        public void InitializePlanes()
        {
            var vectors = Vector3D.AxisX.GetFrameVectors(Parent.StartPoint, Parent.EndPoint);
            var V2 = vectors[1];
            var V3 = vectors[2];
            var V2_curr = Parent.V2_curr;
            var V3_curr = Parent.V3_curr;
            var endP = Parent.EndPoint;
            var secHeight = Parent.SectionHeight;
            var secWidth = Parent.SectionWidth;

            switch (Rotation)
            {
                case OnRotation.TOP:
                case OnRotation.BELOW:
                    {
                        lcrLeft = new Plane(endP + V3 * secWidth * .5 + V3 * _lcrOffset, V3_curr);
                        lcrMiddle = new Plane(endP + V3 * _lcrOffset, V3_curr);
                        lcrRight = new Plane(endP - V3 * secWidth * .5 - V3 * _lcrOffset, -1.0 * V3_curr);

                        depthBehind = new Plane(endP - V2 * secHeight * .5 - V2 * _depthOffset, -1.0 * V2_curr);
                        depthMiddle = new Plane(endP + V2 * _depthOffset, V2_curr);
                        depthFront = new Plane(endP + V2 * secHeight * .5 + V2 * _depthOffset, V2_curr);
                    }
                    break;
                case OnRotation.FRONT:
                case OnRotation.BACK:
                    {
                        lcrLeft = new Plane(endP + V3 * secHeight * .5 + V3 * _lcrOffset, V3_curr);
                        lcrMiddle = new Plane(endP + V3 * _lcrOffset, V3_curr);
                        lcrRight = new Plane(endP - V3 * secHeight * .5 - V3 * _lcrOffset, -1.0 * V3_curr);

                        depthBehind = new Plane(endP - V2 * secWidth * .5 - V2 * _depthOffset, -1.0 * V2_curr);
                        depthMiddle = new Plane(endP + V2 * _lcrOffset, V2_curr);
                        depthFront = new Plane(endP + V2 * secWidth * .5 + V2 * _depthOffset, V2_curr);
                    }
                    break;
                default:
                    break;
            }
        }

        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Transformation GetCurrentTData()
        {
            var ret = new Transformation();

            var t1 = new Rotation((SectionRotation - RotationOffset).ToRadian(), Vector3D.AxisZ);
            var t2 = new Transformation();
            t2.Rotation(Point3D.Origin, Vector3D.AxisZ, Vector3D.AxisY, -1.0 * Vector3D.AxisX,
                         Parent.StartPoint, Parent.V1, Parent.V2_curr, Parent.V3_curr);
            var t3 = new Translation(new Vector3D(Parent.EndPoint, pAllign));

            ret = t1 * t2 * t3;

            return ret;
        }
    }

    public class ProfilePosition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<Solid> allingPntsRepresenter = new List<Solid>();

        public Profile Parent { get; set; }

        public Point3D[,] allignPnts = new Point3D[3,3];
        public PointT pAllign { get; set; }

        private int _lcrIndex = 1;
        public int LCRIndex
        {
            get { return _lcrIndex; }
            set {
                if (value != _lcrIndex)
                {
                    _lcrIndex = value;
                    //AllignCurrentIndex();
                    InitPoints();
                }
            }
        }

        private int _depthIndex = 1;
        public int DepthIndex
        {
            get { return _depthIndex; }
            set {
                if (value != _depthIndex)
                {
                    _depthIndex = value;
                    //AllignCurrentIndex();
                    InitPoints();
                }
            }
        }

        private int lcrDir = 1;
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
                            lcrDir = 1;
                            this.LCRIndex = 1;
                            break;
                        case OnLCR.LEFT:
                            lcrDir = 1;
                            this.LCRIndex = 0;
                            break;
                        case OnLCR.RIGHT:
                            lcrDir = -1;
                            this.LCRIndex = 2;
                            break;
                    }
                    _lcr = value;
                }
            }
        }

        private int depthDir = 1;
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
                            depthDir = 1;
                            DepthIndex = 1;                            
                            break;
                        case AtDepth.FRONT:
                            depthDir = 1;
                            this.DepthIndex = 2;
                            break;
                        case AtDepth.BEHIND:
                            depthDir = -1;
                            this.DepthIndex = 0;
                            break;
                    }
                    _depth = value;
                }
            }
        }

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
                    }
                    _rotation = value;
                    this.InitPoints();
                    //Parent.RefreshView();
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
                    /*Vector3D diff = (value - _lcrOffset) * lcrDir * Parent.V3;
                    var t = new Translation(diff);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            allignPnts[i, j].TransformBy(t);
                        }
                    }
                    pAllign.TransformBy(t);*/
                    _lcrOffset = value;
                    InitPoints();
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
                    /*Vector3D diff = (value - _depthOffset) * Parent.V2 * depthDir ;
                    var t = new Translation(diff);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            allignPnts[i, j].TransformBy(t);
                        }
                    }
                    pAllign.TransformBy(t);*/
                    _depthOffset = value;
                    InitPoints();
                }
            }
        }
        
        /// <summary>
        /// Private field for rotation offset property.
        /// TODO: Add a default value provider for this property.
        /// </summary>
        private double _rotationOffset = 0.0;
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
                    /*Transformation t = new Transformation();
                    double angle = Math.PI * (value - _rotationOffset) / 180.0;
                    t.Rotation(angle, Parent.V1, allignPnts[1, 1]);
                    Parent.V2_curr.TransformBy(t);
                    Parent.V3_curr.TransformBy(t);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            allignPnts[i, j].TransformBy(t);
                        }
                    }*/
                    this.SectionRotation += (value - _rotationOffset);
                    _rotationOffset = value;
                    InitPoints();
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
                    t.Rotation(angle, Parent.V1, pAllign);
                    Parent.TransformBy(t);
                    _sectionRotation = value;
                }
            }
        }

        private double _startPointOffset = 0.0;

        public double StartPointOffset
        {
            get { return _startPointOffset; }
            set {
                if (value != _startPointOffset)
                {
                    _startPointOffset = value;
                }
            }
        }

        private double _endPointOffset = 0.0;

        public double EndPointOffset
        {
            get { return _endPointOffset; }
            set {
                if (value != _endPointOffset)
                {
                    _endPointOffset = value;
                }
            }
        }

        public ProfilePosition()
        {

        }

        public void InitPoints()
        {
            double halfWidth = 0.0;
            double halfHeight = 0.0;

            switch (Rotation)
            {
                case OnRotation.TOP:
                case OnRotation.BELOW:
                    {
                        halfWidth = 0.5 * Parent.SectionWidth;
                        halfHeight = 0.5 * Parent.SectionHeight;
                    }
                    break;
                case OnRotation.FRONT:
                case OnRotation.BACK:
                    {
                        halfWidth = 0.5 * Parent.SectionHeight;
                        halfHeight = 0.5 * Parent.SectionWidth;
                    }
                    break;
            }

            Vector3D halfV2 = halfHeight * Parent.V2;
            Vector3D halfV3 = halfWidth* Parent.V3;
            Vector3D lcrOffsetV = _lcrOffset * lcrDir * Parent.V3;
            Vector3D depthOffsetV = _depthOffset * depthDir * Parent.V2;

            allignPnts[0, 0] = Parent.MidPoint - halfV2 + halfV3 + lcrOffsetV + depthOffsetV;
            allignPnts[1, 0] = Parent.MidPoint          + halfV3 + lcrOffsetV + depthOffsetV;
            allignPnts[2, 0] = Parent.MidPoint + halfV2 + halfV3 + lcrOffsetV + depthOffsetV;
            allignPnts[0, 1] = Parent.MidPoint - halfV2          + lcrOffsetV + depthOffsetV;
            allignPnts[1, 1] = new Point3D(Parent.MidPoint.X, Parent.MidPoint.Y, Parent.MidPoint.Z) + lcrOffsetV + depthOffsetV; ;
            allignPnts[2, 1] = Parent.MidPoint + halfV2          + lcrOffsetV + depthOffsetV; ;
            allignPnts[0, 2] = Parent.MidPoint - halfV2 - halfV3 + lcrOffsetV + depthOffsetV; ;
            allignPnts[1, 2] = Parent.MidPoint          - halfV3 + lcrOffsetV + depthOffsetV; ;
            allignPnts[2, 2] = Parent.MidPoint + halfV2 - halfV3 + lcrOffsetV + depthOffsetV; ;

            Transformation t = new Transformation();
            t.Rotation(_rotationOffset.ToRadian(), Parent.V1, allignPnts[1, 1]);
            Parent.V2_curr.TransformBy(t);
            Parent.V3_curr.TransformBy(t);
            allingPntsRepresenter.Clear();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    allignPnts[i, j].TransformBy(t);
                    var temp = Solid.CreateSphere(8, 10, 10);
                    temp.Translate(new Vector3D(Point3D.Origin, allignPnts[i, j]));
                    allingPntsRepresenter.Add(temp);
                }
            }
            pAllign.TransformBy(new Translation(new Vector3D(pAllign, allignPnts[DepthIndex, LCRIndex])));
            var pAllignRep = Solid.CreateSphere(10, 10, 10);
            pAllignRep.Translate(new Vector3D(Point3D.Origin, pAllign));
            pAllignRep.Color = System.Drawing.Color.Red;
            pAllignRep.ColorMethod = colorMethodType.byEntity;            
            allingPntsRepresenter.Add(pAllignRep);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RepresentPoints"));
        }
    
        private void AllignCurrentIndex()
        {
            
            var currentAlingPoint = allignPnts[_depthIndex, _lcrIndex];

            var t = new Translation(new Vector3D(pAllign, currentAlingPoint));

            pAllign.TransformBy(t);
        }

        public Transformation GetCurrentTData()
        {
            var ret = new Transformation();

            var t1 = new Rotation((SectionRotation - RotationOffset).ToRadian(), Vector3D.AxisZ);
            var t2 = new Transformation();
            t2.Rotation(Point3D.Origin, Vector3D.AxisZ, Vector3D.AxisY, -1.0 * Vector3D.AxisX,
                         Parent.StartPoint, Parent.V1, Parent.V2_curr, Parent.V3_curr);
            var t3 = new Translation(new Vector3D(Parent.MidPoint, pAllign));

            ret = t1 * t2 * t3;

            return ret;
        }
    }

    public class Profile: Solid, INotifyTransformation, INotifyEntityChanged, IFFEntity
    {
        #region props
        private PointT _startPoint;
        /// <summary>
        /// Start point of the profile
        /// </summary>
        public PointT StartPoint
        {
            get { return _startPoint; }
            set { _startPoint = value; }
        }

        private PointT _endPoint;
        /// <summary>
        /// End point of the profile
        /// </summary>
        public PointT EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        public Point3D MidPoint { get; private set; }

        public Vector3D V1 { get; set; }
        public Vector3D V2 { get; set; }
        public Vector3D V3 { get; set; }
        /// <summary>
        /// Rotated weak axis vector of the profile
        /// </summary>
        public Vector3D V2_curr { get; private set; }
        /// <summary>
        /// Rotated weak axis vector of the profile
        /// </summary>
        public Vector3D V3_curr { get; private set; }

        private Region _crossSection ;
        public Region CrossSection
        {
            get { return _crossSection; }
            set {
                if (value != _crossSection)
                {
                    NotifyCrossSectionChanging(value);
                    _crossSection = value;
                }                
            }
        }

        public double SectionWidth { get; set; }
        public double SectionHeight { get; set; }

        [Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ExpandableObject]
        public ProfilePosition Position { get; set; } = new ProfilePosition();

        public event TransformingEventHandler OnTransforming;
        public event EntityChangedEventHandler OnEntityChanged;

        public object Parent { get; set; }
        #endregion

        #region ctors
        public Profile(): base()
        {

        }

        public Profile(brepType bType, TextureMappingData textureData, IList<Portion> portions): base(bType, textureData, portions)
        {

        }

        public Profile(PointT startPoint, PointT endPoint, Region crossSection): base()
        {
            this._startPoint = startPoint;
            this._endPoint = endPoint;
            this.MidPoint = (startPoint + endPoint) / 2.0;
            this._crossSection = crossSection;
        }
        #endregion

        private void Init()
        {
            InitSection();
            InitVectors();
            InitPosition();
        }

        private void InitSection()
        {
            CrossSection.Regen(0.0);

            SectionWidth = CrossSection.BoxSize.X;
            SectionHeight = CrossSection.BoxSize.Y;
        }

        private void InitPosition()
        {
            Position.Parent = this;
            Position.pAllign = new PointT(MidPoint);
            //Position.InitializePlanes();
            Position.InitPoints();
            Position.pAllign.OnTransforming += NotifyTransformation;
        }        

        private void InitVectors()
        {
            var vectors = Vector3D.AxisX.GetFrameVectors(this.StartPoint, this.EndPoint);

            V1 = vectors[0];
            V2 = vectors[1];
            V3 = vectors[2];

            var currentV2V3Transfromation = new Transformation();
            currentV2V3Transfromation.Rotation(Position.RotationOffset.ToRadian(), this.V1);

            V2_curr = new Vector3D(V2.X, V2.Y, V2.Z);
            V2_curr.TransformBy(currentV2V3Transfromation);
            V3_curr = new Vector3D(V3.X, V3.Y, V3.Z);
            V3_curr.TransformBy(currentV2V3Transfromation);
            this.MidPoint = (this.StartPoint + this.EndPoint) / 2.0;
        }

        private void NotifyCrossSectionChanging(Region section)
        {
            var solid = section.ExtrudeAsSolid(Point3D.Distance(this.StartPoint, this.EndPoint), 0.0);
            solid.TransformBy(Position.GetCurrentTData());
        }

        public void NotifyTransformation(object sender, TransformingEventArgs e)
        {
            if (sender is PointT point)
            {
                if (point == Position.pAllign)
                {
                    TransformBy(e.TData);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static Profile Create(Region crossSection, Point3D startPoint, Point3D endPoint)
        {
            crossSection.Regen(0.0);
            Point3D bboxMid = (crossSection.BoxMax + crossSection.BoxMin) / 2.0;
            crossSection.Translate(-bboxMid.X, -bboxMid.Y, bboxMid.Z);

            Profile profile = crossSection.ExtrudeAsSolid<Profile>(Vector3D.AxisZ * startPoint.DistanceTo(endPoint), 0.1);
            profile._crossSection = crossSection;
            profile._startPoint = new PointT(startPoint);
            profile._endPoint = new PointT(endPoint);
            profile.Init();

            profile.TransformBy(profile.Position.GetCurrentTData());

            return profile;
        }

        public override void TransformBy(Transformation transform)
        {
            base.TransformBy(transform);
            OnEntityChanged?.Invoke(this, null);
        }

        public List<SnapPoint> GetSnapPoints(SnapState state)
        {
            var ret = new List<SnapPoint>();
            foreach (var snapMode in state.EnabledSnapModes)
            {
                switch (snapMode)
                {
                    case SnapState.SnapMode.Wireframe:
                        ret.Add(new SnapPoint(this.StartPoint, SnapState.SnapType.END));
                        ret.Add(new SnapPoint(this.MidPoint, SnapState.SnapType.MID));
                        ret.Add(new SnapPoint(this.EndPoint, SnapState.SnapType.END));
                        break;
                    //case SnapMode.Solid:
                    //    var temp = p.GetSnapPoint();
                    //    temp.ForEach(item => snapPoints.Add(new SnapPoint(item, SnapType.QUAD)));
                    //    break;
                    default:
                        break;
                }
            }
            return ret;
        }
    }
}
