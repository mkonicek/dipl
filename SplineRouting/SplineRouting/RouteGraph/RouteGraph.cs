﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SplineRouting
{
	/// <summary>
	/// Description of RouteGraph.
	/// </summary>
	public class RouteGraph
	{
		static readonly double boxPadding = 10;
		static readonly double boxSafetyMargin = 5;
		static readonly double multiEdgeGap = 10;
		
		List<IRect> boxes = new List<IRect>();
		public List<IRect> Boxes {
			get { return boxes; }
		}
		
		List<RouteVertex> vertices = new List<RouteVertex>();
		public List<RouteVertex> Vertices {
			get { return vertices; }
		}
		
		AStarShortestPathFinder pathFinder;
		
		public RouteGraph()
		{
			pathFinder = new AStarShortestPathFinder(this);
		}
		
		public static RouteGraph InitializeVertices(IEnumerable<IRect> nodes, IEnumerable<IEdge> edges)
		{
			var graph = new RouteGraph();
			// add vertices for node corners
			foreach (var node in nodes) {
				graph.Boxes.Add(node);
				foreach (var vertex in GetRectCorners(node, boxPadding)) {
					graph.Vertices.Add(vertex);
				}
			}
			// add vertices for egde endpoints
			foreach (var multiEdgeGroup in edges.GroupBy(edge => GetStartEnd(edge))) {
				int multiEdgeCount = multiEdgeGroup.Count();
				IRect fromRect = multiEdgeGroup.First().From;
				IRect toRect = multiEdgeGroup.First().To;
				var sourceCenter = GeomUtils.RectCenter(fromRect);
				var targetCenter = GeomUtils.RectCenter(toRect);
				if (Math.Abs(sourceCenter.X - targetCenter.X) > Math.Abs(sourceCenter.Y - targetCenter.Y) ||
				    (fromRect == toRect))
				{
					// the line is horizontal
					double multiEdgeSpanSource = GetMultiEdgeSpan(fromRect.Height, multiEdgeCount, multiEdgeGap);
					double multiEdgeSpanTarget = GetMultiEdgeSpan(toRect.Height, multiEdgeCount, multiEdgeGap);
					double originSourceCurrentY = sourceCenter.Y - multiEdgeSpanSource / 2;
					double originTargetCurrentY = targetCenter.Y - multiEdgeSpanTarget / 2;
					foreach (var edge in multiEdgeGroup.OrderBy(edge => edge.From == edge.To)) {
						Point2D sourceOrigin = new Point2D(sourceCenter.X, originSourceCurrentY);
						Point2D targetOrigin = new Point2D(targetCenter.X, originTargetCurrentY);
						// Here user could provide custom edgeStart and edgeEnd
						// inflate boxes a little so that edgeStart and edgeEnd are a little outside of the box (to prevent floating point errors)
						if (edge.From == edge.To) {
							var edgeStart = new Point2D(fromRect.Left + fromRect.Width + 0.01, originSourceCurrentY);
							var edgeEnd = new Point2D(fromRect.Left + fromRect.Width / 2, fromRect.Top);
							graph.AddEdgeEndpointVertices(edge, edgeStart, edgeEnd);
						} else {
							var edgeStart = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.From.Inflated(1e-3));
							var edgeEnd = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.To.Inflated(1e-3));
							graph.AddEdgeEndpointVertices(edge, edgeStart, edgeEnd);
						}
						originSourceCurrentY += multiEdgeSpanSource / (multiEdgeCount - 1);
						originTargetCurrentY += multiEdgeSpanTarget / (multiEdgeCount - 1);
					}
				}
				else
				{
					// the line is vertical
					double multiEdgeSpanSource = GetMultiEdgeSpan(fromRect.Width, multiEdgeCount, multiEdgeGap);
					double multiEdgeSpanTarget = GetMultiEdgeSpan(toRect.Width, multiEdgeCount, multiEdgeGap);
					double originSourceCurrentX = sourceCenter.X - multiEdgeSpanSource / 2;
					double originTargetCurrentX = targetCenter.X - multiEdgeSpanTarget / 2;
					foreach (var edge in multiEdgeGroup.OrderBy(edge => edge.From == edge.To)) {
						Point2D sourceOrigin = new Point2D(originSourceCurrentX, sourceCenter.Y);
						Point2D targetOrigin = new Point2D(originTargetCurrentX, targetCenter.Y);
						// Here user could provide custom edgeStart and edgeEnd
						// inflate boxes a little so that edgeStart and edgeEnd are a little outside of the box (to prevent floating point errors)
						var edgeStart = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.From.Inflated(1e-3));
						var edgeEnd = GeomUtils.LineRectIntersection(sourceOrigin, targetOrigin, edge.To.Inflated(1e-3));
						graph.AddEdgeEndpointVertices(edge, edgeStart, edgeEnd);
						originSourceCurrentX += multiEdgeSpanSource / (multiEdgeCount - 1);
						originTargetCurrentX += multiEdgeSpanTarget / (multiEdgeCount - 1);
					}
				}
			}
			return graph;
		}
		
		void AddEdgeEndpointVertices(IEdge edge, Point2D? edgeStart, Point2D? edgeEnd)
		{
			if (edgeStart == null || edgeEnd == null) {
				// should not happen
				return;
			}
			var startPoint = new RouteVertex(edgeStart.Value.X, edgeStart.Value.Y);
			startPoint.IsEdgeEndpoint = true;
			var endPoint = new RouteVertex(edgeEnd.Value.X, edgeEnd.Value.Y);
			endPoint.IsEdgeEndpoint = true;
			this.vertices.Add(startPoint);
			this.vertices.Add(endPoint);
			// remember what RouteVertices we created for this user edge
			this.setStartVertex(edge, startPoint);
			this.setEndVertex(edge, endPoint);
		}
		
		static IEnumerable<RouteVertex> GetRectCorners(IRect rect, double padding)
		{
			double left = rect.Left - padding;
			double top = rect.Top - padding;
			double right = left + rect.Width + 2 * padding;
			double bottom = top + rect.Height + 2 * padding;
			yield return new RouteVertex(left, top);
			yield return new RouteVertex(right, top);
			yield return new RouteVertex(right, bottom);
			yield return new RouteVertex(left, bottom);
		}
		
		public void ComputeVisibilityGraph()
		{
			for (int i = 0; i < this.Vertices.Count; i++) {
				for (int j = i + 1; j < this.Vertices.Count; j++) {
					var vertex = this.Vertices[i];
					var vertex2 = this.Vertices[j];
					if (Visible(vertex, vertex2))
					{
						// bidirectional edge
						vertex.AddNeighbor(vertex2);
						vertex2.AddNeighbor(vertex);
					}
				}
			}
		}
		
		public bool Visible(IPoint vertex, IPoint vertex2)
		{
			// test for intersection with every box
			foreach (var rect in this.Boxes) {
				if (GeomUtils.LineRectIntersection(vertex, vertex2, rect) != null)
					return false;
			}
			return true;
		}
		
		public RoutedEdge RouteEdge(IEdge edge)
		{
			var pathVertices = pathFinder.FindShortestPath(getStartVertex(edge), getEndVertex(edge));
			return BuildRoutedEdge(pathVertices);
		}
		
		public RoutedEdge TryRouteEdgeStraight(IEdge edge)
		{
			var startVertex = getStartVertex(edge);
			var endVertex = getEndVertex(edge);
			if (Visible(startVertex, endVertex)) {
				// route the edge straight
				return BuildRoutedEdge(new [] {startVertex, endVertex });
			} else
				return null;
		}
		
		public RoutedEdge BuildRoutedEdge(IEnumerable<IPoint> points)
		{
			var routedEdge = new RoutedEdge();
			foreach (var point in points) {
				routedEdge.Points.Add(new Point2D(point.X, point.Y));
			}
			return routedEdge;
		}
		
		Dictionary<IEdge, RouteVertex> edgeStarts = new Dictionary<IEdge, RouteVertex>();
		Dictionary<IEdge, RouteVertex> edgeEnds = new Dictionary<IEdge, RouteVertex>();
		
		RouteVertex getStartVertex(IEdge edge)
		{
			return edgeStarts[edge];
		}
		RouteVertex getEndVertex(IEdge edge)
		{
			return edgeEnds[edge];
		}
		void setStartVertex(IEdge edge, RouteVertex value)
		{
			edgeStarts[edge] = value;
		}
		void setEndVertex(IEdge edge, RouteVertex value)
		{
			edgeEnds[edge] = value;
		}
		
		static EdgeStartEnd GetStartEnd(IEdge edge)
		{
			return new EdgeStartEnd { From = edge.From, To = edge.To };
		}
		
		static double GetMultiEdgeSpan(double space, int multiEdgeCount, double multiEdgeGap)
		{
			if ((multiEdgeCount + 1) * multiEdgeGap < space)
				// the edges fit, maintain the gap
				return (multiEdgeCount - 1) * multiEdgeGap;
			else
				// there are too many edges, we have to make smaller gaps to fit edges into given space
				return space - multiEdgeGap;
		}
	}
}
