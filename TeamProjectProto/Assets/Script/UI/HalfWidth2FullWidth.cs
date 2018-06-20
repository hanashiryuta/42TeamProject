/*
 * 作成日時：180614
 * フォント表示に合うように半角を全角にするクラス
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HalfWidth2FullWidth
{

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
    /// 半角を全角に変える（今数字だけ）
    /// </summary>
    /// <param name="half">半角文字</param>
    /// <returns></returns>
    static char Half2Full(char half)
    {
        char result = ' ';

        switch (half)
        {
            case '0':
                result = '０';
                break;

            case '1':
                result = '１';
                break;

            case '2':
                result = '２';
                break;

            case '3':
                result = '３';
                break;

            case '4':
                result = '４';
                break;

            case '5':
                result = '５';
                break;

            case '6':
                result = '６';
                break;

            case '7':
                result = '７';
                break;

            case '8':
                result = '８';
                break;

            case '9':
                result = '９';
                break;

            case '.':
                result = '．';
                break;

            default:
                result = half;
                break;
        }

        return result;
    }


}
