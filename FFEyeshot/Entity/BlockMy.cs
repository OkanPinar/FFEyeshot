using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Eyeshot;

namespace FFEyeshot.Entity
{
    public class BlockMy : Block
    {
        public object Parent { get; set; }

        public BlockMy(): base()
        {

        }

        public BlockMy(devDept.Geometry.linearUnitsType unitType): base(unitType)
        {

        }
    }
}
