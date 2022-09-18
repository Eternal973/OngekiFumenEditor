﻿using Caliburn.Micro;
using OngekiFumenEditor.Base;
using OngekiFumenEditor.Base.OngekiObjects;
using OngekiFumenEditor.Base.OngekiObjects.ConnectableObject;
using OngekiFumenEditor.Base.OngekiObjects.Lane.Base;
using OngekiFumenEditor.Modules.FumenVisualEditor;
using OngekiFumenEditor.Utils;
using OngekiFumenEditor.Utils.ObjectPool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static OngekiFumenEditor.Modules.FumenPreviewer.Graphics.Drawing.TargetImpl.CommonLinesDrawTargetBase<OngekiFumenEditor.Base.OngekiObjects.Lane.Base.LaneStartBase>;
using static OngekiFumenEditor.Modules.FumenPreviewer.Graphics.Drawing.ILineDrawing;

namespace OngekiFumenEditor.Modules.FumenPreviewer.Graphics.Drawing.TargetImpl.OngekiObjects
{
    [Export(typeof(IDrawingTarget))]
    public class HoldDrawingTarget : CommonDrawTargetBase<Hold>
    {
        public override IEnumerable<string> DrawTargetID { get; } = new string[] { "HLD", "CHD", "XHD" };

        private TapDrawingTarget tapDraw;
        private ILineDrawing lineDrawing;

        public HoldDrawingTarget() : base()
        {
            tapDraw = IoC.Get<TapDrawingTarget>();
            lineDrawing = IoC.Get<ILineDrawing>();
        }

        public override void Draw(IFumenPreviewer target, Hold hold)
        {
            if (hold.Children.FirstOrDefault() is not HoldEnd holdEnd || hold.ReferenceLaneStart is not LaneStartBase start)
                return;

            var color = start.LaneType switch
            {
                LaneType.Left => new Vector4(1, 0, 0, 0.75f),
                LaneType.Center => new Vector4(0, 1, 0, 0.75f),
                LaneType.Right => new Vector4(0, 0, 1, 0.75f),
                LaneType.WallLeft => new Vector4(35 / 255.0f, 4 / 255.0f, 117 / 255.0f, 0.75f),
                LaneType.WallRight => new Vector4(136 / 255.0f, 3 / 255.0f, 152 / 255.0f, 0.75f),
                _ => default,
            };

            //draw line
            using var d = ObjectPool<List<LineVertex>>.GetWithUsingDisposable(out var list, out _);
            list.Clear();

            void Upsert<T>(T obj) where T : IHorizonPositionObject, ITimelineObject
            {
                var y = (float)TGridCalculator.ConvertTGridToY(obj.TGrid, target.Fumen.BpmList, 1.0, 240);
                var x = (float)XGridCalculator.ConvertXGridToX(obj.XGrid, 30, target.ViewWidth, 1);
                list.Add(new(new(x, y), color));
            }

            Upsert(hold);
            foreach (var node in start.Children.AsEnumerable<ConnectableObjectBase>().Prepend(start).Where(x => hold.TGrid <= x.TGrid && x.TGrid <= holdEnd.TGrid))
                Upsert(node);
            Upsert(holdEnd);

            lineDrawing.Draw(target, list, 13);

            //draw taps
            tapDraw.Draw(target, start.LaneType, hold.TGrid, hold.XGrid);
            tapDraw.Draw(target, start.LaneType, holdEnd.TGrid, holdEnd.XGrid);
        }
    }
}