using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private ColorRGB mcolor;
        private bool isClosingConfirmed = false;
        public MainWindow()
        {
            InitializeComponent();

            mcolor = new ColorRGB();
            mcolor.Red = 0;
            mcolor.Green = 0;
            mcolor.Blue = 0;

            curColor.Background = new SolidColorBrush(Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue));

            // Установка значений для слайдеров
            sldrRed.Value = mcolor.Red;
            sldrGreen.Value = mcolor.Green;
            sldrBlue.Value = mcolor.Blue;


            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!isClosingConfirmed)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to close the window?", "Closing confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; 
                }
                else
                {
                    isClosingConfirmed = true;
                }
            }
        }

        //private List<ColorInfo> GetRGBColorItems()
        //{
        //    List<ColorInfo> colorItems = new List<ColorInfo>();
        //    for (byte r = 0; r <= 255; r++)
        //    {
        //        for (byte g = 0; g <= 255; g++)
        //        {
        //            for (byte b = 0; b <= 255; b++)
        //            {
        //                Color color = Color.FromRgb(r, g, b);
        //                colorItems.Add(new ColorInfo(color.ToString(), color));
        //            }
        //        }
        //    }
        //    return colorItems;
        //}

        private void sld_Color_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            string crlName = slider.Name; // Определяем имя контрола, который покрутили
            double value = slider.Value; // Получаем значение контрола

            // В зависимости от выбранного контрола, меняем ту или иную компоненту и переводим ее в тип byte
            if (crlName.Equals("sldrRed"))
                mcolor.Red = Convert.ToByte(value);

            if (crlName.Equals("sldrGreen"))
                mcolor.Green = Convert.ToByte(value);
            
            if (crlName.Equals("sldrBlue"))
                mcolor.Blue = Convert.ToByte(value);

            // Задаем значение переменной класса clr 
            Color clr = Color.FromRgb(mcolor.Red, mcolor.Green, mcolor.Blue);

            // Устанавливаем фон для контрола Label 
            curColor.Background = new SolidColorBrush(clr);

            // Задаем цвет кисти для контрола InkCanvas
            canvas.DefaultDrawingAttributes.Color = clr;

        }


    }

    public class ColorInfo
    {
        public string ColorName { get; set; }
        public Color Color { get; set; }

        public ColorInfo(string colorName, Color color)
        {
            ColorName = colorName;
            Color = color;
        }
    }

    public class ColorRGB
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
    }
}

