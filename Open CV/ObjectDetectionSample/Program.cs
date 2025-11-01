using OpenCvSharp;
using System;

class Program
{
    static void Main()
    {
        using var capture = new VideoCapture(0);
        if (!capture.IsOpened())
        {
            Console.WriteLine("Kamera başlatılamadı!");
            return;
        }

        using var window = new Window("Kamera");

        Mat frame = new Mat();

        while (true)
        {
            capture.Read(frame);
            if (frame.Empty())
                continue;

            var hsv = new Mat();
            Cv2.CvtColor(frame, hsv, ColorConversionCodes.BGR2HSV);

            // --- KIRMIZI ---
            Scalar lowerRed1 = new Scalar(0, 120, 70);
            Scalar upperRed1 = new Scalar(10, 255, 255);
            Scalar lowerRed2 = new Scalar(170, 120, 70);
            Scalar upperRed2 = new Scalar(180, 255, 255);
            Mat redMask1 = new Mat();
            Mat redMask2 = new Mat();
            Cv2.InRange(hsv, lowerRed1, upperRed1, redMask1);
            Cv2.InRange(hsv, lowerRed2, upperRed2, redMask2);
            Cv2.BitwiseOr(redMask1, redMask2, redMask1);
            Cv2.FindContours(redMask1, out Point[][] redContours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            foreach (var contour in redContours)
            {
                var area = Cv2.ContourArea(contour);
                if (area > 500)
                {
                    var rect = Cv2.BoundingRect(contour);
                    Cv2.Rectangle(frame, rect, new Scalar(0,0,255), 2); // KIRMIZI DIKDRTGEN
                    Cv2.PutText(frame, "KIRMIZI", new Point(rect.X, rect.Y - 10), HersheyFonts.HersheySimplex, 0.7, new Scalar(0,0,255), 2);
                }
            }

            // --- MAVI ---
            Scalar lowerBlue = new Scalar(100, 150, 50); // ALT HSV
            Scalar upperBlue = new Scalar(140, 255, 255); // ÜST HSV
            Mat blueMask = new Mat();
            Cv2.InRange(hsv, lowerBlue, upperBlue, blueMask);
            Cv2.FindContours(blueMask, out Point[][] blueContours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            foreach (var contour in blueContours)
            {
                var area = Cv2.ContourArea(contour);
                if (area > 500)
                {
                    var rect = Cv2.BoundingRect(contour);
                    Cv2.Rectangle(frame, rect, new Scalar(255,0,0), 2); // MAVI DIKDRTGEN
                    Cv2.PutText(frame, "MAVI", new Point(rect.X, rect.Y - 10), HersheyFonts.HersheySimplex, 0.7, new Scalar(255,0,0), 2);
                }
            }

            // --- SIYAH ---
            Scalar lowerBlack = new Scalar(0, 0, 0);
            Scalar upperBlack = new Scalar(180, 255, 50); // SIYAH için düşük parlaklık
            Mat blackMask = new Mat();
            Cv2.InRange(hsv, lowerBlack, upperBlack, blackMask);
            Cv2.FindContours(blackMask, out Point[][] blackContours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            foreach (var contour in blackContours)
            {
                var area = Cv2.ContourArea(contour);
                if (area > 500)
                {
                    var rect = Cv2.BoundingRect(contour);
                    Cv2.Rectangle(frame, rect, new Scalar(0,0,0), 2); // SIYAH DIKDRTGEN (kenar gri renk yapılabilir)
                    Cv2.PutText(frame, "SIYAH", new Point(rect.X, rect.Y - 10), HersheyFonts.HersheySimplex, 0.7, new Scalar(128,128,128), 2);
                }
            }

            window.ShowImage(frame);

            int key = Cv2.WaitKey(1);
            if (key == 27)
                break;
        }
    }
}
