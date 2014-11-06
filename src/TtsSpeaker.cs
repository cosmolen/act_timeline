namespace ACTTimeline
{
    using System;
    using System.Speech.Synthesis;
    using System.Windows.Forms;

    using ACT.TTSYukkuri;

    public class TtsSpeaker : IDisposable
    {
        private SynthesizerWrapper synthesizer;
        public SynthesizerWrapper Synthesizer
        {
            get
            {
                if (this.synthesizer == null)
                {
                    PopulateSynthesizer();
                }

                return this.synthesizer;
            }
        }

        public string Name { get; private set; }
        public string VoiceName { get; private set; }
        public int Rate { get; private set; }
        public int Volume { get; private set; }

        public TtsSpeaker(string name, string voiceName, int rate, int volume)
        {
            this.Name = name;
            this.VoiceName = voiceName;
            this.Rate = rate;
            this.Volume = volume;
        }

        private void PopulateSynthesizer()
        {
            var synthesizerWrapper = new SynthesizerWrapper();

            if (this.Name.ToUpper() == "TTSYUKKURI")
            {
                // スピークdelegateにゆっくりプラグインを割り当てる
                synthesizerWrapper.SpeakAsyncByYukkuriDelegate = SpeechController.Default.Speak;
            }
            else
            {
                // 標準のTTSを割り当てる
                synthesizerWrapper.Synthesizer = new SpeechSynthesizer();

                if (!string.IsNullOrEmpty(this.VoiceName))
                {
                    try
                    {
                        synthesizerWrapper.Synthesizer.SelectVoice(this.VoiceName);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(string.Format(
                            "The selected TTS engine '{1}' at the speaker named '{0}' is not installed on your system.",
                            this.Name,
                            this.VoiceName));
                    }
                }

                synthesizerWrapper.Synthesizer.Rate = this.Rate;
                synthesizerWrapper.Synthesizer.Volume = this.Volume;

                synthesizerWrapper.SpeakAsyncDelegate = synthesizerWrapper.Synthesizer.SpeakAsync;
            }

            this.synthesizer = synthesizerWrapper;
        }

        public void Dispose()
        {
            if (this.synthesizer != null)
            {
                this.synthesizer.Dispose();
                this.synthesizer = null;
            }
        }
    }
}
