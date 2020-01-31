using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Eyeshot.Translators;

namespace FFEyeshot.Entity
{


    public class CrossSection: Region
    {
        public event EventHandler CrossSectionChanging;

        public static Dictionary<string, CrossSection> Items = new Dictionary<string, CrossSection>();

        public string Name { get; set; }

        public CrossSection() :base()
        {

        }

        public CrossSection(Region other): base(other)
        {

        }
        
        public virtual void InitView(Plane sketchPlane)
        {

        }
    }

    public class AutocadCrossSection: CrossSection
    {
        public AutocadCrossSection()
        {

        }

        public AutocadCrossSection(Region other):base(other)
        {

        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                }
            }
        }

        public static AutocadCrossSection FromAutocad(string path, string name)
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

            var reg = new Region(curves.ToArray());
            reg.Regen(0.0);
            Point3D bboxMid = (reg.BoxMax + reg.BoxMin) / 2.0;
            reg.Translate(-bboxMid.X, -bboxMid.Y, bboxMid.Z);
            reg.Regen(0.0);
            var ret = new AutocadCrossSection(reg) { Path = path };
            return ret;
        }
    }
}
