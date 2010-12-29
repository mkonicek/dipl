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
	/// Description of Input.
	/// </summary>
	public class EdgeRouter
	{
		public EdgeRouter()
		{
		}

		public RoutedEdge Result
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
		
		public List<RoutedEdge> RouteEdges(IEnumerable<IRect> nodes, IEnumerable<IEdge> edges)
		{
			var routeGraph = RouteGraph.InitializeVertices(nodes, edges);
			List<RoutedEdge> routedEdges = new List<RoutedEdge>();
			var occludedEdges = new List<IEdge>();
			foreach (IEdge edge in edges)	{
				var straightEdge = routeGraph.TryRouteEdgeStraight(edge);
				if (straightEdge != null)
					routedEdges.Add(straightEdge);
				else
					occludedEdges.Add(edge);
			}
			if (occludedEdges.Count > 0)	{
				// there are some edges that couldn't be routed as straight lines
				routeGraph.ComputeVisibilityGraph();
				foreach (IEdge edge in occludedEdges) {
					RoutedEdge routedEdge = routeGraph.RouteEdge(edge);
					routedEdges.Add(routedEdge);
				}
			}
			return routedEdges;
		}
	}
}
