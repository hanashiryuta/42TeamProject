/*
 * 作成日時：180614
 * フォント表示に合うように半角を全角にするクラス
 * 作成者：何承恩
 */
using System;

public static class HalfWidth2FullWidth
{
    //半角
    static char[] halfWidth = new char[]
    {
        '0', '1', '2', '3', '4',
        '5', '6', '7', '8', '9',
        'A', 'B', 'C', 'D', 'E',
        'F', 'G', 'H', 'I', 'J',
        'K', 'L', 'M', 'N', 'O',
        'P', 'Q', 'R', 'S', 'T',
        'U', 'V', 'W', 'X', 'Y',
        'Z', 'a', 'b', 'c', 'd',
        'e', 'f', 'g', 'h', 'i',
        'j', 'k', 'l', 'm', 'n',
        'o', 'p', 'q', 'r', 's',
        't', 'u', 'v', 'w', 'x',
        'y', 'z',
        '-', ' ', ':', '.', ',', '/', '%', '#', '!', '@',
        '&', '(', ')', '<', '>', '"', '\'', '?', '[', ']',
        '{', '}', '\\', '|', '+', '=', '_', '^', '$', '~', '`'
    };

    //全角
    static char[] fullWidth = new char[]
    {
        '０', '１', '２', '３', '４',
        '５', '６', '７', '８', '９',
        'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ',
        'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ',
        'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ',
        'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ',
        'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ',
        'Ｚ', 'ａ', 'ｂ', 'ｃ', 'ｄ',
        'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ',
        'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ',
        'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ',
        'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ',
        'ｙ', 'ｚ',
        '－', '　', '：', '．', '，', '／', '％', '＃', '！', '＠',
        '＆', '（', '）', '＜', '＞', '＂', '＇', '？', '［', '］',
        '｛', '｝', '＼', '｜', '＋', '＝', '＿', '＾', '＄', '～', '｀'
    };

    /// <summary>
    /// 半角stringを全角にする
    /// </summary>
    /// <param name="halfWidth"></param>
    /// <returns></returns>
    public static string Set2FullWidth(string halfWidth)
    {
        char[] half = new char[halfWidth.Length];
        string result = "";
        char tmp = ' ';

        //もらった数列を一個一個取出し
        for(int i = 0; i < halfWidth.Length; i++)
        {
            //全角に変える
            tmp = Half2Full(char.Parse(halfWidth.Substring(i, 1)));
            result += tmp;
        }
        return result;
    }

    /// <summary>
    /// 半角float/intを全角にする
    /// </summary>
    /// <param name="halfWidth"></param>
    /// <returns></returns>
    public static string Set2FullWidth(float halfWidth)
    {
        char[] half = new char[halfWidth.ToString().Length];
        string result = "";
        char tmp = ' ';

        //もらった数列を一個一個取出し
        for (int i = 0; i < halfWidth.ToString().Length; i++)
        {
            //全角に変える
            tmp = Half2Full(char.Parse(halfWidth.ToString().Substring(i, 1)));
            result += tmp;
        }
        return result;
    }


    /// <summary>
    /// 半角を全角に変える
    /// </summary>
    /// <param name="half">半角文字</param>
    /// <returns></returns>
    static char Half2Full(char half)
    {
        char result = ' ';

        int arrayIndex = 0;
        foreach(var h in halfWidth)
        {
            if(half == h)
            {
                //同じ位置にいる対象を出す
                result = fullWidth[arrayIndex];
            }
            arrayIndex++;
        }

        //対応できるものがなかったらそのまま返す
        if(result == ' ')
        {
            result = half;
        }

        return result;
    }

}
