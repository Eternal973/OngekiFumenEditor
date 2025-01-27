﻿using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace OngekiFumenEditor.Modules.FumenVisualEditor.Graphics.Drawing.TargetImpl.EditorObjects.Lane
{
	[Export(typeof(IFumenEditorDrawingTarget))]
	internal class BeamEditorObjectDrawingTarget : TextureLaneEditorObjectDrawingTarget
	{
		public override IEnumerable<string> DrawTargetID { get; } = new[]
		{
			"BMS"
		};

		public BeamEditorObjectDrawingTarget() : base(
			LoadTextrueFromDefaultResource("NS.png"),
			LoadTextrueFromDefaultResource("NN.png"),
			LoadTextrueFromDefaultResource("NE.png")
			)
		{

		}
	}
}
