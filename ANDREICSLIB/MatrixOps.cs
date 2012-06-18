﻿using System;

namespace ANDREICSLIB
{
	public static class MatrixOps
	{
		public static T[][] CloneMatrix<T>(T[][] gridIN, int widthI, int heightI) where T : new()
		{
			var outm = CreateMatrix<T>(widthI, heightI);

			for (int y = 0; y < heightI; y++)
			{
				for (int x = 0; x < widthI; x++)
				{
					outm[y][x] = gridIN[y][x];
				}
			}
			return outm;
		}
		
		public static T[][] CreateMatrix<T>(int widthI, int heightI) where T:new()
		{
			var outm = new T[heightI][];

			for (int y = 0; y < heightI; y++)
			{
				outm[y] = new T[widthI];
				for (int x=0;x<widthI;x++)
				{
					outm[y][x] = new T();
				}
			}
			return outm;
		}

		public static String SerialiseMatrix<T>(T[][] matrix, int width,int height,String rowsep=",",String linesep="\r\n")
		{
			String ret = "";
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					ret += matrix[y][x].ToString() + rowsep;
				}
				if (y != (height - 1))
					ret += linesep;
			}
			return ret;
		}


	}
}
