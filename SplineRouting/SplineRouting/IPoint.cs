﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace SplineRouting
{
	/// <summary>
	/// Enables passing any type of graph (implementing FromTo, IEdge) into Input.
	/// </summary>
	public interface IPoint
	{
		double X { get; set; }
		double Y { get; set; }
	}
}
