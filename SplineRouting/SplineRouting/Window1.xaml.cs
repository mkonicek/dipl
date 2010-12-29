// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SplineRouting
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		int numRect = 12;
		/*Brush normalBrush = new LinearGradientBrush(
			new Color { R = 150, G = 255, B = 200, A  = 255 },
			new Color { R = 255, G = 255, B = 200, A  = 255 }, 90);*/
		Brush normalBrush = new SolidColorBrush(new Color { R = 170, G = 200, B = 255, A  = 255 });
		Brush selectedBrush = new SolidColorBrush(new Color { R = 255, G = 100, B = 100, A  = 255 });
		
		List<Box> boxes = new List<Box>();
		List<Edge> edges = new List<Edge>();
		
		Box selectedBox;
		
		public Window1()
		{
			InitializeComponent();
			canvas.MouseUp += new MouseButtonEventHandler(canvas_MouseUp);
			canvas.Background = new SolidColorBrush(Colors.White);
		}
		
		void Redraw()
		{
			canvas.Children.Clear();
			DrawNodes();
			//var routeGraph = RouteGraph.InitializeVertices(boxes, edges);
			//routeGraph.ComputeVisibilityGraph();
			
			
			/*foreach (var vertex in routeGraph.Vertices) {
				foreach (var neighbor in vertex.Neighbors) {
					var line = new Line { Stroke = new SolidColorBrush(Colors.Azure), X1 = vertex.X, Y1 = vertex.Y, X2 = neighbor.EndVertex.X, Y2 = neighbor.EndVertex.Y };
					canvas.Children.Add(line);
				}
			}*/
			
			var router = new EdgeRouter();
			foreach (var routedEdge in router.RouteEdges(boxes, edges))
			{
				if (chbSmooth.IsChecked.GetValueOrDefault(false))
				{
					DrawRoutedEdge(routedEdge);
				}
				else
				{
					for (int i = 1; i < routedEdge.Points.Count; i++)
					{
						Path pathVisible = new Path();
						pathVisible.Stroke = Brushes.Black;
						pathVisible.Fill = Brushes.Black;
						pathVisible.StrokeThickness = 1;
						pathVisible.Data = new LineGeometry(routedEdge.Points[i - 1].ToWindowsPoint(), routedEdge.Points[i].ToWindowsPoint());
						canvas.Children.Add(pathVisible);
					}
				}
			}
			
			/*foreach (var vertex in routeGraph.Vertices) {
				var el = new Ellipse { Fill = new SolidColorBrush(Colors.CadetBlue), Width = 4, Height = 4 };
				Canvas.SetLeft(el, vertex.X);
				Canvas.SetTop(el, vertex.Y);
				canvas.Children.Add(el);
			}*/
		}
		
		void DrawRoutedEdge(RoutedEdge routedEdge)
		{
			/*for (int i = 1; i < routedEdge.Points.Count; i++) {
				var point0 = routedEdge.Points[i - 1];
				var point1 = routedEdge.Points[i];
				var line = new Line { Stroke = new SolidColorBrush(Colors.Black),
					X1 = point0.X, Y1 = point0.Y, X2 = point1.X, Y2 = point1.Y };
				canvas.Children.Add(line);
			}*/
			PathFigure edgeSplineFigure = createEdgeSpline(routedEdge);
			PathGeometry geometryVisible = new PathGeometry();
			geometryVisible.Figures.Add(edgeSplineFigure);
			Path pathVisible = new Path();
			pathVisible.Stroke = Brushes.Black;
			pathVisible.Fill = Brushes.Black;
			pathVisible.StrokeThickness = 1;
			pathVisible.Data = geometryVisible;
			canvas.Children.Add(pathVisible);
		}
		
		private PathFigure createEdgeSpline(RoutedEdge edge)
		{
			PathFigure figure = new PathFigure();
			figure.IsClosed = false;
			figure.IsFilled = false;
			
			figure.StartPoint = edge.SplinePoints[0].ToWindowsPoint();
			for (int i = 1; i < edge.SplinePoints.Count; i += 3)
			{
				figure.Segments.Add(new BezierSegment(
					edge.SplinePoints[i].ToWindowsPoint(),
					edge.SplinePoints[i + 1].ToWindowsPoint(),
					edge.SplinePoints[i + 2].ToWindowsPoint(),
					true));
			}
			
			return figure;
		}

		
		void DrawNodes()
		{
			foreach (var node in this.boxes) {
				canvas.Children.Add(node.rect);
			}
		}

		void canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (selectedBox != null)
			{
				selectedBox.rect.Fill = normalBrush;
				selectedBox.Left = Mouse.GetPosition(canvas).X;
				selectedBox.Top = Mouse.GetPosition(canvas).Y;
				selectedBox = null;
			}
			Redraw();
		}
		
		Rectangle makeRect()
		{
			var rect = new Rectangle();
			rect.Width = 40;
			rect.Height = 60;
			rect.Fill = normalBrush;
			return rect;
		}
		
		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			RandomizeBoxes();
			Redraw();
		}
		
		void RandomizeBoxes()
		{
			Random r = new Random();
			for (int i = 0; i < numRect; i++) {
				Box box = new Box(r.Next(0, (int)canvas.ActualWidth - 100), r.Next(0, (int)canvas.ActualHeight - 100), 60, 90);
				boxes.Add(box);
				box.rect.Fill = normalBrush;
				box.rect.MouseUp += delegate(object s, MouseButtonEventArgs eA) { boxClicked(box, eA); };
			}
		}
		
		void boxClicked(Box box, MouseButtonEventArgs e)
		{
			if (selectedBox == null)
			{
				selectedBox = box;
				selectedBox.rect.Fill = selectedBrush;
			}
			else
			{
				edges.Add(new Edge { From = selectedBox, To = box });
				selectedBox.rect.Fill = normalBrush;
				selectedBox = null;
			}
			e.Handled = true;
			Redraw();
		}
	}
	
	public static class Point2DExtensions
	{
		public static Point ToWindowsPoint(this Point2D point)
		{
			return new Point(point.X, point.Y);
		}
	}
}