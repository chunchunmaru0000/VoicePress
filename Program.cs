using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Vosk;

namespace VoicePress
{
    class Program
    {
        private static VoskRecognizer VoskRecognizer { get; set; }
        private static Model Model { get; set; }
        private static WaveInEvent WIE { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            SetupRecognition();

            WIE.StartRecording();
            Console.ReadLine();
            WIE.StopRecording();

            VoskRecognizer.Dispose();
            Model.Dispose();
        }

        private static void SetupRecognition()
        {
            int deviceNum = SelectDevice();

            Vosk.Vosk.SetLogLevel(0);
            Model = new Model("C:\\myos\\vp\\ru");
            VoskRecognizer = new VoskRecognizer(Model, 16000.0f);

            WIE = new WaveInEvent()
            {
                DeviceNumber = deviceNum,
                WaveFormat = new WaveFormat(16000, 1)
            };
            WIE.DataAvailable += DataRecognized;
        }

        private static int SelectDevice()
        {
            Console.WriteLine("[*] Select device:");
            Console.WriteLine(string.Join("\n",
                Enumerable
                .Range(0, WaveInEvent.DeviceCount)
                .Select(i => $"[{i}] {WaveInEvent.GetCapabilities(i).ProductName}"
            )));
            int deviceNum;
            while (!int.TryParse(Console.ReadLine(), out deviceNum)) ;
            Console.WriteLine($"[*] Selected [{WaveInEvent.GetCapabilities(deviceNum).ProductName}]");
            return deviceNum;
        }

        private static void DataRecognized(object s, WaveInEventArgs e) 
        {
            if (!VoskRecognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                return;
                /*
                string result = VoskRecognizer.PartialResult();
                Console.WriteLine($"[r] Recognized partial result: \n{result}");

                string text =
                    JsonDocument.Parse(result)
                    .RootElement.GetProperty("partial")
                    .GetString();
                if (!string.IsNullOrWhiteSpace(text))
                    Console.WriteLine($"[P] Partial text\n{text}");
                    */

            string text =
                JsonDocument
                .Parse(VoskRecognizer.Result())
                .RootElement
                .GetProperty("text")
                .GetString();

            Console.WriteLine($"[T] Text\n{text}");
            TryTextOnCommands(text);
        }

        [DllImport("User32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, ulong dwExtraInfo);

        private static void PressKey(Key key) => PressKey((byte)key);

        private static void PressKey(byte key)
        {
            keybd_event(key, 0, (uint)KeyState.DOWN, 0);
            keybd_event(key, 0, (uint)KeyState.UP, 0);
        }

        private static Dictionary<string, Key> KeyCommands { get; set; } = new Dictionary<string, Key>() {
            { "ноль", Key.ZERO },
            { "один", Key.ONE },
            { "два", Key.TWO },
            { "три", Key.THREE },
            { "четыре", Key.FOUR },
            { "пять", Key.FIFE },
            { "шесь", Key.SIX },
            { "семь", Key.SEVEN },
            { "восемь", Key.EIGHT },
            { "девять", Key.NINE },

            { "центр", Key.SPACE },
            { "энтер", Key.SPACE },
            { "пробел", Key.SPACE },

            { "текст", Key.L },
            { "история", Key.L },
            { "лог", Key.L },

            { "выход", Key.ESC },
            { "выйти", Key.ESC },
        };

        private static void TryTextOnCommands(string text)
        {
            string[] words = text.ToLower().Split(' ');
            foreach(string word in words) {
                if (KeyCommands.ContainsKey(word))
                    PressKey(KeyCommands[word]);
            }
        }
    }
}
