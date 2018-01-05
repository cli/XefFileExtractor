using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public class KinectColor : KinectStream {
        private const int ColorWidth = 1920;
        private const int ColorHeight = 1080;
        private const double FrameTime = (double) 1000 / 30;

        private FrameAnalysis _frameAnalysis;

        public KinectColor(KStudioEventStream stream) : base(stream) {
            _frameAnalysis = CountDroppedFrames();
        }

        public override void Extract(string outputPath) {
            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;
            int frameCount = _frameAnalysis.FrameCount;
            double[] colorTiming = new double[frameCount];
            var values = Enumerable.Range(0, frameCount);

            Parallel.ForEach(values, new ParallelOptions {MaxDegreeOfParallelism = 8},
                (value, pls, index) => {
                    WriteableBitmap colorBitmap = new WriteableBitmap(ColorWidth, ColorHeight, 96.0, 96.0,
                        PixelFormats.Bgr32,
                        null);
                    byte[] colorData = new byte[ColorWidth * ColorHeight * 2];

                    // Determine if this frame was a dropped frame
                    int frameIndex = _frameAnalysis.FrameMap[(int) index];

                    var currEvent = stream.ReadEvent((uint) frameIndex);
                    currEvent.CopyEventDataToArray(colorData, 0);

                    NativeMethods.YUY2ToBGR(colorData, 1920 * 1080 / 2, colorBitmap.BackBuffer, 1920 * 1080 * 4);
                    colorBitmap.WritePixels(new Int32Rect(0, 0, 1920, 1080), colorBitmap.BackBuffer, 1920 * 1080 * 4, colorBitmap.BackBufferStride);

                    // create a png bitmap encoder which knows how to save a .png file
                    //BitmapEncoder encoder = new PngBitmapEncoder();
                    BitmapEncoder encoder = new BmpBitmapEncoder();

                    // create frame from the writable bitmap and add to encoder
                    encoder.Frames.Add(BitmapFrame.Create(colorBitmap));

                    string filePath = outputPath + "/Kinect_Output/Color/";
                    Directory.CreateDirectory(filePath);
                    string path = Path.Combine(filePath, "ColorFrame_" + index + ".bmp");

                    // write the new file to disk
                    try {
                        using (FileStream fs = new FileStream(path, FileMode.Create)) {
                            encoder.Save(fs);
                        }
                    }
                    catch (IOException) { }

                    colorTiming[frameIndex] = currEvent.RelativeTime.TotalMilliseconds;

                    // Update progress
                    OnProgressUpdated(new KinectFileProgressChangedEventArgs {
                        Progress = (int) ((float) ((int) index + 1) / frameCount * 100),
                        StatusName = Name
                    });
                });

            // Write timings
            string filepath = outputPath + "/Kinect_Output";
            Utils.WriteTimingToFile(filepath + "/color_timing.txt", colorTiming);
        }

        /// <summary>
        ///  Calculates dropped frame statistics based on the difference in 
        ///  timestamps between two consecutive headers.
        /// </summary>
        /// <param name="stream">
        ///  The KStudioSeekableEventStream object of the video stream
        /// </param>
        /// <param name="frameTime">The number of milliseconds per frame</param>
        /// <returns>
        ///  FrameAnalysis object with collected frame statistics.
        /// </returns>
        public override FrameAnalysis CountDroppedFrames() {
            double prevFrameTime = 0;
            double cumulativeTime = 0;
            int eventIndex = 0;
            FrameAnalysis result = new FrameAnalysis();

            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            prevFrameTime = stream.StartRelativeTime.TotalMilliseconds;

            foreach (KStudioEventHeader header in stream.EventHeaders) {
                double currentFrameTime = header.RelativeTime.TotalMilliseconds;
                double deltaTime = currentFrameTime - prevFrameTime;

                cumulativeTime += deltaTime - FrameTime;

                result.FrameCount++;
                result.FrameMap.Add(eventIndex);

                while (cumulativeTime > FrameTime) {
                    result.DroppedFrames++;
                    cumulativeTime -= FrameTime;
                    result.FrameMap.Add(eventIndex);
                    result.DroppedIndices.Add(result.FrameCount++);
                }

                eventIndex++;
                prevFrameTime = currentFrameTime;
            }

            return result;
        }

        private static class NativeMethods {
            [DllImport("ColorConversions.dll", CallingConvention = CallingConvention.StdCall)]
            public static extern void YUY2ToBGR(byte[] pYUYV, int width, IntPtr pRGBA, int outSize);
        }
    }
}