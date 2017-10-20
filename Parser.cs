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
        var opn = new char[]{'+', '-', '/', 'x'};

        var nums = s.Split(opn);

        if (s.Contains("-"))
            return (Convert.ToDouble(nums[0]) - Convert.ToDouble(nums[1]));

        else if(s.Contains("+"))
            return (Convert.ToDouble(nums[0]) + Convert.ToDouble(nums[1]));

        else if(s.Contains("/"))
            return (Convert.ToDouble(nums[0]) / Convert.ToDouble(nums[1]));

        else if(s.Contains("x"))
            return (Convert.ToDouble(nums[0]) * Convert.ToDouble(nums[1]));

        else   
            return Convert.ToDouble(s);
            
    } 

    private string Paran(string s)
    {
        int c = 0;
        var toRet = "";
            for(int i = 0; i < s.Length; i++)
            {
                if(s[i] == '(')
                {
                    c = i + 1;
                    continue;
                }

                if(s[i] == ')')
                {
                    var stmnt = s.Substring(c, i - c);
                    var segment = s.Substring(c - 1, i + 1 - (c - 1));

                    var e = evaluate(stmnt);

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


        for(int i = start + 1; i < s.Length; i++)
        {
            if(s[i] == '+' || s[i] == '-' || s[i] == 'x' || s[i] == '/')
            {
                o = i;
                toRet = s.Substring(start + 1, o - (start + 1));
                break;
            }
            if(i == s.Length - 1)
            {
                return(s.Substring(start + 1, s.Length - (start + 1)));
            }
        }

        return toRet;
    }

    private string FindPrevOperator(string s, int start)
    {
        
        int o = 0;

        var toRet = "";
        for(int i = start - 1; i > 0; i--)
        {
            if(s[i] == '+' || s[i] == '-' || s[i] == 'x' || s[i] == '/')
            {
                o = i;
                toRet = s.Substring(o + 1, start - (o + 1));
                break;
            }
            if(i == 1){
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
            if(l[i] == 'v' && l[i+1] == 'a' && l[i+2] =='r')
            {
                s = i + 2;
                continue;
            }

            if(l[i] == '=')
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
            if(line[i] == '=')
            {
                s = i;
                break;
            }
        }

        return line.Substring(s + 1, (line.Length - 1) - s);
    }

    private string Order(string s)
    {

        var toRet = "";

        for (int i = 0; i < s.Length; i++)
        {
            if(s[i] == 'x' )
            {
                var str = FindPrevOperator(s, i) + "x" + FindNextOperator(s, i);
                var e = evaluate(str);

                var expr = " " + e.ToString() + " ";
                
                s = s.Replace(str, expr);
            }

            if(!s.Contains("x"))
            {
                if(s[i] == '/')
                {
                    var str = FindPrevOperator(s, i) + "/" + FindNextOperator(s, i);
                    var e = evaluate(str);

                    var expr = " " + e.ToString() + " ";

                    s = s.Replace(str, expr);
                }
                if(!s.Contains("/"))
                {
                    if(s[i] == '-' && s[i+1] == ' ')
                    {
                        var str = FindPrevOperator(s, i) + "-" + FindNextOperator(s, i);
                        var e = evaluate(str);

                        var expr = " " + e.ToString() + " ";

                        s = s.Replace(str, expr);
                    }
                    if(!s.Contains("-"))
                    {
                        if(s.Contains('+'))
                        {
                            var str = FindPrevOperator(s, s.IndexOf('+')) + "+" + FindNextOperator(s, s.IndexOf('+'));
                            var e = evaluate(str);

                            var expr = " " + e.ToString() + " ";
                        
                            s = s.Replace(str, expr);

                            toRet = s;
                        }
                        else{
                            continue;
                        }
                    }
                }
            }
        }
        return toRet;
    }

    private void StoreVariable(string line)
    {
        var name = getVarName(line);
        var value = getVarValue(line);
        
        if(!variables.Keys.Contains(name))
        {
            variables.Add(name, value);
        }
        else
        {
            variables[name] = value;
        }
    }

    public void Parse(string s)
    {

        //If Math Is Detected
        if ((s.Contains('+') || s.Contains('-') || s.Contains('/') || s.Contains('*')) && s.IsNumeric())
        {
            var p = Paran(s);
            var res = Order(p);
            Console.WriteLine(res);
        }
        else if (s.Contains("var"))
        {
            StoreVariable(s);
        }
        else if ( (s.Contains('+') || s.Contains('-') || s.Contains('/') || s.Contains('*') || s.Contains('=')) && !s.IsNumeric())
        {
            foreach (var item in variables.Keys.ToList())
            {
                if (s.Contains(item) && !s.Contains('='))
                {
                    string val = variables[item];

                    s = s.Replace(item, val);

                    var res = ""; 
                    if(s.Contains('('))
                    {
                         var p = Paran(s);
                        res = Order(p); 
                    }
                    else
                    {
                        res = Order(s);
                    }

                    Console.WriteLine(res);
                }
                else if(s.Contains(item) && s.Contains('='))
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