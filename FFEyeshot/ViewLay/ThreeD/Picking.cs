using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using devDept.Graphics;
using devDept.Eyeshot.Entities;
using System.Drawing;
using devDept.Geometry;

namespace FFEyeshot.ViewLay.ThreeD
{
    public partial class ViewPort3D
    {
        public Common.ViewportPickState CurrentPickState { get; set; }

        private bool buttonPressed = false;

        private System.Drawing.Point initialLocation;
        private System.Drawing.Point currentLocation;

        public event SelectionChangedEventHandler CustomSelectionChanged;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && ActionMode == devDept.Eyeshot.actionType.None)
            {
                if (!GetViewCubeIcon().Contains(RenderContextUtility.ConvertPoint(GetMousePosition(e))))
                {
                    buttonPressed = true;
                    initialLocation = currentLocation = RenderContextUtility.ConvertPoint(GetMousePosition(e));
                    CurrentPickState = Common.ViewportPickState.Pick;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (buttonPressed)
            {
                System.Drawing.Point location = RenderContextUtility.ConvertPoint(GetMousePosition(e));

                int diffX = location.X - initialLocation.X;

                if (diffX > 10)
                    CurrentPickState = Common.ViewportPickState.Enclosed;
                else if (diffX < -10)
                    CurrentPickState = Common.ViewportPickState.Crossing;
                else
                    CurrentPickState = Common.ViewportPickState.Pick;

                currentLocation = location;

                PaintBackBuffer();
                SwapBuffers();

            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            List<int> added = new List<int>();
            List<int> removed = new List<int>();

            if (buttonPressed)
            {
                IList<devDept.Eyeshot.Entities.Entity> myEnts = Entities.CurrentBlockReference != null
                    ? Blocks[Entities.CurrentBlockReference.BlockName].Entities
                    : new List<devDept.Eyeshot.Entities.Entity>(Entities);

                buttonPressed = false;
                int ent;
                int[] ents;

                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    for (int i = 0; i < myEnts.Count; i++)
                    {
                        if (myEnts[i].Selected)
                            removed.Add(i);

                        myEnts[i].Selected = false;
                    }
                }

                int dx = currentLocation.X - initialLocation.X;
                int dy = currentLocation.Y - initialLocation.Y;

                System.Drawing.Point p1 = initialLocation;
                System.Drawing.Point p2 = currentLocation;

                NormalizeBox(ref p1, ref p2);

                switch (CurrentPickState)
                {
                    case Common.ViewportPickState.Pick:

                        ent = GetEntityUnderMouseCursor(currentLocation);

                        if (ent >= 0)
                        {
                            ManageSelection(ent, myEnts, added, removed);
                        }

                        break;

                    case Common.ViewportPickState.Crossing:

                        if (dx != 0 && dy != 0)
                        {
                            ents =
                                GetAllCrossingEntities(new System.Drawing.Rectangle(p1,
                                    new System.Drawing.Size(Math.Abs(dx), Math.Abs(dy))));

                            for (int i = 0; i < ents.Length; i++)
                            {
                                ManageSelection(ents[i], myEnts, added, removed);
                            }
                        }

                        break;

                    case Common.ViewportPickState.Enclosed:

                        if (dx != 0 && dy != 0)
                        {
                            ents =
                                GetAllEnclosedEntities(new System.Drawing.Rectangle(p1,
                                    new System.Drawing.Size(Math.Abs(dx), Math.Abs(dy))));

                            for (int i = 0; i < ents.Length; i++)
                            {
                                ManageSelection(ents[i], myEnts, added, removed);
                            }
                        }
                        break;

                }
                Entities.Regen();
                Invalidate();
            }

            if (CustomSelectionChanged != null)
                CustomSelectionChanged(this, new SelectionChangedEventArgs(added.ToArray(), removed.ToArray(), this));

            base.OnMouseUp(e);
        }

        private void ManageSelection(int ent, IList<devDept.Eyeshot.Entities.Entity> myEnts, List<int> added, List<int> removed)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                myEnts[ent].Selected = !myEnts[ent].Selected;
                if (myEnts[ent].Selected)
                    added.Add(ent);
                else
                    removed.Add(ent);
            }
            else
            {
                myEnts[ent].Selected = true;
                added.Add(ent);
            }
        }
        
        protected override void DrawOverlay(DrawSceneParams data)
        {
            if (buttonPressed)
            {
                if (CurrentPickState == Common.ViewportPickState.Crossing)
                    DrawSelectionBox(initialLocation, currentLocation, System.Drawing.Color.DarkBlue, true, true);
                else if (CurrentPickState == Common.ViewportPickState.Enclosed)
                    DrawSelectionBox(initialLocation, currentLocation, System.Drawing.Color.DarkRed, true, false);
            }
        }

        void DrawSelectionBox(System.Drawing.Point p1, System.Drawing.Point p2, Color transparentColor, bool drawBorder, bool dottedBorder)
        {
            p1.Y = (int)(ActualHeight - p1.Y);
            p2.Y = (int)(ActualHeight - p2.Y);

            NormalizeBox(ref p1, ref p2);

            // Adjust the bounds so that it doesn't exit from the current viewport frame
            int[] viewFrame = Viewports[ActiveViewport].GetViewFrame();
            int left = viewFrame[0];
            int top = viewFrame[1] + viewFrame[3];
            int right = left + viewFrame[2];
            int bottom = viewFrame[1];

            if (p2.X > right - 1)
                p2.X = right - 1;

            if (p2.Y > top - 1)
                p2.Y = top - 1;

            if (p1.X < left + 1)
                p1.X = left + 1;

            if (p1.Y < bottom + 1)
                p1.Y = bottom + 1;

            renderContext.SetState(blendStateType.Blend);
            renderContext.SetColorWireframe(System.Drawing.Color.FromArgb(40, transparentColor.R, transparentColor.G, transparentColor.B));
            renderContext.SetState(rasterizerStateType.CCW_PolygonFill_CullFaceBack_NoPolygonOffset);

            int w = p2.X - p1.X;
            int h = p2.Y - p1.Y;

            renderContext.DrawQuad(new System.Drawing.RectangleF(p1.X + 1, p1.Y + 1, w - 1, h - 1));
            renderContext.SetState(blendStateType.NoBlend);

            if (drawBorder)
            {
                renderContext.SetColorWireframe(System.Drawing.Color.FromArgb(255, transparentColor.R, transparentColor.G, transparentColor.B));

                List<Point3D> pts = null;

                if (dottedBorder)
                {
                    renderContext.SetLineStipple(1, 0x0F0F, Viewports[0].Camera);
                    renderContext.EnableLineStipple(true);
                }

                int l = p1.X;
                int r = p2.X;
                if (renderContext.IsDirect3D)
                {
                    l += 1;
                    r += 1;
                }

                pts = new List<Point3D>(new Point3D[]
                    {
                    new Point3D(l, p1.Y), new Point3D(p2.X, p1.Y),
                    new Point3D(r, p1.Y), new Point3D(r, p2.Y),
                    new Point3D(r, p2.Y), new Point3D(l, p2.Y),
                    new Point3D(l, p2.Y), new Point3D(l, p1.Y),
                    });


                renderContext.DrawLines(pts.ToArray());

                if (dottedBorder)
                    renderContext.EnableLineStipple(false);
            }
        }

        internal static void NormalizeBox(ref System.Drawing.Point p1, ref System.Drawing.Point p2)
        {

            int firstX = Math.Min(p1.X, p2.X);
            int firstY = Math.Min(p1.Y, p2.Y);
            int secondX = Math.Max(p1.X, p2.X);
            int secondY = Math.Max(p1.Y, p2.Y);

            p1.X = firstX;
            p1.Y = firstY;
            p2.X = secondX;
            p2.Y = secondY;
        }
    }
}
