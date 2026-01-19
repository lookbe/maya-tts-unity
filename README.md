# Maya1 TTS for Unity

A Unity 6 integration for **Maya1**. This package provides a high-performance, local neural text-to-speech solution leveraging ONNX Runtime and Llama.cpp.

---

## 🎭 Human-Like Speech & Low Latency

Maya1 is a state-of-the-art 3B parameter speech model built for **expressive voice generation**, designed to capture real human emotion and precise voice design in real-time.

* **Precise Voice Design:** Create any voice you can imagine through natural language descriptions—from a "20s British girl" to an "American guy" or even a "full-blown demon."
* **Real Human Emotion:** Make your NPCs feel alive with built-in support for 20+ emotion tags including `<laugh>`, `<cry>`, `<whisper>`, `<rage>`, `<sigh>`, and `<gasp>`.
* **Sub-100ms Latency:** Optimized for instant streaming, Maya1 sounds alive and responsive, making it ideal for interactive agents and live conversational AI.
* **Consumer GPU Ready:** Engineered to run locally on a single GPU. The implementation is efficient enough for consumer-grade cards with as little as **6GB VRAM**.
* **Local & Private:** Everything runs on your machine. No API keys, no subscription fees, and no data leaves the user's device.

---

## ⚠️ Hardware & Platform Support
* **API Support:** Currently supports **Vulkan** only.
* **Platform:** **Windows** (Vulkan backend).

---

## Installation

Follow these steps exactly to ensure all native dependencies are resolved.

### Configure manifest.json
Open your project's `Packages/manifest.json` and update it to include the scoped registry and the Git dependencies.

```json
{
  "scopedRegistries": [
    {
      "name": "npm",
      "url": "https://registry.npmjs.com",
      "scopes": [
        "com.github.asus4"
      ]
    }
  ],
  "dependencies": {
    "com.github.asus4.onnxruntime": "0.4.2",
    "com.github.asus4.onnxruntime.unity": "0.4.2",
    "ai.lookbe.llamacpp": "https://github.com/lookbe/llama-cpp-unity.git",
    "ai.lookbe.mayatts": "https://github.com/lookbe/maya-tts-unity.git",

    ... other dependencies
  }
}
```

---

## Requirements: Models

You must download the following two models separately:

1. **Maya1 Model (GGUF format):** Recommended versions can be found at [mradermacher/maya1-GGUF](https://huggingface.co/mradermacher/maya1-GGUF). (Base model info: [maya-research/maya1](https://huggingface.co/maya-research/maya1))
2. **SNAC Decoder (ONNX format):** Use the exact `decoder_model.onnx` file from [snac_24khz-ONNX](https://huggingface.co/onnx-community/snac_24khz-ONNX/tree/main/onnx).

---

## ▶️ Demo Video

Watch the demo here:  
https://www.youtube.com/watch?v=iOdATgxXe0o

---

## Testing

1. **Import Samples:** Go to the Package Manager, select **Maya1 TTS Unity**, and import the **Basic Maya** sample.
2. **Configure Paths:**
    * Select the `MayaTTS` object in the Hierarchy.
    * In the Inspector, locate the **Maya Model Path** and **SNAC Model Path** fields.
    * **Important:** Paste the **absolute path** (e.g., `C:\Models\maya1.gguf`) for both files.
3. **Run:** Press Play. The system will initialize the Vulkan backend on your GPU.

> **Note:** You can extend the component script to use `Application.streamingAssetsPath` for builds, but absolute paths are required for the initial setup.

---
