﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Elev8
{
	public partial class OrientationCube : UserControl
	{
		Vector[] CubePt = new Vector[10];	// 8 points, plus the line for heading
		Vector[] pt = new Vector[10];

		int[] CubeLine;

		float CenterX = 0.0f;
		float CenterY = 0.0f;

		const float ViewDist = 9.0f;
		const float ViewScale = 1200.0f;
		float DrawScale = 1.0f;

		public float cubeWidth = 1, cubeDepth = 1, cubeHeight = 1;

		public OrientationCube()
		{
			InitializeComponent();

			ResizeRedraw = true;
			DoubleBuffered = true;

			BuildCube();

			CubeLine = new int[] {
				0,1, 1,3, 3,2, 2,0,		// Top square
				4,5, 5,7, 7,6, 6,4,		// Bottom square
				0,4, 1,5, 2,6, 3,7,		// Connecting lines
				8, 9					// Heading indicator
			};
		}

		Quaternion quat = new Quaternion();
		Matrix mat = new Matrix();

		public float CubeWidth
		{
			get { return cubeWidth; }
			set { cubeWidth = value; BuildCube(); }
		}

		public float CubeHeight
		{
			get { return cubeHeight; }
			set { cubeHeight = value; BuildCube(); }
		}

		public float CubeDepth
		{
			get { return cubeDepth; }
			set { cubeDepth = value; BuildCube(); }
		}


		public Quaternion Quat
		{
			set {
				quat = value;
				mat.From( quat );

				Invalidate();
			}
		}

		public Matrix Mat
		{
			set {
				mat = value;

				Invalidate();
			}
		}

		static float Cos( float f )
		{
			return (float)Math.Cos( (float)f );
		}

		static float Sin( float f )
		{
			return (float)Math.Sin( (float)f );
		}


		void BuildCube()
		{
			CubePt[0] = new Vector( -cubeWidth, -cubeHeight, -cubeDepth );
			CubePt[1] = new Vector( -cubeWidth,  cubeHeight, -cubeDepth );
			CubePt[2] = new Vector(  cubeWidth, -cubeHeight, -cubeDepth );
			CubePt[3] = new Vector(  cubeWidth,  cubeHeight, -cubeDepth );
			CubePt[4] = new Vector( -cubeWidth, -cubeHeight,  cubeDepth );
			CubePt[5] = new Vector( -cubeWidth,  cubeHeight,  cubeDepth );
			CubePt[6] = new Vector(  cubeWidth, -cubeHeight,  cubeDepth );
			CubePt[7] = new Vector(  cubeWidth,  cubeHeight,  cubeDepth );

			CubePt[8] = new Vector( 0, 0, cubeDepth - 0.1f );
			CubePt[9] = new Vector( 0, 0, cubeDepth + 0.1f );

			Invalidate();
		}


		private void OrientationCube_Paint( object sender, PaintEventArgs e )
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;


			CenterX = (float)(ClientRectangle.Width / 2);
			CenterY = (float)(ClientRectangle.Height / 2);
			DrawScale = ViewScale * ((float)ClientRectangle.Width / 560.0f);

			float RealCenterX = CenterX;

			float[] cx = new float[2];
			cx[0] = CenterX * 2 / 4;
			cx[1] = CenterX + CenterX * 2 / 4;

			DrawCube( g, mat, Color.Black );
		}


		void DrawCube( Graphics g, Matrix m, Color col )
		{
			for(int i = 0; i < CubePt.Length; i++)
			{
				pt[i] = m.Mul( CubePt[i] );
			}

			for(int i = 0; i < CubePt.Length; i++)
			{
				pt[i].v[2] += ViewDist;

				if(pt[i].v[2] == 0) {
					pt[i].v[2] = 0.0001f;
				}

				pt[i].v[0] /= pt[i].v[2];
				pt[i].v[1] /= pt[i].v[2];
			}

			Pen penCol = new Pen( col );
			PointF[] cb = new PointF[2];

			for(int i = 0; i < CubeLine.Length; i += 2)
			{
				cb[0].X = pt[CubeLine[i]].v[0];
				cb[0].Y = pt[CubeLine[i]].v[1];
				cb[1].X = pt[CubeLine[i + 1]].v[0];
				cb[1].Y = pt[CubeLine[i + 1]].v[1];

				cb[0].X = cb[0].X * DrawScale + CenterX;
				cb[0].Y = cb[0].Y * -DrawScale + CenterY;
				cb[1].X = cb[1].X * DrawScale + CenterX;
				cb[1].Y = cb[1].Y * -DrawScale + CenterY;

				g.DrawLine( penCol, cb[0], cb[1] );
			}
		}
	}
}
