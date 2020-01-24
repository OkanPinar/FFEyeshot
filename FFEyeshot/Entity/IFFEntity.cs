using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFEyeshot.Entity
{
    public interface IFFEntity
    {
        List<ViewLay.ThreeD.SnapPoint> GetSnapPoints(ViewLay.ThreeD.SnapState state);
    }
}
