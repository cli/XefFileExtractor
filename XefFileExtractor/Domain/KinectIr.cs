using System.Linq;
using System.Threading.Tasks;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public class KinectIr : KinectStream {
        public KinectIr(KStudioEventStream stream) : base(stream) { }

        public override void Extract(string outputPath) {
            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            int frameCount = (int) stream.EventCount;
            double[] irTiming = new double[frameCount];
            var values = Enumerable.Range(0, frameCount);

            Parallel.ForEach(values, (value, pls, index) => {
                //var currEvent = stream.ReadEvent((uint) index);
                var currEvent = stream.EventHeaders[(int) index];
                irTiming[index] = currEvent.RelativeTime.TotalMilliseconds;

                // Update progress
                OnProgressUpdated(new KinectFileProgressChangedEventArgs {
                    Progress = (int) ((float) (index + 1) / frameCount * 100),
                    StatusName = Name
                });
            });

            // Write timings
            string filepath = outputPath + "/Kinect_Output";
            Utils.WriteTimingToFile(filepath + "/IR_timing.txt", irTiming);
        }

        public override FrameAnalysis CountDroppedFrames() {
            throw new System.NotImplementedException();
        }
    }
}