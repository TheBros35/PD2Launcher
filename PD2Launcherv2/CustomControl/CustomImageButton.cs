using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PD2Launcherv2.CustomControl
{
    public class CustomImageButton : Button
    {
        static CustomImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomImageButton), new FrameworkPropertyMetadata(typeof(CustomImageButton)));
        }

        public static readonly DependencyProperty NormalImageSourceProperty = DependencyProperty.Register(
            "NormalImageSource",
            typeof(ImageSource),
            typeof(CustomImageButton),
            new PropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty PressedImageSourceProperty = DependencyProperty.Register(
            "PressedImageSource",
            typeof(ImageSource),
            typeof(CustomImageButton),
            new PropertyMetadata(default(ImageSource)));

        public ImageSource NormalImageSource
        {
            get => (ImageSource)GetValue(NormalImageSourceProperty);
            set => SetValue(NormalImageSourceProperty, value);
        }

        public ImageSource PressedImageSource
        {
            get => (ImageSource)GetValue(PressedImageSourceProperty);
            set => SetValue(PressedImageSourceProperty, value);
        }
    }
}
