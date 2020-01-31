using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;
using devDept.Eyeshot.Entities;
using devDept.Graphics;
using devDept.Eyeshot;

namespace FFEyeshot.Entity
{
    [Serializable]
    public class HybridSolid : Solid
    {
        public object Parent { get; set; }

        //TODO: Add V1,V2,V3 wire vertices to draw vectors
        public Point3D[] wireVertices { get; set; }

        private EntityGraphicsData wireGraphicsData = new EntityGraphicsData();

        public HybridSolid(): base()
        { 

        }

        public HybridSolid(HybridSolid another): base(another)
        {
            wireVertices = new Point3D[another.wireVertices.Length];
            for (int i = 0; i < wireVertices.Length; i++)
            {
                wireVertices[i] = (Point3D)another.wireVertices[i].Clone();
            }
        }

        public void ChangeNature(entityNatureType nature)
        {
            entityNature = nature;
        }

        public override void Compile(CompileParams data)
        {
            data.RenderContext.Compile(wireGraphicsData, (context, @params) =>
            {
                context.DrawLines(wireVertices);
            }, null);

            base.Compile(data);
        }

        public override void Regen(RegenParams data)
        {
            entityNatureType currNature = entityNature;

            entityNature = entityNatureType.Polygon; // so the regen of the mesh is done correctly

            base.Regen(data);

            entityNature = currNature;
        }

        public override void Dispose()
        {
            wireGraphicsData.Dispose();
            base.Dispose();
        }

        protected override void DrawForShadow(RenderParams renderParams)
        {
            if (entityNature != entityNatureType.Wire)
                base.DrawForShadow(renderParams);
        }

        protected override void Draw(DrawParams data)
        {
            if (entityNature == entityNatureType.Wire)
                data.RenderContext.Draw(wireGraphicsData);
            else
            {
                base.Draw(data);
            }
        }

        protected override void Render(RenderParams data)
        {
            if (entityNature == entityNatureType.Wire)
                data.RenderContext.Draw(wireGraphicsData);
            else
                base.Render(data);
        }

        protected override void DrawForSelection(GfxDrawForSelectionParams data)
        {
            if (entityNature == entityNatureType.Wire)
                data.RenderContext.Draw(wireGraphicsData);
            else
                base.DrawForSelection(data);
        }

        protected override void DrawEdges(DrawParams data)
        {
            if (entityNature != entityNatureType.Wire)
                base.DrawEdges(data);
        }

        protected override void DrawIsocurves(DrawParams data)
        {
            if (entityNature != entityNatureType.Wire)
                base.DrawIsocurves(data);
        }

        protected override void DrawHiddenLines(DrawParams data)
        {
            if (entityNature == entityNatureType.Wire)
                Draw(data);
            else
                base.DrawHiddenLines(data);
        }

        protected override void DrawNormals(DrawParams data)
        {
            if (entityNature != entityNatureType.Wire)
                base.DrawNormals(data);
        }

        protected override void DrawSilhouettes(DrawSilhouettesParams drawSilhouettesParams)
        {
            if (entityNature != entityNatureType.Wire)
                base.DrawSilhouettes(drawSilhouettesParams);
        }

        protected override void DrawWireframe(DrawParams drawParams)
        {
            if (entityNature == entityNatureType.Wire)
                Draw(drawParams);
            else

                base.DrawWireframe(drawParams);
        }

        protected override void DrawSelected(DrawParams drawParams)
        {
            if (entityNature == entityNatureType.Wire)
                drawParams.RenderContext.Draw(wireGraphicsData);
            else
                base.DrawSelected(drawParams);
        }

        protected override void DrawVertices(DrawParams drawParams)
        {
            if (entityNature == entityNatureType.Wire)
            {
                drawParams.RenderContext.DrawPoints(wireVertices);
            }
            else
                base.DrawVertices(drawParams);
        }

        protected override bool InsideOrCrossingFrustum(FrustumParams data)
        {
            if (entityNature == entityNatureType.Wire)
            {
                for (int i = 0; i < wireVertices.Length; i += 2)
                {
                    if (Utility.IsSegmentInsideOrCrossing(data.Frustum, new Segment3D(wireVertices[i], wireVertices[i + 1])))
                        return true;
                }
                return false;
            }

            return base.InsideOrCrossingFrustum(data);
        }

        public override void TransformBy(Transformation xform)
        {
            /*if (wireVertices != null)
                foreach (Point3D s in wireVertices)
                    s.TransformBy(xform);*/
            base.TransformBy(xform);
        }

        public override object Clone()
        {
            return new HybridSolid(this);
        }

        protected override bool AllVerticesInFrustum(FrustumParams data)
        {
            if (entityNature == entityNatureType.Wire)

                return UtilityEx.AllVerticesInFrustum(data, wireVertices, wireVertices.Length);

            return base.AllVerticesInFrustum(data);
        }

        protected override bool ComputeBoundingBox(TraversalParams data, out Point3D boxMin, out Point3D boxMax)
        {
            if (entityNature == entityNatureType.Wire)

                UtilityEx.ComputeBoundingBox(data.Transformation, wireVertices, out boxMin, out boxMax);

            else

                base.ComputeBoundingBox(data, out boxMin, out boxMax);

            return true;
        }
    }
}
