using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class Parser
{
    private Dictionary<string, string> variables;

    public Parser()
    {
        variables = new Dictionary<string, string>();
    }

    public double evaluate(string s)
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

    private string Paran(string s)
    {
        int c = 0;
        var toRet = "";
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '(')
            {
                c = i + 1;
                continue;
            }

            if (s[i] == ')')
            {
                var stmnt = s.Substring(c, i - c);
                var segment = s.Substring(c - 1, i + 1 - (c - 1));

                var e = Order(stmnt);

                s = s.Replace(segment, Convert.ToString(e));

                toRet = s;
                if (!s.Contains("("))
                    break;
            }
        }

        return toRet;
    }

    private string FindNextOperator(string s, int start)
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

    private string FindPrevOperator(string s, int start)
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

    private string getVarName(string l)
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

    private string getVarValue(string line)
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

    private string P(string s)
    {
        int i = s.IndexOf('(');
        int j = s.IndexOf(')');

        var str = s.Substring(i + 1, j - (i + 1));
        return str;
    }

    public string OrderN(string s)
    {
        if (s.Contains("x"))
        {
            var str = FindPrevOperator(s, s.IndexOf('x')) + "x" + FindNextOperator(s, s.IndexOf('x'));
            
            var e = evaluate(str);

            var expr = " " + e.ToString() + " ";

            s = s.Replace(str, expr);
           
        }

        if (!s.Contains("x"))
        {
            if (s.Contains('/'))
            {
                var str = FindPrevOperator(s, s.IndexOf('/')) + "/" + FindNextOperator(s, s.IndexOf('/'));
                var e = evaluate(str);

                var expr = " " + e.ToString() + " ";

                s = s.Replace(str, expr);
            }
            if (!s.Contains("/"))
            {
                if (s.Contains('-'))
                {
                    var str = FindPrevOperator(s, s.IndexOf('-')) + "-" + FindNextOperator(s, s.IndexOf('-'));
                    var e = evaluate(str);

                    var expr = " " + e.ToString() + " ";

                    s = s.Replace(str, expr);
                }
                if (!s.Contains("-"))
                {
                    while (s.Contains('+'))
                    {
                        var str = FindPrevOperator(s, s.IndexOf('+')) + "+" + FindNextOperator(s, s.IndexOf('+'));
                        var e = evaluate(str);

                        var expr = " " + e.ToString() + " ";

                        s = s.Replace(str, expr);
                    }
                }
            }
        }
        return s;
    }

    public string Order(string s)
    {
        
        if (s.Contains('('))
        {
            var r = OrderN(P(s));

            s = s.Replace(s.Substring(s.IndexOf('('), s.IndexOf(')') - s.IndexOf('(')), r).
                Replace("(", string.Empty).Replace(")", string.Empty);
        }
        if (!s.Contains('('))
        {
            if (s.Contains("x"))
            {
                var str = FindPrevOperator(s, s.IndexOf('x')) + "x" + FindNextOperator(s, s.IndexOf('x'));
                var e = evaluate(str);

                var expr = " " + e.ToString() + " ";

                s = s.Replace(str, expr);
            }

            if (!s.Contains("x"))
            {
                if (s.Contains('/'))
                {
                    var str = FindPrevOperator(s, s.IndexOf('/')) + "/" + FindNextOperator(s, s.IndexOf('/'));
                    var e = evaluate(str);

                    var expr = " " + e.ToString() + " ";

                    s = s.Replace(str, expr);
                }
                if (!s.Contains("/"))
                {
                    if (s.Contains('-'))
                    {
                        var str = FindPrevOperator(s, s.IndexOf('-')) + "-" + FindNextOperator(s, s.IndexOf('-'));
                        var e = evaluate(str);

                        var expr = " " + e.ToString() + " ";

                        s = s.Replace(str, expr);
                    }
                    if (!s.Contains("-"))
                    {
                        if (s.Contains('+'))
                        {
                            var str = FindPrevOperator(s, s.IndexOf('+')) + "+" + FindNextOperator(s, s.IndexOf('+'));
                            var e = evaluate(str);

                            var expr = " " + e.ToString() + " ";

                            s = s.Replace(str, expr);
                        }
                    }
                }
            }
        }
        return s;
        
    }


    private void StoreVariable(string line)
    {
        var name = getVarName(line);
        var value = getVarValue(line);

        if (!variables.Keys.Contains(name))
        {
            variables.Add(name, value);
        }
        else
        {
            variables[name] = value;
        }
    }

    private bool isOp(string s)
    {
        if (s == "+" || s == "-" || s == "/" || s == "x")
        {
            return true;
        }

        else { return false; }
    }

    private bool HasVar(string s)
    {
        return variables.Keys.Any(t => s.Contains(t));
    }

    public void Parse(string s)
    {
        //TODO: Fix Brackets with variables
        //If Math Is Detected
        if ((s.Contains('+') || s.Contains('-') || s.Contains('/') || s.Contains('x')) && !s.Contains('=') && !HasVar(s))
        {
            var res = Order(s);
            Console.WriteLine(res);
        }

        //Variable Assignment
        else if (s.Contains("var"))
        {
            StoreVariable(s);
        }


        //operation with variable or new assignment
        else if (s.Contains('+') || s.Contains('-') || s.Contains('/') || s.Contains('x') || s.Contains('=') && HasVar(s))
        {
            foreach (var item in variables.Keys.ToList())
            {
                //Variable Operation

                if (!s.Contains('=') && HasVar(s))
                {
                    var words = s.Split(' ');
                    var vars = new List<string>();

                    foreach (var w in words)
                    {
                        if (HasVar(w) && !isOp(w))
                        {
                            var n = "";

                            if (w.Contains('('))
                            {
                                n = w.Replace("(", string.Empty);
                                s = s.Replace(n, variables[n]);
                            }
                            else
                            {
                                s = s.Replace(w, variables[w]);
                            }
                        }

                    }

                    Console.WriteLine(Order(s));
                    break;
                }
                //New assignment
                else if (s.Contains("chv"))
                {
                    StoreVariable(s);
                }
            }
        }
    }

    public Dictionary<string, string> getVars()
    {
        return variables;
    }

    public void setVars(Dictionary<string, string> n)
    {
        variables = n;
    }


}