using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB 50 30 10 2
//cabracadabrarrarrad 7 6 10 5
//2000302013020130313031303130313333333 15 13 4 4

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("===== LZ77 LAB 10 =====");
            Console.WriteLine("1. Прямое преобразование (сжатие)");
            Console.WriteLine("2. Обратное преобразование (распаковка) из C");
            Console.WriteLine("3. Выход");
            Console.Write("Выберите пункт: ");
            string cmd = Console.ReadLine();

            switch (cmd)
            {
                case "1": CompressInteractive(); break;
                case "2": DecompressInteractive(); break;
                case "3": return;
                default:
                    Console.WriteLine("Неверный пункт.");
                    break;
            }

            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }

    // ---------------- Compression ----------------
    static void CompressInteractive()
    {
        Console.WriteLine("Введите текст:");
        string text = Console.ReadLine() ?? "";
        byte[] data = Encoding.UTF8.GetBytes(text);

        Console.Write("Введите n1 (размер окна словаря): ");
        int n1 = ReadIntDefault(16);

        Console.Write("Введите n2 (размер буфера просмотра): ");
        int n2 = ReadIntDefault(8);

        Console.WriteLine("\nФормат p и q:");
        Console.WriteLine("1 - десятичная (base 10)");
        Console.WriteLine("2 - двоичная (base 2)");
        Console.WriteLine("3 - четверичная (base 4)");
        Console.Write("Выберите: ");
        string fmtSel = Console.ReadLine()?.Trim() ?? "1";

        string fmt = "dec";
        if (fmtSel == "1") fmt = "dec";
        else if (fmtSel == "2") fmt = "bin";
        else if (fmtSel == "3") fmt = "base4";
        else
        {
            Console.WriteLine("Неверный выбор, использую десятичную.");
            fmt = "dec";
        }

        Console.Write("Введите мощность алфавита: ");
        int N = ReadIntDefault(2);
        if (N < 2) N = 2;

        int pWidth = Math.Max(1, (int)Math.Ceiling(Math.Log(n1) / Math.Log(N)));
        int qWidth = Math.Max(1, (int)Math.Ceiling(Math.Log(n2) / Math.Log(N)));

        Console.WriteLine($"\nАлфавит мощностью A = {N}");
        Console.WriteLine($"Вычислено: pWidth = {pWidth}, qWidth = {qWidth} (по основанию A={N})");
        Console.WriteLine();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var triples = LZ77CompressWithSteps(data, n1, n2, fmt, pWidth, qWidth, out List<string[]> CiList);
        sw.Stop();

        Console.WriteLine("\n=== Ci ПО ШАГАМ ===");
        for (int i = 0; i < CiList.Count; i++)
        {
            var ci = CiList[i];
            Console.WriteLine($"C{i + 1} = {ci[0]} {ci[1]} {ci[2]}");
        }

        // build unified C: p+q+s (s encoded: if control -> \r or \n, else raw char)
        StringBuilder C = new StringBuilder();
        foreach (var ci in CiList)
        {
            C.Append(ci[0]);
            C.Append(ci[1]);
            C.Append(ci[2]);
        }

        Console.WriteLine("\n=== C (единая строка) ===");
        Console.WriteLine(C.ToString());

        Console.WriteLine("\n=== Статистика ===");
        int inputSize = data.Length;

        string Cstr = C.ToString();
        int outputSize = Encoding.ASCII.GetByteCount(Cstr);

        double R1 = (double)outputSize / inputSize * 100.0;
        double R2 = 100.0 - R1;

        Console.WriteLine($"Исходный размер (байт): {inputSize}");
        Console.WriteLine($"Размер выходной строки C (байт): {outputSize}");
        Console.WriteLine($"Число триад: {triples.Count}");
        Console.WriteLine($"R1 = {R1:F2}%");
        Console.WriteLine($"R2 = {R2:F2}%");

        Console.WriteLine($"Время сжатия: {sw.Elapsed.TotalMilliseconds:F4} мс");

    }

    static List<Triple> LZ77CompressWithSteps(byte[] data, int n1, int n2, string fmt, int pWidth, int qWidth, out List<string[]> CiOut)
    {
        var list = new List<Triple>();
        CiOut = new List<string[]>();

        int pos = 0;
        int len = data.Length;
        int step = 0;

        while (pos < len)
        {
            step++;
            string dictStr = BuildDictionaryDisplay(data, pos, n1);
            string bufStr = BuildBufferDisplay(data, pos, n2);

            int bestLen = 0;
            int bestOff = 0;
            int searchStart = Math.Max(0, pos - n1);
            int maxLook = Math.Min(n2, len - pos);

            // поиск совпадения: максимальная длина, при равной — меньший offset (ближе к буферу)
            for (int i = searchStart; i < pos; i++)
            {
                int k = 0;
                while (k < maxLook && data[i + k] == data[pos + k]) k++;

                if (k > bestLen)
                {
                    bestLen = k;
                    bestOff = pos - i;
                }
                else if (k == bestLen && k > 0)
                {
                    int candidateOff = pos - i;
                    if (candidateOff < bestOff)
                        bestOff = candidateOff;
                }
            }

            // обработка ситуации "ушло в конец" — уменьшаем bestLen и берём s
            char? next = null;
            if (bestLen > 0 && pos + bestLen >= len)
            {
                bestLen--;
                if (pos + bestLen < len) next = (char)data[pos + bestLen];
            }
            else
            {
                if (pos + bestLen < len) next = (char)data[pos + bestLen];
            }

            var t = new Triple(bestOff, bestLen, next);
            list.Add(t);

            // форматируем p и q в выбранной системе
            string pFmt = FormatNumberForOutput(t.Offset, fmt, pWidth);
            string qFmt = FormatNumberForOutput(t.Length, fmt, qWidth);

            // s as token for C: if control -> use escape sequences "\r" or "\n" (two chars)
            string sToken = t.Next.HasValue ? EncodeSToken(t.Next.Value) : "";

            CiOut.Add(new string[] { pFmt, qFmt, sToken });

            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"Шаг {step}:");
            Console.WriteLine($"| Словарь: {dictStr}");
            Console.WriteLine($"| Буфер:   {bufStr}");
            Console.WriteLine($"| Триада = ({t.Offset},{t.Length},{(t.Next.HasValue ? EscapeForOutput(t.Next.Value) : "")})");
            Console.WriteLine($"| Ci = {pFmt} {qFmt} {ShowPrintableToken(sToken)}");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine();

            pos += bestLen + 1;
        }

        return list;
    }

    // ---------------- Decompression ----------------
    static void DecompressInteractive()
    {
        Console.WriteLine("LZ77: распаковка из C (слитная строка)");
        Console.Write("Введите строку C (слитно, без пробелов): ");
        string C = Console.ReadLine() ?? "";

        Console.Write("Введите n1 (размер окна словаря): ");
        int n1 = ReadIntDefault(16);

        Console.Write("Введите n2 (размер буфера просмотра): ");
        int n2 = ReadIntDefault(8);

        Console.WriteLine("\nФормат p и q:");
        Console.WriteLine("1 - десятичная (base 10)");
        Console.WriteLine("2 - двоичная (base 2)");
        Console.WriteLine("3 - четверичная (base 4)");
        Console.Write("Выберите: ");
        string fmtSel = Console.ReadLine()?.Trim() ?? "1";

        string fmt = "dec";
        if (fmtSel == "1") fmt = "dec";
        else if (fmtSel == "2") fmt = "bin";
        else if (fmtSel == "3") fmt = "base4";
        else
        {
            Console.WriteLine("Неверный выбор, использую десятичную.");
            fmt = "dec";
        }

        Console.Write("Введите мощность алфавита: ");
        int N = ReadIntDefault(2);
        if (N < 2) N = 2;

        int pWidth = Math.Max(1, (int)Math.Ceiling(Math.Log(n1) / Math.Log(N)));
        int qWidth = Math.Max(1, (int)Math.Ceiling(Math.Log(n2) / Math.Log(N)));

        Console.WriteLine($"Вычислено: pWidth = {pWidth}, qWidth = {qWidth} (по основанию A={N})");
        Console.WriteLine();

        var triples = ParseCStringToTriples(C, pWidth, qWidth, fmt);

        Console.WriteLine("\n=== РАСШИФРОВАННЫЕ ТРИАДЫ ===");
        for (int i = 0; i < triples.Count; i++)
        {
            var t = triples[i];
            Console.WriteLine($"C{i + 1}: p={t.Offset}, q={t.Length}, s={(t.Next.HasValue ? ShowPrintable(t.Next.Value) : "(null)")}");
        }

        var outBytes = LZ77Decompress(triples);
        Console.WriteLine("\n=== ВОССТАНОВЛЕННЫЙ ТЕКСТ ===");
        Console.WriteLine(Encoding.UTF8.GetString(outBytes));
    }

    // Parse concatenated C string into triples (pWidth,qWidth,sToken)
    static List<Triple> ParseCStringToTriples(string C, int pWidth, int qWidth, string fmt)
    {
        var triples = new List<Triple>();
        int i = 0;
        int len = C.Length;
        int step = 0;

        while (i < len)
        {
            step++;
            if (i + pWidth > len)
            {
                Console.WriteLine($"[{step}] Недостаточно символов для p (ожидалось {pWidth}, осталось {len - i}) — прекращаю.");
                break;
            }
            string pStr = C.Substring(i, pWidth); i += pWidth;

            if (i + qWidth > len)
            {
                // неполный q — берем все, затем корректируем: последний символ станет s
                string qPartial = C.Substring(i, Math.Max(0, len - i));
                if (qPartial.Length == 0)
                {
                    int p = ParseNumberFromString(pStr, fmt);
                    triples.Add(new Triple(p, 0, null));
                    Console.WriteLine($"[{step}] Прочитан p='{pStr}'->{p}, q отсутствует -> q=0, s=null");
                    break;
                }
                else
                {
                    // последний символ qPartial -> s, остальное -> qDigits
                    string qDigits = qPartial.Substring(0, qPartial.Length - 1);
                    char sChar = qPartial[qPartial.Length - 1];
                    int p = ParseNumberFromString(pStr, fmt);
                    int qVal = qDigits.Length > 0 ? ParseNumberFromString(qDigits, fmt) : 0;
                    char? sVal = DecodeSTokenAtString(sChar.ToString());
                    triples.Add(new Triple(p, qVal, sVal));
                    Console.WriteLine($"[{step}] pStr='{pStr}'->{p}, неполный q '{qPartial}' -> q={qVal}, s='{ShowPrintable(sVal ?? (char)0)}'");
                    break;
                }
            }

            string qStr = C.Substring(i, qWidth); i += qWidth;

            // now s may be an escaped token starting with '\' (two chars) or a single char
            if (i >= len)
            {
                // no s — reduce q by 1 and take last digit of qStr as s
                if (qStr.Length >= 1)
                {
                    string qDigits = qStr.Substring(0, qStr.Length - 1);
                    char sChar = qStr[qStr.Length - 1];
                    int pVal = ParseNumberFromString(pStr, fmt);
                    int qVal = qDigits.Length > 0 ? ParseNumberFromString(qDigits, fmt) : 0;
                    char? sVal = DecodeSTokenAtString(sChar.ToString());
                    triples.Add(new Triple(pVal, qVal, sVal));
                    Console.WriteLine($"[{step}] pStr='{pStr}'->{pVal}; qStr='{qStr}'(no s) -> adjusted q={qVal}, s='{ShowPrintable(sVal ?? (char)0)}'");
                    break;
                }
                else
                {
                    int pVal = ParseNumberFromString(pStr, fmt);
                    triples.Add(new Triple(pVal, 0, null));
                    Console.WriteLine($"[{step}] pStr='{pStr}'->{pVal}; q empty, s absent -> q=0");
                    break;
                }
            }

            // detect escaped s: starts with '\' e.g. "\r" or "\n" stored as two characters '\' + 'r' / 'n'
            char nextChar = C[i];
            string sToken;
            if (nextChar == '\\' && i + 1 < len)
            {
                sToken = C.Substring(i, 2);
                i += 2;
            }
            else
            {
                sToken = C.Substring(i, 1);
                i += 1;
            }

            int pVal2 = ParseNumberFromString(pStr, fmt);
            int qVal2 = ParseNumberFromString(qStr, fmt);
            char? sVal2 = DecodeSTokenAtString(sToken);

            triples.Add(new Triple(pVal2, qVal2, sVal2));
            Console.WriteLine($"[{step}] pStr='{pStr}'->{pVal2}, qStr='{qStr}'->{qVal2}, s='{ShowPrintable(sVal2 ?? (char)0)}'");
        }

        return triples;
    }

    // ---------------- LZ77 Decompress core ----------------
    static byte[] LZ77Decompress(List<Triple> triples)
    {
        var outb = new List<byte>();

        foreach (var t in triples)
        {
            if (t.Offset == 0 || t.Length == 0)
            {
                if (t.Next.HasValue)
                    outb.Add((byte)t.Next.Value);
                continue;
            }

            int start = outb.Count - t.Offset;
            if (start < 0) start = 0;
            for (int k = 0; k < t.Length; k++)
            {
                int idx = start + k;
                if (idx >= 0 && idx < outb.Count)
                    outb.Add(outb[idx]);
                else
                    outb.Add(0);
            }

            if (t.Next.HasValue) outb.Add((byte)t.Next.Value);
        }

        return outb.ToArray();
    }

    // ----------------- helpers -----------------

    class Triple
    {
        public int Offset;
        public int Length;
        public char? Next;
        public Triple(int p, int q, char? s) { Offset = p; Length = q; Next = s; }
    }

    static int ReadIntDefault(int def)
    {
        string s = Console.ReadLine();
        if (int.TryParse(s, out int v) && v > 0) return v;
        return def;
    }

    static string BuildDictionaryDisplay(byte[] d, int pos, int n1)
    {
        var sb = new StringBuilder();
        int start = pos - n1;
        for (int i = 0; i < n1; i++)
        {
            int idx = start + i;
            if (idx < 0 || idx >= d.Length) sb.Append('.');
            else sb.Append(DisplayChar(d[idx]));
        }
        return sb.ToString();
    }

    static string BuildBufferDisplay(byte[] d, int pos, int n2)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < n2; i++)
        {
            int idx = pos + i;
            if (idx < 0 || idx >= d.Length) sb.Append('.');
            else sb.Append(DisplayChar(d[idx]));
        }
        return sb.ToString();
    }

    static char DisplayChar(byte b)
    {
        char c = (char)b;
        if (c == '\r') return '␍';
        if (c == '\n') return '␊';
        if (char.IsControl(c)) return '.';
        return c;
    }

    static string EscapeForOutput(char c)
    {
        return c switch
        {
            '\r' => "\\r",
            '\n' => "\\n",
            _ => c.ToString(),
        };
    }

    static string ShowPrintable(char c)
    {
        if (c == '\r') return "<CR>";
        if (c == '\n') return "<LF>";
        return c.ToString();
    }

    static string ShowPrintableToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return "";
        if (token == "\\r") return "<CR>";
        if (token == "\\n") return "<LF>";
        return token;
    }

    static string FormatNumberForOutput(int value, string fmt, int width)
    {
        string s;
        if (fmt == "dec") s = value.ToString();
        else if (fmt == "bin") s = Convert.ToString(value, 2);
        else /* base4 */ s = ConvertToBase(value, 4);

        if (s.Length < width) s = new string('0', width - s.Length) + s;
        return s;
    }

    static string ConvertToBase(int value, int radix)
    {
        if (value == 0) return "0";
        const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (radix < 2 || radix > digits.Length)
            throw new ArgumentException("radix");

        bool neg = value < 0;
        int v = Math.Abs(value);
        StringBuilder sb = new StringBuilder();

        while (v > 0)
        {
            int rem = v % radix;
            v /= radix;
            sb.Insert(0, digits[rem]);
        }

        if (neg) sb.Insert(0, '-');

        return sb.ToString();
    }

    static int ParseNumberFromString(string s, string fmt)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        try
        {
            if (fmt == "dec") return int.Parse(s);
            if (fmt == "bin") return Convert.ToInt32(s, 2);
            if (fmt == "base4") return Base4ToDec(s);
            return Convert.ToInt32(s, 4);
        }
        catch
        {
            Console.WriteLine($"Warning: can't parse '{s}' as {fmt}, returning 0.");
            return 0;
        }
    }

    static int Base4ToDec(string s)
    {
        int result = 0;
        foreach (char c in s)
        {
            if (c < '0' || c > '3')
                throw new Exception($"Недопустимый символ '{c}' в числе base4");

            int digit = c - '0';
            result = result * 4 + digit;
        }
        return result;
    }

    static string EncodeSToken(char c)
    {
        if (c == '\r') return "\\r";
        if (c == '\n') return "\\n";
        return c.ToString();
    }

    static char? DecodeSTokenAtString(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;
        if (token == "\\r") return '\r';
        if (token == "\\n") return '\n';
        return token[0];
    }
}
