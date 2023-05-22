using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace graphics_editor
{
    public partial class MainWindow : Window
    {

        private void Click_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog();
                openDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All Files (*.*)|*.*";
                if (openDialog.ShowDialog() == true)
                {
                    Click_Clear(sender, e);
                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(openDialog.FileName, UriKind.Relative));
                    brush.Stretch = Stretch.Uniform; // Set the stretch mode to None
                    canvas.Background = brush;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error!");
            }
        }


        /////////////////////////////////////////////

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = ".PNG";
                saveDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All Files (*.*)|*.*";
                if (saveDialog.ShowDialog() == true)
                {
                    ToImageSource(canvas, saveDialog.FileName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error!");
            }
        }

        /////////////////////////////////////////////

        private void Click_Clear(object sender, RoutedEventArgs e)
        {
            canvas.Strokes.Clear();
            canvas.Background = null;
            shapeStack.Clear();
            undoneShapeStack.Clear();
            canvas.Children.Clear();
        }

        ////////////////////////////////////////////


        public static void ToImageSource(UIElement element, string filename)
        {
            // Получаем границы элемента визуального дерева
            Rect elementBounds = VisualTreeHelper.GetDescendantBounds(element);

            // Создаем RenderTargetBitmap с использованием размеров элемента
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)elementBounds.Width, (int)elementBounds.Height, 96d, 96d, PixelFormats.Pbgra32);

            // Создаем DrawingVisual и DrawingContext для отрисовки элемента
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Создаем VisualBrush с элементом и отрисовываем его
                VisualBrush visualBrush = new VisualBrush(element);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), elementBounds.Size));
            }

            // Рендерим элемент на RenderTargetBitmap
            renderTargetBitmap.Render(drawingVisual);

            // Создаем PngBitmapEncoder и добавляем кадр с RenderTargetBitmap
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Сохраняем изображение в файл
            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }
    }
}
