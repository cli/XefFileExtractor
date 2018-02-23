using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Kinect.Tools;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace XefFileExtractor.Domain {
    public class KinectDepth : KinectStream {
        private const int DepthWidth = 512; // Width of the depth image
        private const int DepthHeight = 424; // Height of the depth image
        private const int DepthBytesPerPixel = 2;

        public KinectDepth(KStudioEventStream stream) : base(stream) { }

        public override unsafe void Extract(string outputPath) {
            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            int frameCount = (int) stream.EventCount;
            double[] depthTiming = new double[frameCount];

            var values = Enumerable.Range(0, frameCount);

            Parallel.ForEach(values, (value, pls, index) => {
                WriteableBitmap depthBitmap = new WriteableBitmap(DepthWidth, DepthHeight, 96.0, 96.0,
                    PixelFormats.Gray16, null);
                var currEvent = stream.ReadEvent((uint) index);
                byte[] depthPixels = new byte[DepthWidth * DepthHeight];

                IntPtr buffer = Marshal.AllocHGlobal((int) currEvent.EventDataSize);

                currEvent.CopyEventDataToBuffer(currEvent.EventDataSize, buffer);

                //ushort* frameData = (ushort*)buffer;

                //for (int i = 0; i < (int) (currEvent.EventDataSize / DepthBytesPerPixel); ++i)
                //{
                //    ushort depth = frameData[i];

                //    depthPixels[i] = (byte)(depth >= 256 && depth <= 8000 ? depth / (8000 / 256) : 0);
                //}

                //currEvent.CopyEventDataToArray(depthPixels, 0);
                depthBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, DepthWidth, DepthHeight), buffer, (int) currEvent.EventDataSize, depthBitmap.BackBufferStride);

                // create a png bitmap encoder which knows how to save a .png file
                BitmapEncoder encoder = new PngBitmapEncoder();
                //TiffBitmapEncoder
                //BitmapEncoder encoder = new BmpBitmapEncoder();

                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(depthBitmap));

                string filePath = outputPath + "/Kinect_Output/Depth/";
                Directory.CreateDirectory(filePath);
                string path = Path.Combine(filePath, "DepthFrame_" + index + ".png");

                // write the new file to disk
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (IOException) { }

                depthTiming[index] = currEvent.RelativeTime.TotalMilliseconds;

                // Update progress
                OnProgressUpdated(new KinectFileProgressChangedEventArgs {
                    Progress = (int) ((float) (index + 1) / frameCount * 100),
                    StatusName = Name
                });
            });

            // Write timings
            string filepath = outputPath + "\\Kinect_Output";
            Utils.WriteTimingToFile(filepath + "\\depth_timing.txt", depthTiming);
        }

        public override FrameAnalysis CountDroppedFrames() {
            throw new System.NotImplementedException();
        }
    }
}