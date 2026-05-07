using System;
using System.IO;
using System.Data.SqlTypes;
using System.Globalization;
using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedType(Format.UserDefined, IsByteOrdered = false, MaxByteSize = 8000)]
public struct LicenseData : INullable, IBinarySerialize
{
    private int _licenseId;
    private string _softwareName;
    private string _licenseKey;
    private DateTime _expirationDate;
    private decimal _price;
    private bool _isNull;

    public int LicenseId
    {
        get { return _licenseId; }
        set { _licenseId = value; }
    }
    public string SoftwareName
    {
        get { return _softwareName; }
        set { _softwareName = value; }
    }
    public string LicenseKey
    {
        get { return _licenseKey; }
        set { _licenseKey = value; }
    }
    public DateTime ExpirationDate
    {
        get { return _expirationDate; }
        set { _expirationDate = value; }
    }
    public decimal Price
    {
        get { return _price; }
        set { _price = value; }
    }
    public bool IsNull
    {
        get { return _isNull; }
    }
    public static LicenseData Null
    {
        get
        {
            LicenseData license = new LicenseData();
            license._isNull = true;
            return license;
        }
    }

    public static LicenseData Parse(SqlString s)
    {
        if (s.IsNull || s.Value == "NULL")
            return Null;

        string[] parts = s.Value.Split('|');

        if (parts.Length != 5)
            throw new ArgumentException("Неверный формат. Ожидается: ID|Название ПО|Ключ лицензии|Дата окончания|Стоимость");

        LicenseData license = new LicenseData();
        license._licenseId = int.Parse(parts[0]);
        license._softwareName = parts[1];
        license._licenseKey = parts[2];
        license._expirationDate = DateTime.Parse(parts[3]);
        license._price = decimal.Parse(parts[4].Replace(',', '.'), CultureInfo.InvariantCulture);
        license._isNull = false;

        return license;
    }

    public override string ToString()
    {
        if (_isNull)
            return "NULL";

        return string.Format(
            CultureInfo.InvariantCulture,
            "{0}|{1}|{2}|{3:yyyy-MM-dd}|{4:F2}",
            _licenseId,
            _softwareName,
            _licenseKey,
            _expirationDate,
            _price
        );
    }

    [SqlMethod]
    public SqlString GetLicenseInfo()
    {
        if (_isNull)
            return SqlString.Null;

        return new SqlString(string.Format(
            CultureInfo.InvariantCulture,
            "Лицензия №{0}. ПО: {1}. Ключ: {2}. Дата окончания: {3:yyyy-MM-dd}. Стоимость: {4:F2}",
            _licenseId,
            _softwareName,
            _licenseKey,
            _expirationDate,
            _price
        ));
    }

    [SqlMethod]
    public SqlBoolean IsExpired()
    {
        if (_isNull)
            return SqlBoolean.Null;

        return new SqlBoolean(_expirationDate.Date < DateTime.Today);
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_isNull);

        if (!_isNull)
        {
            writer.Write(_licenseId);
            writer.Write(_softwareName ?? "");
            writer.Write(_licenseKey ?? "");
            writer.Write(_expirationDate.ToBinary());
            writer.Write(_price);
        }
    }

    public void Read(BinaryReader reader)
    {
        _isNull = reader.ReadBoolean();

        if (!_isNull)
        {
            _licenseId = reader.ReadInt32();
            _softwareName = reader.ReadString();
            _licenseKey = reader.ReadString();
            _expirationDate = DateTime.FromBinary(reader.ReadInt64());
            _price = reader.ReadDecimal();
        }
    }
}

public class FileOperations
{
    [SqlProcedure]
    public static void ReadLicenseFile(SqlString filePath)
    {
        if (filePath.IsNull)
            throw new ArgumentException("Путь к файлу не может быть NULL");

        string path = filePath.Value;

        if (!File.Exists(path))
            throw new FileNotFoundException("Файл не найден: " + path);

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            SqlContext.Pipe.Send(line);
        }
    }
}
