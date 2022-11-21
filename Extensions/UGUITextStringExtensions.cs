using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minerva.Module
{
    public static class UGUITextStringExtensions
    {
#if UNITY_EDITOR

        [UnityEditor.MenuItem("CONTEXT/Text/Convert TMP UGUI")]
        public static void ConvertToTMPUGUI(UnityEditor.MenuCommand menuCommand)
        {
            Text text = (menuCommand.context as Text);
            Object.Destroy(text);
            var tmp = text.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text.text;
            tmp.color = text.color;
            tmp.fontSize = text.fontSize;
        }

        [UnityEditor.MenuItem("CONTEXT/Text/Convert TMP")]
        public static void ConvertToTMP(UnityEditor.MenuCommand menuCommand)
        {
            Text text = (menuCommand.context as Text);
            Object.Destroy(text);
            var tmp = text.gameObject.AddComponent<TextMeshPro>();
            tmp.text = text.text;
            tmp.color = text.color;
            tmp.fontSize = text.fontSize;
        }
#endif

        public static string UGUIUnderline(this string str)
        {
            return $"<u>{str}</u>";
        }
        public static string UGUIItalic(this string str)
        {
            return $"<i>{str}</i>";
        }

        public static string UGUIBold(this string str)
        {
            return $"<b>{str}</b>";
        }

        public static string UGUIStringSize(this string str, string percentage)
        {
            return $"<size={percentage}>{str}</size>";
        }

        public static string UGUIColor(this string str, string colorCode)
        {
            return $"<color={colorCode}>{str}</color>";
        }

        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            if (text.Length < 2)
            {
                return text.ToUpper();
            }

            string baseString = text[0].ToString().ToUpper() + text.Substring(1, text.Length - 1);
            bool isCapitalized = true;
            for (int i = 0; i < baseString.Length; i++)
            {
                bool isCurrentCapitalized = char.IsUpper(baseString, i);
                if (isCurrentCapitalized && isCapitalized) continue;
                if (isCurrentCapitalized && !isCapitalized)
                {
                    baseString = baseString.Substring(0, i) + " " + baseString.Substring(i);
                }
                isCapitalized = isCurrentCapitalized;
            }
            return baseString;
        }

    }
}