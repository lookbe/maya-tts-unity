using LlamaCpp;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MayaTTS
{
    public class MayaModel : LlamaCpp.Completion
    {
        // special tokens for Maya tts
        protected const int CODE_START_TOKEN_ID = 128257;
        protected const int CODE_END_TOKEN_ID = 128258;
        protected const int CODE_TOKEN_OFFSET = 128266;
        protected const int SNAC_MIN_ID = 128266;
        protected const int SNAC_MAX_ID = 156937;
        protected const int SNAC_TOKENS_PER_FRAME = 7;

        protected const int SOH_ID = 128259;
        protected const int EOH_ID = 128260;
        protected const int SOA_ID = 128261;
        protected const int BOS_ID = 128000;
        protected const int EOT_ID = 128009;

        protected class MayaPayload : CompletionPayload
        {
            public string Prompt;
        }

        public void Prompt(string prompt, string speakerDescription)
        {
            if (string.IsNullOrEmpty(prompt) || string.IsNullOrEmpty(speakerDescription))
            {
                return;
            }

            if (_llamaContext == IntPtr.Zero)
            {
                Debug.LogError("invalid context");
                return;
            }

            if (_llamaModel == IntPtr.Zero)
            {
                Debug.LogError("model not loaded");
                return;
            }

            if (status != LlamaCpp.ModelStatus.Ready)
            {
                Debug.LogError("invalid status");
                return;
            }

            status = LlamaCpp.ModelStatus.Generate;
            var payload = new MayaPayload()
            {
                Prompt = $"<description=\"{speakerDescription}\"> {prompt}",

                Temp = this.temperature,
                TopK = this.topK,
                TopP = this.topP,
                MinP = this.minP,
                RepeatPenalty = this.repeatPenalty,

            };

            RunBackground(payload, RunMayaPrompt);
        }

        void RunMayaPrompt(MayaPayload inputPayload, CancellationToken cts)
        {
            int BOS_TOKEN = LlamaCpp.Native.llama_vocab_bos(_llamaVocab);
            int[] prompt_token = Tokenize(inputPayload.Prompt);

            List<int> token_list = new List<int>();
            token_list.Add(SOH_ID);
            token_list.Add(BOS_TOKEN);
            token_list.AddRange(prompt_token);
            token_list.Add(EOT_ID);
            token_list.Add(EOH_ID);
            token_list.Add(SOA_ID);
            token_list.Add(CODE_START_TOKEN_ID);

            var payload = new GenerationPayload()
            {
                Tokens = token_list.ToArray(),
                Temp = inputPayload.Temp,
                TopK = inputPayload.TopK,
                TopP = inputPayload.TopP,
                MinP = inputPayload.MinP,
                RepeatPenalty = inputPayload.RepeatPenalty,

            };

            RunGenerate(payload, cts);
        }

        protected override bool EndGeneration(int token, int generated_token_count)
        {
            return Native.llama_vocab_is_eog(_llamaVocab, token) || token == CODE_END_TOKEN_ID;
        }
    }
}
