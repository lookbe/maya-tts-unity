using LlamaCpp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MayaTTS
{
    public class MayaTTS : MonoBehaviour
    {
		[Header("Model")]
		public string mayaModelPath = string.Empty;
        public string snacModelPath = string.Empty;

		[Header("Speaker")]
		[TextArea(3, 3)]
		public string speakerDescription = string.Empty;

		protected MayaModel maya;
        protected SnacDecoder decoder;
        protected AudioSource audioSource;

        private Queue<float> audioQueue = new Queue<float>();

        public delegate void StatusChangedDelegate(ModelStatus status);
        public event StatusChangedDelegate OnStatusChanged;

        private ModelStatus _status = ModelStatus.Init;

        // Public getter, no public setter
        public ModelStatus status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStatusChanged?.Invoke(_status);
                }
            }
        }

        public void InitModel()
        {
            if (string.IsNullOrEmpty(mayaModelPath))
            {
                return;
            }

            if (string.IsNullOrEmpty(snacModelPath))
            {
                return;
            }

            if (_status != ModelStatus.Init)
            {
                Debug.LogError("invalid status");
                return;
            }

            status = ModelStatus.Loading;
            StartCoroutine(RunInitModel());
        }

        IEnumerator RunInitModel()
        {
            Debug.Log($"Load Maya tts model");

            maya.modelPath = mayaModelPath;
            maya.InitModel();

            decoder.modelPath = snacModelPath;
            decoder.InitModel();

            yield return new WaitWhile(() => maya.status != ModelStatus.Ready);
            yield return new WaitWhile(() => decoder.status != ModelStatus.Ready);

            Debug.Log("Load model done");

            status = ModelStatus.Ready;
        }

        public void Prompt(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return;
            }

            if (status != ModelStatus.Ready)
            {
                Debug.LogError("invalid status");
                return;
            }

            status = ModelStatus.Generate;
            maya.Prompt(prompt, speakerDescription);
            StartCoroutine(WaitForGenerationAndPlaybackDone());
        }

        IEnumerator WaitForGenerationAndPlaybackDone()
        {
            yield return new WaitUntil(() => maya.status == ModelStatus.Ready);
            yield return new WaitUntil(() => decoder.status == ModelStatus.Ready);

            // Wait for all audio samples to be played
            yield return new WaitUntil(() => audioQueue.Count == 0);
            status = ModelStatus.Ready;
        }

        public void Stop()
        {
            if (status != ModelStatus.Generate)
            {
                Debug.Log("already stopped");
                return;
            }

            maya.Stop();
        }


        // harcoded value from snac decoder
        private const int SampleRate = 24000;
        private const int Channels = 1;

        private void Awake()
        {
            maya = GetComponentInChildren<MayaModel>();
            decoder = GetComponentInChildren<SnacDecoder>();
            audioSource = GetComponent<AudioSource>();

            audioSource.clip = AudioClip.Create("StreamingClip", SampleRate * 60, Channels, SampleRate, true, OnAudioRead);
            audioSource.loop = true;
            audioSource.Play();
        }

        private void OnEnable()
        {
            maya.OnStatusChanged += OnModelStatusChanged;

            decoder.OnResponseGenerated += OnResponseGenerated;
            decoder.OnStatusChanged += OnModelStatusChanged;
        }

        private void OnDisable()
        {
            maya.OnStatusChanged -= OnModelStatusChanged;

            decoder.OnResponseGenerated -= OnResponseGenerated;
            decoder.OnStatusChanged -= OnModelStatusChanged;
        }

        void OnModelStatusChanged(ModelStatus status)
        {
            if (status == ModelStatus.Error)
            {
                StopAllCoroutines();
                this.status = ModelStatus.Error;
            }
        }

        void OnResponseGenerated(float[] audioChunk)
        {
            foreach (var s in audioChunk)
                audioQueue.Enqueue(s);
        }

        private void OnAudioRead(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (audioQueue.Count > 0)
                    data[i] = audioQueue.Dequeue();
                else
                    data[i] = 0f;
            }
        }
    }
}
