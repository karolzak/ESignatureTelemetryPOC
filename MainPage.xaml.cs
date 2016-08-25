using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SigTestWin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public bool imageCropped = false;
        public int X1 { get; set; }
        public int Y1 { get; set; }

        public bool PressureSensitive {get;set;}

        // Scenario specific constants and variables.
        private double _strokethickness =3;
        public double STROKETHICKNESS { 
            get
            {
                return _strokethickness;
            }
            set 
            {
                _strokethickness = value * 0.01;
            }
        }
        Line line1, line2, line3, line4;
        Point _previousContactPt;
        uint penId = 0;
        uint touchId = 0;
        private List<TouchData> list;
            // Create the InkManager instance.
        InkManager inkManager = new Windows.UI.Input.Inking.InkManager();
        private DateTime _unixTime;
        private Color _strokeColor = Color.FromArgb(255, 0, 46, 184);
        private bool presssed;
        private SolidColorBrush brush;
        public MainPage()
        {
            this.InitializeComponent();
            slider1.Value = STROKETHICKNESS * 100;
            PressureSensitive = false;
            // Add pointer event handlers to the Canvas object.
            InkCanvas.PointerPressed += new PointerEventHandler(InkCanvas_PointerPressed);
            InkCanvas.PointerMoved += new PointerEventHandler(InkCanvas_PointerMoved);
            InkCanvas.PointerReleased += new PointerEventHandler(InkCanvas_PointerReleased);
            InkCanvas.PointerExited += new PointerEventHandler(InkCanvas_PointerReleased);
            list = new List<TouchData>(1000);
            brush = new SolidColorBrush(_strokeColor);
            this._unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            ClearButton.Width = InkCanvas.Width;
            SaveButton.Width = InkCanvas.Width;
            
            inkManager.SetDefaultDrawingAttributes(new InkDrawingAttributes {Color = _strokeColor, FitToCurve = false, Size = new Size(STROKETHICKNESS, STROKETHICKNESS), IgnorePressure = true});
            X1 = 600;
            slider2.Value = X1;
            Y1 = 200;
            slider3.Value = Y1;
            
        }


        private void RemoveSignRect()
        {
            try
            {
                InkCanvas.Children.Remove(line1);
                InkCanvas.Children.Remove(line2);
                InkCanvas.Children.Remove(line3);
                InkCanvas.Children.Remove(line4);
            }
            catch { }
        }
        private void CalcSignRect()
        {
            try
            {
                line1 = new Line()
                 {
                     X1 = (InkCanvas.Width / 2) - (X1 / 2),
                     Y1 = (InkCanvas.Height / 2) - (Y1 / 2),
                     X2 = (InkCanvas.Width / 2) + (X1 / 2),
                     Y2 = (InkCanvas.Height / 2) - (Y1 / 2),
                     Stroke = new SolidColorBrush(Colors.Red),
                     StrokeThickness = 2,

                     StrokeLineJoin = PenLineJoin.Round,
                     StrokeDashCap = PenLineCap.Round,
                     StrokeEndLineCap = PenLineCap.Round,
                     StrokeStartLineCap = PenLineCap.Round,
                 };

                line2 = new Line()
                  {
                      X1 = (InkCanvas.Width / 2) - (X1 / 2),
                      Y1 = (InkCanvas.Height / 2) + (Y1 / 2),
                      X2 = (InkCanvas.Width / 2) + (X1 / 2),
                      Y2 = (InkCanvas.Height / 2) + (Y1 / 2),
                      Stroke = new SolidColorBrush(Colors.Red),
                      StrokeThickness = 2,

                      StrokeLineJoin = PenLineJoin.Round,
                      StrokeDashCap = PenLineCap.Round,
                      StrokeEndLineCap = PenLineCap.Round,
                      StrokeStartLineCap = PenLineCap.Round,
                  };

                line3 = new Line()
                  {
                      X1 = (InkCanvas.Width / 2) - (X1 / 2),
                      Y1 = (InkCanvas.Height / 2) - (Y1 / 2),
                      X2 = (InkCanvas.Width / 2) - (X1 / 2),
                      Y2 = (InkCanvas.Height / 2) + (Y1 / 2),
                      Stroke = new SolidColorBrush(Colors.Red),
                      StrokeThickness = 2,

                      StrokeLineJoin = PenLineJoin.Round,
                      StrokeDashCap = PenLineCap.Round,
                      StrokeEndLineCap = PenLineCap.Round,
                      StrokeStartLineCap = PenLineCap.Round,
                  };

                line4 = new Line()
                 {
                     X1 = (InkCanvas.Width / 2) + (X1 / 2),
                     Y1 = (InkCanvas.Height / 2) - (Y1 / 2),
                     X2 = (InkCanvas.Width / 2) + (X1 / 2),
                     Y2 = (InkCanvas.Height / 2) + (Y1 / 2),
                     Stroke = new SolidColorBrush(Colors.Red),
                     StrokeThickness = 2,

                     StrokeLineJoin = PenLineJoin.Round,
                     StrokeDashCap = PenLineCap.Round,
                     StrokeEndLineCap = PenLineCap.Round,
                     StrokeStartLineCap = PenLineCap.Round,
                 };
            }
            catch { }
        }

        private void DrawSignRect()
        {
            try
            {

                InkCanvas.Children.Add(
                 line1
               );

                InkCanvas.Children.Add(
                 line2
               );

                InkCanvas.Children.Add(
                 line3
               );

                InkCanvas.Children.Add(
                line4
               );
            }
            catch { }
        }

        private long ToUnixTimestamp(DateTime timestamp)
        {
            return (long)(timestamp - _unixTime).TotalMilliseconds;
        }
        public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(InkCanvas);
            _previousContactPt = pt.Position;
            // Accept input only from a pen or mouse with the left button pressed. 
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen ||
                    pointerDevType == PointerDeviceType.Mouse &&
                    pt.Properties.IsLeftButtonPressed)
            {
                // Pass the pointer information to the InkManager.
                //inkManager.ProcessPointerDown(pt);
                presssed = true;
                penId = pt.PointerId;
                list.Add(new TouchData(DateTime.Now, pt.Position.X, pt.Position.Y, pt.Properties.Pressure));
                //textBlock.Text = pt.Properties.Pressure.ToString();
                e.Handled = true;
            }
            
        }

        public void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen ||
                    pointerDevType == PointerDeviceType.Mouse )
            {
                if (presssed)
                {

                    var pt = e.GetCurrentPoint(InkCanvas);
                    list.Add(new TouchData(DateTime.Now, pt.Position.X, pt.Position.Y, pt.Properties.Pressure));
                    var pts = e.GetIntermediatePoints(InkCanvas).Select(el => el.Position);

                    //sprawdzanie czy ma wykrywać siłę nacisku czy nie
                    double tempThickness = STROKETHICKNESS;
                    if (PressureSensitive)
                    {
                        tempThickness = STROKETHICKNESS * pt.Properties.Pressure;
                    }

                    foreach (var point in pts)
                    {
                        InkCanvas.Children.Add(
              new Line()
              {
                  X1 = _previousContactPt.X,
                  Y1 = _previousContactPt.Y,
                  X2 = point.X,
                  Y2 = point.Y,
                  Stroke = brush,
                  StrokeThickness = tempThickness,
                  StrokeLineJoin = PenLineJoin.Round,
                  StrokeDashCap = PenLineCap.Round,
                  StrokeEndLineCap = PenLineCap.Round,
                  StrokeStartLineCap = PenLineCap.Round,
              }
            );
                        _previousContactPt = point;
                    }
                }
                e.Handled = true;
            }
        }


        public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen ||
                    pointerDevType == PointerDeviceType.Mouse )
            {
                if (presssed)
                {
                    PointerPoint pt = e.GetCurrentPoint(InkCanvas);
                    list.Add(new TouchData(DateTime.Now, pt.Position.X, pt.Position.Y, pt.Properties.Pressure));
                }

                penId = 0;
                presssed = false;
                e.Handled = true;
            }
        }

        //Render ink strokes as cubic bezier segments.


        //public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    // Get information about the pointer location.
        //    PointerPoint pt = e.GetCurrentPoint(InkCanvas);
        //    _previousContactPt = pt.Position;

        //    // Accept input only from a pen or mouse with the left button pressed. 
        //    PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
        //    if (pointerDevType == PointerDeviceType.Pen ||
        //            pointerDevType == PointerDeviceType.Mouse &&
        //            pt.Properties.IsLeftButtonPressed)
        //    {
        //        // Pass the pointer information to the InkManager.
        //        inkManager.ProcessPointerDown(pt);
        //        penId = pt.PointerId;
        //        list.Add(new TouchData(DateTime.Now, pt.Position.X, pt.Position.Y, pt.Properties.Pressure));
        //        //textBlock.Text = pt.Properties.Pressure.ToString();
        //        e.Handled = true;
        //    }
        //}

        //public void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    if (e.Pointer.PointerId == penId)
        //    {
        //        PointerPoint pt = e.GetCurrentPoint(InkCanvas);

        //        // Render a red line on the canvas as the pointer moves. 
        //        // Distance() is an application-defined function that tests
        //        // whether the pointer has moved far enough to justify 
        //        // drawing a new line.
        //        Point currentContactPt = pt.Position;
        //        if (Distance(currentContactPt, _previousContactPt) > 0.00000001)
        //        {
        //            list.Add(new TouchData(DateTime.Now, pt.Position.X, pt.Position.Y, pt.Properties.Pressure));
        //            //textBlock.Text = pt.Properties.Pressure.ToString();
        //            Line line = new Line()
        //            {
        //                X1 = _previousContactPt.X,
        //                Y1 = _previousContactPt.Y,
        //                X2 = currentContactPt.X,
        //                Y2 = currentContactPt.Y,
        //                StrokeThickness = STROKETHICKNESS,
        //                Stroke = new SolidColorBrush(_strokeColor),

        //            };

        //            _previousContactPt = currentContactPt;

        //            // Draw the line on the canvas by adding the Line object as
        //            // a child of the Canvas object.
        //            InkCanvas.Children.Add(line);

        //            // Pass the pointer information to the InkManager.
        //            inkManager.ProcessPointerUpdate(pt);
        //        }
        //    }
        //    e.Handled = true;
        //}


        //public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        //{
        //    if (e.Pointer.PointerId == penId)
        //    {
        //        PointerPoint pt = e.GetCurrentPoint(InkCanvas);
        //        list.Add(new TouchData(DateTime.Now, pt.Position.X, pt.Position.Y, pt.Properties.Pressure));
        //        //textBlock.Text = pt.Properties.Pressure.ToString();
        //        // Pass the pointer information to the InkManager. 
        //        inkManager.ProcessPointerUp(pt);
        //    }

        //    else if (e.Pointer.PointerId == touchId)
        //    {
        //        // Process touch input
        //    }

        //    touchId = 0;
        //    penId = 0;

        //    // Call an application-defined function to render the ink strokes.
        //    RenderAllStrokes();

        //    e.Handled = true;
        //}

        // Render ink strokes as cubic bezier segments.
        private void RenderAllStrokes()
        {
            // Clear the canvas.
            InkCanvas.Children.Clear();

            // Get the InkStroke objects.
            IReadOnlyList<InkStroke> inkStrokes = inkManager.GetStrokes();

            // Process each stroke.
            foreach (InkStroke inkStroke in inkStrokes)
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigureCollection pathFigures = new PathFigureCollection();
                PathFigure pathFigure = new PathFigure();
                PathSegmentCollection pathSegments = new PathSegmentCollection();

                // Create a path and define its attributes.
                Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
                path.Stroke = new SolidColorBrush(_strokeColor);
                path.StrokeThickness = STROKETHICKNESS;
                inkStroke.DrawingAttributes.Size = new Size(STROKETHICKNESS, STROKETHICKNESS);
                // Get the stroke segments.
                IReadOnlyList<InkStrokeRenderingSegment> segments;
                segments = inkStroke.GetRenderingSegments();

                // Process each stroke segment.
                bool first = true;
                foreach (InkStrokeRenderingSegment segment in segments)
                {
                    // The first segment is the starting point for the path.
                    if (first)
                    {
                        pathFigure.StartPoint = segment.BezierControlPoint1;
                        first = false;
                    }

                    // Copy each ink segment into a bezier segment.
                    BezierSegment bezSegment = new BezierSegment();
                    bezSegment.Point1 = segment.BezierControlPoint1;
                    bezSegment.Point2 = segment.BezierControlPoint2;
                    bezSegment.Point3 = segment.Position;

                    // Add the bezier segment to the path.
                    pathSegments.Add(bezSegment);
                }

                // Build the path geometerty object.
                pathFigure.Segments = pathSegments;
                pathFigures.Add(pathFigure);
                pathGeometry.Figures = pathFigures;

                // Assign the path geometry object as the path data.
                path.Data = pathGeometry;

                // Render the path by adding it as a child of the Canvas object.
                InkCanvas.Children.Add(path);
            }
        }

        private double Distance(Point currentContact, Point previousContact)
        {
            return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) +
                    Math.Pow(currentContact.Y - previousContact.Y, 2));
        }

        async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.InkCanvas.Children.Count == 0)
            {
                var message = new MessageDialog("Brak podpisu");
                await message.ShowAsync();
            }
            else
            {
                try
                {




                    var sb = new StringBuilder("Timestamp,Pressure,X,Y" + Environment.NewLine);
                    foreach (var touchData in list)
                    {
                        sb.AppendFormat("{0},{1},{2},{3}{4}",/* touchData.Timestamp.Second +":"+touchData.Timestamp.Millisecond */this.ToUnixTimestamp(touchData.Timestamp), touchData.Pressure, touchData.X,
                            touchData.Y, Environment.NewLine);
                    }

                    FileSavePicker savePicker = new FileSavePicker
                    {
                        SuggestedStartLocation = PickerLocationId.Desktop,
                        SuggestedFileName = DateTime.Now.ToString("yyyy-MM-dd HHmmss")
                    };
                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add("csv", new List<string> { ".csv" });
                    // Default file name if the user does not type one in or select a file to replace
                    
                    StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                        CachedFileManager.DeferUpdates(file);
                        // write to file
                        await FileIO.WriteTextAsync(file, sb.ToString());
                        // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                        // Completing updates may require Windows to ask for user input.
                        await CachedFileManager.CompleteUpdatesAsync(file);
                        //this.Clear();
                    }

                    savePicker.FileTypeChoices.Clear();
                    savePicker.FileTypeChoices.Add("png", new List<string> {".png"});
                    file = await savePicker.PickSaveFileAsync();
                    if (file == null) return;

                    //IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    //IOutputStream outputStream = writeStream.GetOutputStreamAt(0);

                    //await inkManager.SaveAsync(outputStream);
                    //await outputStream.FlushAsync();

                    //////RenderTargetBitmap rtb = new RenderTargetBitmap();
                    
                    //////await rtb.RenderAsync(InkCanvas);
                    //////var buffer = await rtb.GetPixelsAsync();
                    //////var arr = buffer.ToArray();

                    //////var ff = await file.OpenAsync(FileAccessMode.ReadWrite);
                    
                    //////var dd = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ff);
                    //////dd.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)rtb.PixelWidth,
                    //////    (uint)rtb.PixelHeight, 96, 96, arr);

                    //////await dd.FlushAsync();
                    //////ff.Dispose();

                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                    await renderTargetBitmap.RenderAsync(InkCanvas);
                    

                    WriteableBitmap bitmapImage = new WriteableBitmap(renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight);

                    IBuffer pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderTargetBitmap.PixelWidth, (uint)renderTargetBitmap.PixelHeight, 96, 96, pixelBuffer.ToArray());

                        await encoder.FlushAsync();
                        stream.Seek(0);

                        bitmapImage.SetSource(stream);
                        bitmapImage.Invalidate();

                        WriteableBitmap croppedBitmap;

                        if (imageCropped)
                            croppedBitmap = bitmapImage.Crop(new Rect((InkCanvas.Width / 2) - (X1 / 2), (InkCanvas.Height / 2) - (Y1 / 2), X1, Y1));
                        else
                            croppedBitmap = bitmapImage;

                        using (IRandomAccessStream ff = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            BitmapEncoder encoder1 = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ff);
                            Stream pixelStream = croppedBitmap.PixelBuffer.AsStream();
                            byte[] pixels = new byte[pixelStream.Length];
                            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                            encoder1.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                                (uint)croppedBitmap.PixelWidth,
                                                (uint)croppedBitmap.PixelHeight,
                                                96.0,
                                                96.0,
                                                pixels);
                            await encoder1.FlushAsync();
                        }

                    }

                    

                    // Redraw the WriteableBitmap


                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                    //CachedFileManager.DeferUpdates(file);

                    //IRandomAccessStream writeStream = new InMemoryRandomAccessStream();//await file.OpenAsync(FileAccessMode.ReadWrite);
                    //IOutputStream outputStream = writeStream.GetOutputStreamAt(0);

                    //await inkManager.SaveAsync(outputStream);
                    //await outputStream.FlushAsync();
                    //var s = await file.OpenAsync(FileAccessMode.ReadWrite);
                    //var str = writeStream.GetInputStreamAt(0).AsStreamForRead();

                    //var ms = new MemoryStream();

                    //    await str.CopyToAsync(ms);
                    //var ddd = ms.ToArray();


                    //var dd = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, s);
                    //dd.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)InkCanvas.ActualWidth,
                    //    (uint)InkCanvas.ActualHeight, 32, 32, ddd);

                    //await dd.FlushAsync();
                    //s.Dispose();
                    this.ClearButton_Click(null, new RoutedEventArgs());
                }
                catch (Exception ex)
                {
                    var message = new MessageDialog(ex.ToString());
                    message.ShowAsync();
                }
                
            }
            
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Clear();
        }

        private void Clear()
        {
            list.Clear();
            foreach (var inkStroke in this.inkManager.GetStrokes())
            {
                inkStroke.Selected = true;
            }
            this.inkManager.DeleteSelected();
            this.InkCanvas.Children.Clear();
            CalcSignRect();
            DrawSignRect();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            STROKETHICKNESS = e.NewValue ;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            PressureSensitive = switch1.IsOn;
        }

        private void Slider_WidthChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            X1 = Convert.ToInt32(e.NewValue);
            RemoveSignRect();
            CalcSignRect();
            DrawSignRect();
        }

        private void Slider_HeightChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Y1 = Convert.ToInt32(e.NewValue);
            RemoveSignRect();
            CalcSignRect();
            DrawSignRect();
        }

        private void ToggleSwitch2_Toggled(object sender, RoutedEventArgs e)
        {
            imageCropped = switch2.IsOn;
        }

    }

    public struct TouchData
    {
        public DateTime Timestamp { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public float Pressure { get; set; }

        public TouchData(DateTime timestamp, double x, double y, float pressure) : this()
        {
            Timestamp = timestamp;
            X = x;
            Y = y;
            Pressure = pressure;
        }
    }
}
