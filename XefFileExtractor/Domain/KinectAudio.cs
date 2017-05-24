using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Kinect.Tools;

namespace XefFileExtractor.Domain {
    public class KinectAudio : KinectStream {
        private const double FrameTime = 16;

        public KinectAudio(KStudioEventStream stream) : base(stream) { }

        public override void Extract(string outputPath) {
            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;
            IReadOnlyList<KStudioEventHeader> headers = stream.EventHeaders;

            int frameCount = (int) stream.EventCount;
            double[] audioTiming = new double[frameCount];
            const int eventFrameSize = 14432; // Size for a single audio event frame
            const int frameSize = 1024; // The size of the actual audio data in an event frame
            int dataSize = 0;
            int bytePos = 0;

            // DEBUG
            double totalMissedTime = 0;
            int missedFrames = 0;
            double cumulativeTime = 0;
            double lDiff = 0;
            KStudioEventHeader lHeader = null;
            // END DEBUG

            // Hacky counting of total data size
            for (int i = 0; i < frameCount; i++) {
                KStudioEventHeader header = headers[i];
                int j = eventFrameSize;
                double missedTime = 0;
                double diff = header.RelativeTime.TotalMilliseconds;

                /* DEBUG */
                if (lHeader != null) {
                    diff = header.RelativeTime.TotalMilliseconds - lHeader.RelativeTime.TotalMilliseconds;
                    if (diff > lDiff) {
                        lDiff = diff;
                    }
                }

                lHeader = header;
                /* END DEBUG */

                do {
                    dataSize += frameSize;
                    j += eventFrameSize - 16;
                    missedTime += 16;
                } while (j <= (int) header.EventDataSize);

                missedTime = diff - missedTime;
                cumulativeTime += missedTime;
                totalMissedTime += missedTime;
                while (cumulativeTime > 16) {
                    cumulativeTime -= 16;
                    missedFrames++;
                }
            }

            // Calculate additional bytes needed
            //int numBytes = (int) totalMissedTime / 16 * 1024;
            int numBytes = missedFrames * frameSize;
            if (numBytes > 0) {
                dataSize += numBytes;
            }

            // DEBUG
            missedFrames = 0;
            cumulativeTime = 0;
            KStudioEvent lEvent = null;
            // END DEBUG

            byte[] rawAudio = new byte[dataSize];

            for (uint i = 0; i < frameCount; i++) {
                int j = eventFrameSize;
                var currEvent = stream.ReadEvent(i);
                byte[] rawBuffer = new byte[currEvent.EventDataSize];
                int offset = 96;
                double diff = currEvent.RelativeTime.TotalMilliseconds;
                double missedTime = 0;

                if (lEvent != null) {
                    diff = currEvent.RelativeTime.TotalMilliseconds - lEvent.RelativeTime.TotalMilliseconds;
                    if (diff > lDiff) {
                        lDiff = diff;
                    }
                }

                lEvent = currEvent;

                audioTiming[i] = currEvent.RelativeTime.TotalMilliseconds;
                currEvent.CopyEventDataToArray(rawBuffer, 0);

                do {
                    Array.Copy(rawBuffer, offset, rawAudio, bytePos, frameSize);
                    bytePos += frameSize;
                    j += eventFrameSize - 16;
                    offset += eventFrameSize - 16;
                    missedTime += 16;
                } while (j <= (int) currEvent.EventDataSize);

                offset = 96;

                // If there is any missed time, add it here.
                missedTime = diff - missedTime;
                cumulativeTime += missedTime;
                while (cumulativeTime > 16) {
                    cumulativeTime -= 16;
                    missedFrames++;
                    bytePos += frameSize;
                }

                // Update progress
                OnProgressUpdated(new KinectFileProgressChangedEventArgs {
                    Progress = (int) ((float) (i + 1) / frameCount * 100),
                    StatusName = Name
                });
            }

            string filepath = outputPath + "/Kinect_Output";
            Directory.CreateDirectory(filepath);
            File.WriteAllBytes(filepath + "/raw_audio.wav", rawAudio);

            // Write timings
            Utils.WriteTimingToFile(filepath + "/audio_timing.txt", audioTiming);
        }

        /// <summary>
        ///  Calculates dropped frame statistics based on the difference in 
        ///  timestamps between two consecutive headers.
        /// </summary>
        /// <param name="stream">
        ///  The KStudioSeekableEventStream object of the audio stream
        /// </param>
        /// <param name="frameTime">
        ///  The number of milliseconds per audio audio frame. This is not
        ///  necessarily the time per event header as multiple audio samples can
        ///  be stored in a single event.
        /// </param>
        /// <returns>
        ///  FrameAnalysis object with collected frame statistics.
        /// </returns>
        public override FrameAnalysis CountDroppedFrames() {
            double prevFrameTime = 0;
            double cumulativeTime = 0;
            int eventFrameSize = 14432;
            FrameAnalysis result = new FrameAnalysis();

            KStudioSeekableEventStream stream = (KStudioSeekableEventStream) _stream;

            foreach (KStudioEventHeader header in stream.EventHeaders) {
                double currentFrameTime = header.RelativeTime.TotalMilliseconds;
                double deltaTime = currentFrameTime - prevFrameTime;

                result.FrameCount++;
                double missedTime = 0;

                int j = 0;

                do {
                    j += eventFrameSize - 16;
                    missedTime += FrameTime;
                } while (j <= (int) header.EventDataSize);

                missedTime = deltaTime - missedTime;
                cumulativeTime += missedTime;
                while (cumulativeTime > FrameTime) {
                    cumulativeTime -= FrameTime;
                    result.DroppedFrames++;
                    result.DroppedIndices.Add(result.FrameCount++);
                }

                prevFrameTime = currentFrameTime;
            }

            return result;
        }
    }
}