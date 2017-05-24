using System.Diagnostics;
using System.IO;
using System.Linq;
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
                    byte[] colorRgba = new byte[ColorWidth * ColorHeight * 4];

                    // Determine if this frame was a dropped frame
                    int frameIndex = _frameAnalysis.FrameMap[(int) index];

                    var currEvent = stream.ReadEvent((uint) frameIndex);
                    currEvent.CopyEventDataToArray(colorData, 0);

                    for (int i = 0; i < (ColorWidth * ColorHeight) / 2; i++) {
                        int y0 = colorData[(i << 2) + 0] - 16;
                        int u = colorData[(i << 2) + 1] - 128;
                        int y1 = colorData[(i << 2) + 2] - 16;
                        int v = colorData[(i << 2) + 3] - 128;

                        byte r = Utils.ClipToByte((298 * y0 + 409 * v + 128) >> 8);
                        byte g = Utils.ClipToByte((298 * y0 - 100 * u - 208 * v + 128) >> 8);
                        byte b = Utils.ClipToByte((298 * y0 + 516 * u + 128) >> 8);

                        colorRgba[(i << 3) + 0] = b;
                        colorRgba[(i << 3) + 1] = g;
                        colorRgba[(i << 3) + 2] = r;
                        colorRgba[(i << 3) + 3] = 0xFF; // A

                        r = Utils.ClipToByte((298 * y1 + 409 * v + 128) >> 8);
                        g = Utils.ClipToByte((298 * y1 - 100 * u - 208 * v + 128) >> 8);
                        b = Utils.ClipToByte((298 * y1 + 516 * u + 128) >> 8);

                        colorRgba[(i << 3) + 4] = b;
                        colorRgba[(i << 3) + 5] = g;
                        colorRgba[(i << 3) + 6] = r;
                        colorRgba[(i << 3) + 7] = 0xFF;
                    }

                    int stride = (PixelFormats.Bgr32.BitsPerPixel) * 1920 / 8;
                    colorBitmap.WritePixels(new Int32Rect(0, 0, 1920, 1080), colorRgba, stride, 0);

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
            int lastGoodEvent = 0;
            int eventIndex = 0;
            FrameAnalysis result = new FrameAnalysis();

            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            foreach (KStudioEventHeader header in stream.EventHeaders) {
                double currentFrameTime = header.RelativeTime.TotalMilliseconds;
                double deltaTime = currentFrameTime - prevFrameTime;

                cumulativeTime += deltaTime - FrameTime;

                if (cumulativeTime > FrameTime) {
                    do {
                        result.DroppedFrames++;
                        cumulativeTime -= FrameTime;
                        result.FrameMap.Add(lastGoodEvent);
                        result.DroppedIndices.Add(result.FrameCount++);
                    } while (cumulativeTime > FrameTime);
                } else {
                    lastGoodEvent = eventIndex;
                    result.FrameMap.Add(lastGoodEvent);
                    result.FrameCount++;
                }

                eventIndex++;
                prevFrameTime = currentFrameTime;
            }

            return result;
        }
    }
}