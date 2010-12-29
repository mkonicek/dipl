// <file>
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
	/// RouteVertex + additional info for A* algorithm, used by <see cref="AStarShortestPathFinder" />.
	/// </summary>
	/*public class RouteVertexInfo
	{
		// Design problem. Have a graph of objects, want to pin some information to each node.
		// If I traverse edge from one node to another, want to see the information pinned to target.
		// How to do it?? Common solution is that the original objects must support pinning additional info (Tag).
		// If they don't, we can wrap them, but then having an object, we cannot see the pinned information, eg. when navigating an edge.
		
		public RouteVertex Vertex { get; private set; }
		public double Distance { get; set; }
		public bool Permanent { get; set; }
		
		public RouteVertexInfo(RouteVertex vertex)
		{
			Vertex = vertex;
		}
	}*/
}
