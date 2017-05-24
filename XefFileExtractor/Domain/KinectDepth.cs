using System.Linq;
using System.Threading.Tasks;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public class KinectDepth : KinectStream {
        private const int DepthWidth = 512; // Width of the depth image
        private const int DepthHeight = 424; // Height of the depth image

        public KinectDepth(KStudioEventStream stream) : base(stream) { }

        public override void Extract(string outputPath) {
            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            int frameCount = (int) stream.EventCount;
            double[] depthTiming = new double[frameCount];

            var values = Enumerable.Range(0, frameCount);

            Parallel.ForEach(values, (value, pls, index) => {
                //var currEvent = stream.ReadEvent((uint) index);
                var currEvent = stream.EventHeaders[(int) index];
                depthTiming[index] = currEvent.RelativeTime.TotalMilliseconds;

                // Update progress
                OnProgressUpdated(new KinectFileProgressChangedEventArgs {
                    Progress = (int) ((float) (index + 1) / frameCount * 100),
                    StatusName = Name
                });
            });

            // Write timings
            string filepath = outputPath + "/Kinect_Output";
            Utils.WriteTimingToFile(filepath + "/depth_timing.txt", depthTiming);
        }

        public override FrameAnalysis CountDroppedFrames() {
            throw new System.NotImplementedException();
        }
    }
}