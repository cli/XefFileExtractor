using System.Linq;
using System.Threading.Tasks;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public class KinectBody : KinectStream {
        public KinectBody(KStudioEventStream stream) : base(stream) { }

        public override void Extract(string outputPath) {
            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            int frameCount = (int) stream.EventCount;
            double[] bodyTiming = new double[frameCount];
            var values = Enumerable.Range(0, frameCount);

            Parallel.ForEach(values, (value, pls, index) => {
                //var currEvent = stream.ReadEvent((uint) index);
                var currEvent = stream.EventHeaders[(int) index];
                bodyTiming[index] = currEvent.RelativeTime.TotalMilliseconds;

                // Update progress
                OnProgressUpdated(new KinectFileProgressChangedEventArgs {
                    Progress = (int) ((float) (index + 1) / frameCount * 100),
                    StatusName = Name
                });
            });

            // Write timings
            string filepath = outputPath + "\\Kinect_Output";
            Utils.ExistOrCreateDirectory(filepath);
            Utils.WriteTimingToFile(filepath + "\\body_timing.txt", bodyTiming);
        }

        public override FrameAnalysis CountDroppedFrames() {
            throw new System.NotImplementedException();
        }
    }
}