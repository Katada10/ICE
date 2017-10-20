using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Tokenizer : IToken
{
    public string ReadFile(string name)
    {
        return File.ReadAllText(name);
    }


    public string[] SplitString(string src)
    {
        string pattern = @"([:=\n.*])"; 
        var substrings =  Regex.Split(src, pattern).Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
        return substrings;
    }


    public void WriteFile(string name, string src, string type)
    {
        var text = src + ": " + type + '\n';

        File.AppendAllText(name, text);
    }

    public void Tokenize(string src)
    {
        var words = SplitString(src);
        string type = "";
        
        foreach (var item in words)
        {
            if(item == "+" | item == "-" | item == "*" | item == "/")
            {
                type = "opn";
            }    
            else if(item == "=")
            {
                type = "assn";
            }
            else if(item.IsNumeric())
            {
                type = "num";
            }
            else if(item.Contains('"'))
            {
                type = "str";
            }
            else
            {
                type = "var";
            }

            WriteFile("tokens.txt", item, type);
        }
        
    }
}