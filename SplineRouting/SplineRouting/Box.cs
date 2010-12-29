﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SplineRouting
{
	/// <summary>
	/// Description of Box.
	/// </summary>
	public class Box : IRect, ICloneable
	{
		public Rectangle rect;
		Dictionary<double, Box> inflatedCache = new Dictionary<double, Box>();
		
		public Box(double left, double top, double width, double height)
		{
			this.rect = new Rectangle();
			this.Left = left;
			this.Top = top;
			this.Width = width;
			this.Height = height;
		}
		
		public double Left { get { return Canvas.GetLeft(rect); } set {Canvas.SetLeft(rect, value); } }
		public double Top { get { return Canvas.GetTop(rect); } set {Canvas.SetTop(rect, value); } }
		public double Width { get { return rect.Width; } set { rect.Width = value; } }
		public double Height { get { return rect.Height; } set { rect.Height = value; } }
		
		public IRect Inflated(double padding)
		{
			//if (inflatedCache.ContainsKey(padding))
				//return inflatedCache[padding];
			var clone = (Box)this.Clone();
			GeomUtils.InflateRect(clone, padding);
			//inflatedCache[padding] = clone;
			return clone;
		}
		
		public object Clone()
		{
			return new Box(this.Left, this.Top, this.Width, this.Height);
		}
	}
}
