using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ParseUtils
{
    public static bool IsNumeric(this string input)
    {
        int number;
        return int.TryParse(input, out number);
    }

    public static bool isOp(string s)
    {
        if (s == "+" || s == "-" || s == "/" || s == "x")
        {
            return true;
        }

        else { return false; }
    }
   
    public static double evaluate(string s)
    {
        var opn = new char[] { '+', '-', '/', 'x' };

        var nums = s.Split(opn);

        if (s.Contains("-"))
        {
            return (Convert.ToDouble(nums[0]) - Convert.ToDouble(nums[1]));
        }

        else if (s.Contains("+"))
        {
            return (Convert.ToDouble(nums[0]) + Convert.ToDouble(nums[1]));
        }

        else if (s.Contains("/"))
        {
            return (Convert.ToDouble(nums[0]) / Convert.ToDouble(nums[1]));
        }

        else if (s.Contains("x"))
        {
            return (Convert.ToDouble(nums[0]) * Convert.ToDouble(nums[1]));
        }
        else
            return Convert.ToDouble(s);

    }

    public static string FindNextOperator(string s, int start)
    {
        int o = 0;

        string toRet = null;


        for (int i = start + 1; i < s.Length; i++)
        {
            if (s[i] == '+' || s[i] == '-' || s[i] == 'x' || s[i] == '/')
            {
                o = i;
                toRet = s.Substring(start + 1, o - (start + 1));
                break;
            }
            if (i == s.Length - 1)
            {
                return (s.Substring(start + 1, s.Length - (start + 1)));
            }
        }

        return toRet;
    }

    public static string FindPrevOperator(string s, int start)
    {

        int o = 0;

        var toRet = "";
        for (int i = start - 1; i > 0; i--)
        {
            if (s[i] == '+' || s[i] == '-' || s[i] == 'x' || s[i] == '/')
            {
                o = i;
                toRet = s.Substring(o + 1, start - (o + 1));
                break;
            }
            if (i == 1)
            {
                toRet = s.Substring(0, start);
            }
        }

        return toRet;
    }

    public static string getVarName(string l)
    {
        int s = 0, e = 0;
        for (int i = 0; i < l.Length; i++)
        {
            if ((l[i] == 'v' && l[i + 1] == 'a' && l[i + 2] == 'r') || (l[i] == 'c' && l[i + 1] == 'h' && l[i + 2] == 'v'))
            {
                s = i + 2;
                continue;
            }

            if (l[i] == '=')
            {
                e = i;
                break;
            }
        }

        var sub = l.Substring(s + 1, (e - 1) - s);
        sub = sub.Replace(" ", string.Empty);

        return sub;
    }

    public static string getVarValue(string line)
    {
        int s = 0;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '=')
            {
                s = i;
                break;
            }
        }

        return line.Substring(s + 1, (line.Length - 1) - s);
    }

    public static List<string> FindVar(string s, Dictionary<string, string> variables)
    {
        var words = s.Split(' ');
        var str = new List<string>();

        foreach(var item in variables.Keys)
        {
            foreach(var word in words)
            {
                if (word == item)
                {
                    if (!str.Contains(word))
                        str.Add(word);
                    else
                        continue;
                }
            }
        }
        return str;
    }

    public static string GetIfCondition(string s)
    {
        bool foundFirst = false;
        int x = 0, y = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if(s[i] == ';' && !foundFirst)
            {
                x = i;
                foundFirst = true;
                continue;
            }
            else if(s[i] == ';' && foundFirst)
            {
                foundFirst = false;
                y = i;
                break;
            }
        }
        
        return s.Substring(x + 1, (y -x) - 1);
    }

    public static string EvaluateIf(string c)
    {
        if(c.Contains('='))
        {
            var words = c.Split('=');

            //TODO: Remove Whitespaces
            var replaced = words.Select(x => x.Replace(" ", string.Empty)).ToList();

            if (replaced[0] == replaced[1])
            {
                return "TRUE";
            }
            else 
            {
                return "FALSE";
            }
        }
        else if (c.Contains('<'))
        {
            var words = c.Split('<');

            var replaced = words.Select(x => x.Replace(" ", string.Empty)).ToList();

            if (Convert.ToDouble(replaced[0]) < Convert.ToDouble(replaced[1]))
            {
                return "TRUE";
            }
            else
            {
                return "FALSE";
            }
        }
        else if (c.Contains('>'))
        {
            var words = c.Split('>');

            var replaced = words.Select(x => x.Replace(" ", string.Empty)).ToList();

            if (Convert.ToDouble(replaced[0]) > Convert.ToDouble(replaced[1]))
            {
                return "TRUE";
            }
            else
            {
                return "FALSE";
            }
        }

        return "NULL";
    }
}

