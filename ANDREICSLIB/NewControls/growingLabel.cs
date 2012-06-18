﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB.controls
{
	public partial class growingLabel : Label
	{
		private bool mGrowing;

		public growingLabel()
		{
			this.AutoSize = false;
		}
		private void resizeLabel()
		{
			if (mGrowing) return;
			try
			{
				mGrowing = true;
				Size sz = new Size(this.Width, Int32.MaxValue);
				sz = TextRenderer.MeasureText(this.Text, this.Font, sz, TextFormatFlags.WordBreak);
				this.Height = sz.Height;
			}
			finally
			{
				mGrowing = false;
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			resizeLabel();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			resizeLabel();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			resizeLabel();
		}
	}
}

