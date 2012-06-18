﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ANDREICSLIB
{
	public static class SaveFileDialogUpdates
	{
		private const char sep = '|';
		/// <summary>
		/// create the SFD filter string
		/// </summary>
		/// <param name="descAndExt">tuple with description, fileext eg: JPeg Image, *.jpg</param>
		/// <returns></returns>
        public static String createFilter(List<Tuple<String, String>> descAndExt)
		{
			String ret = "";
			foreach (var v in descAndExt)
			{
				ret += sep + createFilter(v.Item1, v.Item2);
			}
			return ret;
		}

		/// <summary>
		/// create the SFD filter string
		/// </summary>
		/// <param name="description">eg: JPeg Image</param>
		/// <param name="extension">eg  *.jpg</param>
		/// <returns></returns>
		public static String createFilter(String description, String extension)
		{
			return description + sep + extension;
		}
	}
}