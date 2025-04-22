using System.Collections.Generic;

namespace VoicePress
{
    public static class Commands
    {
        public static Dictionary<string, Key> Keys { get; set; } = new Dictionary<string, Key>() {
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
    }
}
