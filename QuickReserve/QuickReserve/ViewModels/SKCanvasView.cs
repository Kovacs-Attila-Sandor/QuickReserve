using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

public class BlurView : ContentView
{
    public static readonly BindableProperty BlurRadiusProperty =
        BindableProperty.Create(nameof(BlurRadius), typeof(float), typeof(BlurView), 10f);

    public float BlurRadius
    {
        get => (float)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    public BlurView()
    {
        var canvasView = new SKCanvasView();
        canvasView.PaintSurface += OnPaintSurface;

        Content = canvasView;
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        // Get dimensions
        var info = e.Info;
        var rect = new SKRect(0, 0, info.Width, info.Height);

        // Apply blur effect
        using (var paint = new SKPaint
        {
            IsAntialias = true,
            ImageFilter = SKImageFilter.CreateBlur(BlurRadius, BlurRadius)
        })
        {
            canvas.DrawRect(rect, paint);
        }
    }
}
