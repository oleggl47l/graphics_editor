using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace graphics_editor
{
    public partial class MainWindow : Window
    {
        private enum ShapeType
        {
            None,
            Ellipse,
            Triangle,
            Line,
            Rectangle
        }

        private ShapeType currentShape = ShapeType.None;
        private Point startPoint;
        private Shape currentShapeElement;
        private double lineWidth = 2;
        private bool isSettingBackgroundColor = false;

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lineWidth = slider.Value; // Получить значение из Slider
            try
            {
                UpdatePencilThickness();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void btn_pencil_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.Ink;
            try
            {
                UpdatePencilThickness();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void UpdatePencilThickness()
        {
            if (canvas != null && canvas.DefaultDrawingAttributes != null)
            {
                var currentAttributes = canvas.DefaultDrawingAttributes;
                var newAttributes = new DrawingAttributes
                {
                    Width = lineWidth,
                    Height = lineWidth,
                    Color = currentAttributes.Color, // сохраняем текущий цвет пера
                    StylusTip = currentAttributes.StylusTip,
                    IsHighlighter = currentAttributes.IsHighlighter,
                    IgnorePressure = currentAttributes.IgnorePressure,
                    FitToCurve = currentAttributes.FitToCurve
                };
                canvas.DefaultDrawingAttributes = newAttributes;
            }
        }



        private void btn_select_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.Select;
            foreach (var textBox in canvas.Children.OfType<TextBox>())
            {
                textBox.SizeChanged += TextBox_SizeChanged;
            }
        }

        private void btn_eraserP_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void btn_eraserS_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void btn_text_Click(object sender, RoutedEventArgs e)
        {
            TextBox textbox = new TextBox
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Width = 100,
                Height = 50,
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
                Margin = new Thickness(20, 20, 0, 0)
            };
            canvas.Children.Add(textbox);
            //Переключение фокуса на элемент, чтоб можно было сразу ввести текст с клавиатуры
            textbox.Focus();
            shapeStack.Push(textbox);
        }

        private void ResizeTextBoxFont(TextBox textBox)
        {
            double fontSize = textBox.ActualHeight * 0.6; 

            if (fontSize > 0)
            {
                textBox.FontSize = fontSize;
            }
        }

        private void TextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ResizeTextBoxFont(textBox);
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            startPoint = e.GetPosition(canvas);

            if (isSettingBackgroundColor)
            {
                canvas.Background = new SolidColorBrush(Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue));
                isSettingBackgroundColor = false;
            }

            switch (currentShape)
            {
                case ShapeType.Ellipse:
                    currentShapeElement = new Ellipse
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue)),
                        StrokeThickness = lineWidth,
                        Fill = Brushes.Transparent,
                        Width = 0,
                        Height = 0
                    };
                    InkCanvas.SetLeft(currentShapeElement, startPoint.X);
                    InkCanvas.SetTop(currentShapeElement, startPoint.Y);
                    canvas.Children.Add(currentShapeElement);
                    break;
                case ShapeType.Triangle:
                    currentShapeElement = new Polygon
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue)),
                        StrokeThickness = lineWidth,
                        Fill = Brushes.Transparent
                    };
                    PointCollection points = new PointCollection
                        {
                            startPoint,
                            new Point(startPoint.X + 100, startPoint.Y),
                            new Point(startPoint.X + 50, startPoint.Y + 100)
                        };
                    ((Polygon)currentShapeElement).Points = points;
                    canvas.Children.Add(currentShapeElement);
                    break;
                case ShapeType.Line:
                    currentShapeElement = new Line
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue)),
                        StrokeThickness = lineWidth,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y
                    };
                    canvas.Children.Add(currentShapeElement);
                    break;
                case ShapeType.Rectangle:
                    currentShapeElement = new Rectangle
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue)),
                        StrokeThickness = lineWidth,
                        Fill = Brushes.Transparent,
                        Width = 0,
                        Height = 0
                    };
                    InkCanvas.SetLeft(currentShapeElement, startPoint.X);
                    InkCanvas.SetTop(currentShapeElement, startPoint.Y);
                    canvas.Children.Add(currentShapeElement);
                    break;
                default:
                    break;
            }
            
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentShapeElement != null)
            {
                double width = e.GetPosition(canvas).X - startPoint.X;
                double height = e.GetPosition(canvas).Y - startPoint.Y;

                switch (currentShape)
                {
                    case ShapeType.Ellipse:
                        double left = startPoint.X;
                        double top = startPoint.Y;
                        if (width < 0)
                            left += width;
                        if (height < 0)
                            top += height;
                        currentShapeElement.Width = Math.Abs(width);
                        currentShapeElement.Height = Math.Abs(height);
                        InkCanvas.SetLeft(currentShapeElement, left);
                        InkCanvas.SetTop(currentShapeElement, top);
                        break;
                    case ShapeType.Triangle:
                        PointCollection points = new PointCollection
                            {
                                startPoint,
                                new Point(e.GetPosition(canvas).X, startPoint.Y),
                                new Point(startPoint.X + (e.GetPosition(canvas).X - startPoint.X) / 2, e.GetPosition(canvas).Y)
                            };
                        ((Polygon)currentShapeElement).Points = points;
                        break;
                    case ShapeType.Line:
                        ((Line)currentShapeElement).X2 = e.GetPosition(canvas).X;
                        ((Line)currentShapeElement).Y2 = e.GetPosition(canvas).Y;
                        break;
                    case ShapeType.Rectangle:
                        double leftRect = startPoint.X;
                        double topRect = startPoint.Y;
                        if (width < 0)
                            leftRect += width;
                        if (height < 0)
                            topRect += height;
                        currentShapeElement.Width = Math.Abs(width);
                        currentShapeElement.Height = Math.Abs(height);
                        InkCanvas.SetLeft(currentShapeElement, leftRect);
                        InkCanvas.SetTop(currentShapeElement, topRect);
                        break;
                    default:
                        break;
                }
            }
        }

        private void btn_ellipse_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.None;
            currentShape = ShapeType.Ellipse;
        }

        private void btn_triangle_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.None;
            currentShape = ShapeType.Triangle;
        }

        private void btn_line_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.None;
            currentShape = ShapeType.Line;
        }

        private void btn_rectangle_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.None;
            currentShape = ShapeType.Rectangle;
        }

        
        private Stack<UIElement> shapeStack = new Stack<UIElement>();
        private Stack<UIElement> undoneShapeStack = new Stack<UIElement>();
        private bool isDrawing = false;

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (shapeStack.Count > 0)
            {
                var lastShape = shapeStack.Pop();
                canvas.Children.Remove(lastShape);
                undoneShapeStack.Push(lastShape);
            }
        }

        private void btn_forward_Click(object sender, RoutedEventArgs e)
        {
            if (undoneShapeStack.Count > 0)
            {
                var shapeToRestore = undoneShapeStack.Pop();
                canvas.Children.Add(shapeToRestore);
                shapeStack.Push(shapeToRestore);
            }
        }
        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing && currentShapeElement != null)
            {
                shapeStack.Push(currentShapeElement);
                currentShapeElement = null;
                isDrawing = false;
            }
        }
        private void btn_background_Click(object sender, RoutedEventArgs e)
        {
            canvas.EditingMode = InkCanvasEditingMode.None;
            isSettingBackgroundColor = true;
        }
    }
}
