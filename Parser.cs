using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class Parser
{
    public Dictionary<string, string> variables;

    private bool el = false;

    public Parser()
    {
        variables = new Dictionary<string, string>();
    }

    public string P(string s)
    {
        int i = s.IndexOf('(');
        int j = s.IndexOf(')');

        var str = s.Substring(i, (j - i) + 1);
        return str;
    }

    private string OrderN(string s)
    {
        while (s.Contains("x"))
        {
            var str = ParseUtils.FindPrevOperator(s, s.IndexOf('x')) + "x" + ParseUtils.FindNextOperator(s, s.IndexOf('x'));

            var e = ParseUtils.evaluate(str);

            var expr = " " + e.ToString() + " ";

            s = s.Replace(str, expr);

        }

        if (!s.Contains("x"))
        {
            while (s.Contains('/'))
            {
                var str = ParseUtils.FindPrevOperator(s, s.IndexOf('/')) + "/" + ParseUtils.FindNextOperator(s, s.IndexOf('/'));
                var e = ParseUtils.evaluate(str);

                var expr = " " + e.ToString() + " ";

                s = s.Replace(str, expr);
            }
            if (!s.Contains("/"))
            {
                while (s.Contains('+'))
                {
                    var str = ParseUtils.FindPrevOperator(s, s.IndexOf('+')) + "+" + ParseUtils.FindNextOperator(s, s.IndexOf('+'));
                    var e = ParseUtils.evaluate(str);

                    var expr = " " + e.ToString() + " ";

                    s = s.Replace(str, expr);
                }
                if (!s.Contains("+"))
                {
                    while (s.Contains('-'))
                    {
                        var str = ParseUtils.FindPrevOperator(s, s.IndexOf('-')) + "-" + ParseUtils.FindNextOperator(s, s.IndexOf('-'));
                        var e = ParseUtils.evaluate(str);

                        var expr = " " + e.ToString() + " ";

                        s = s.Replace(str, expr);
                    }
                }
            }
        }
        return s;
    }

    private string Order(string s)
    {
        if (s.Contains('('))
        {
            var r = OrderN(P(s).Replace("(", string.Empty).Replace(")", string.Empty));

            s = s.Replace(P(s), r);
        }
        if (!s.Contains('('))
        {
            while (s.Contains("x"))
            {
                var str = ParseUtils.FindPrevOperator(s, s.IndexOf('x')) + "x" + ParseUtils.FindNextOperator(s, s.IndexOf('x'));
                var e = OrderN(str);

                var expr = " " + e.ToString() + " ";

                s = s.Replace(str, expr);
            }

            if (!s.Contains("x"))
            {
                while (s.Contains('/'))
                {
                    var str = ParseUtils.FindPrevOperator(s, s.IndexOf('/')) + "/" + ParseUtils.FindNextOperator(s, s.IndexOf('/'));
                    var e = OrderN(str);

                    var expr = " " + e.ToString() + " ";

                    s = s.Replace(str, expr);
                }
                if (!s.Contains("/"))
                {
                    while (s.Contains('+'))
                    {
                        var str = ParseUtils.FindPrevOperator(s, s.IndexOf('+')) + "+" + ParseUtils.FindNextOperator(s, s.IndexOf('+'));
                        var e = OrderN(str);

                        var expr = " " + e.ToString() + " ";

                        s = s.Replace(str, expr);
                    }
                    if (!s.Contains("+"))
                    {
                        while (s.Contains('-'))
                        {
                            var str = ParseUtils.FindPrevOperator(s, s.IndexOf('-')) + "-" + ParseUtils.FindNextOperator(s, s.IndexOf('-'));
                            var e = OrderN(str);

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
        var name = ParseUtils.getVarName(line);
        var value = ParseUtils.getVarValue(line);

        if (!variables.Keys.Contains(name))
        {
            variables.Add(name, value);
        }
        else
        {
            variables[name] = value;
        }
    }

    private bool HasVar(string s)
    {
        return variables.Keys.Any(t => s.Contains(t));
    }


    public string Parse(string s)
    {
        //TODO: Fix Brackets with variables

        #region checkComment
        if (s.Contains("#") || s == string.Empty)
            return "";
        #endregion

        #region parsemath
        //If Math Is Detected
        if ((s.Contains('+') || s.Contains('-') || s.Contains('/') || s.Contains('x')) && !s.Contains('=') && !HasVar(s))
        {
            var res = Order(s);
            return res;
        }else if (s.Contains("var"))
        {
            StoreVariable(s);
            return "";
        }
        else if (s.Contains('+') || s.Contains('-') || s.Contains('/') || s.Contains('x') || s.Contains('=') && HasVar(s))
        {
            foreach (var item in variables.Keys.ToList())
            {
                //Variable Operation

                if (!s.Contains('=') && HasVar(s))
                {
                    var words = s.Split(' ');

                    foreach (var w in words)
                    {
                        if (HasVar(w) && !ParseUtils.isOp(w))
                        {
                            var n = "";

                            if (w.Contains('(') || w.Contains(')'))
                            {
                                n = w.Replace("(", string.Empty).Replace(")", string.Empty);
                                s = s.Replace(n, variables[n]);
                            }
                            else
                            {
                                n = w.Replace("(", string.Empty).Replace(")", string.Empty);
                                s = s.Replace(n, variables[n]);
                            }
                        }

                    }

                    return Order(s);
                }
                //New assignment
                else if (s.Contains("chv"))
                {
                    StoreVariable(s);
                    return "";
                }
            }
        }
        #endregion

        #region logic

        #region containsIf
        if(s.Contains("if"))
        {
            #region getCondition
                var condition = ParseUtils.GetIfCondition(s);
            #endregion
            
            #region evaluate

            if(HasVar(condition))
            {
                var vars = ParseUtils.FindVar(condition, variables);

                foreach(var v in vars)
                {
                    condition = condition.Replace(v, variables[v]);
                }
            }

            if(ParseUtils.EvaluateIf(condition) == "TRUE")
            {
                var toDo = ParseUtils.GetThen(s);
                return Parse(toDo);
            }
            else if(ParseUtils.EvaluateIf(condition) == "FALSE")
            {
                el = true;
                return "";
            }

            #endregion
        }
        #endregion

        #region containsElse

        if (s.Contains("el") && el == true)
        {
            var toDO = s.Replace("el", string.Empty);
            Console.WriteLine(toDO);
            el = false;
            toDO = "";
            return Parse(toDO);
        }
        #endregion


        #endregion

        #region printOnly
        else
        {
            if (HasVar(s) && !s.Contains("if"))
            {
                var t = s.Replace(" ", string.Empty);
                return variables[t];
            }
            else if (s.IsNumeric())
                return s;
        }
        #endregion

        return "Parse Failed";
    }

}