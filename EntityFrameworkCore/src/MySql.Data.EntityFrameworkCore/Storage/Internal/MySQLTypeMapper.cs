﻿// Copyright © 2015, 2017 Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using MySQL.Data.EntityFrameworkCore;

namespace MySql.Data.EntityFrameworkCore.Storage.Internal
{
  public class MySQLTypeMapper : RelationalTypeMapper
  {
    private static int _longTextMaxLength = int.MaxValue;
    private static int _medTextMaxLength = 16777215;
    private static int _textMaxLength = 65535;
    private static int _keyMaxLength = 767;
    private static int _tinyMaxLength = 255;

    private readonly RelationalTypeMapping _int = new RelationalTypeMapping("int", typeof(Int32));
    private readonly RelationalTypeMapping _bigint = new RelationalTypeMapping("bigint", typeof(Int64));
    private readonly RelationalTypeMapping _bit = new RelationalTypeMapping("bit", typeof(short));
    private readonly RelationalTypeMapping _smallint = new RelationalTypeMapping("smallint", typeof(Int16));
    private readonly RelationalTypeMapping _tinyint = new RelationalTypeMapping("tinyint", typeof(Byte));
    private readonly RelationalTypeMapping _char = new RelationalTypeMapping("char", typeof(string));

    private readonly RelationalTypeMapping _varchar = new MySQLSizeableMapping($"varchar({_textMaxLength})", typeof(string), dbType: null, unicode: false, size: _textMaxLength, hasNonDefaultUnicode: true);

    private readonly RelationalTypeMapping _varcharkey = new MySQLSizeableMapping($"varchar({_keyMaxLength})", typeof(string), dbType: null, unicode: false, size: _keyMaxLength, hasNonDefaultUnicode: true);

    private readonly MySQLSizeableMapping _nvarchar
               = new MySQLSizeableMapping($"nvarchar({_textMaxLength})", typeof(string), dbType: null, unicode: true, size: _textMaxLength);

    private readonly RelationalTypeMapping _rowversion = new RelationalTypeMapping("timestamp", typeof(DateTime), dbType: DbType.DateTime);

    private readonly RelationalTypeMapping _longText = new RelationalTypeMapping("longtext", typeof(string));
    private readonly RelationalTypeMapping _mediumText = new RelationalTypeMapping("mediumtext", typeof(string));
    private readonly RelationalTypeMapping _text = new RelationalTypeMapping("text", typeof(string));
    private readonly RelationalTypeMapping _tinyText = new RelationalTypeMapping("tinytext", typeof(string));

    private readonly RelationalTypeMapping _datetime = new RelationalTypeMapping("datetime", typeof(DateTime));
    private readonly RelationalTypeMapping _datetimeoffset = new RelationalTypeMapping("timestamp", typeof(DateTimeOffset), DbType.DateTime);
    private readonly RelationalTypeMapping _date = new RelationalTypeMapping("date", typeof(DateTime));
    private readonly RelationalTypeMapping _time = new RelationalTypeMapping("time", typeof(TimeSpan));
    private readonly RelationalTypeMapping _float = new RelationalTypeMapping("float", typeof(float));
    private readonly RelationalTypeMapping _double = new RelationalTypeMapping("double", typeof(double));
    private readonly RelationalTypeMapping _real = new RelationalTypeMapping("real", typeof(Single));
    private readonly RelationalTypeMapping _decimal = new RelationalTypeMapping("decimal(18, 2)", typeof(Decimal));


    private readonly RelationalTypeMapping _binary = new MySQLBinaryMapping("binary", typeof(byte[]));
    private readonly RelationalTypeMapping _varbinary = new MySQLBinaryMapping("varbinary", typeof(byte[]));
    private readonly RelationalTypeMapping _tinyblob = new RelationalTypeMapping("tinyblob", typeof(byte[]));
    private readonly RelationalTypeMapping _mediumblob = new RelationalTypeMapping("mediumblob", typeof(byte[]));
    private readonly RelationalTypeMapping _blob = new RelationalTypeMapping("blob", typeof(byte[]));
    private readonly RelationalTypeMapping _longblob = new RelationalTypeMapping("longblob", typeof(byte[]));

    private readonly RelationalTypeMapping _enum = new RelationalTypeMapping("enum", typeof(string));
    private readonly RelationalTypeMapping _set = new RelationalTypeMapping("set", typeof(string));


    private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
    private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
    public override IStringRelationalTypeMapper StringMapper { get; }
    public override IByteArrayRelationalTypeMapper ByteArrayMapper { get; }

    public MySQLTypeMapper()
    {
      _storeTypeMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
      {
        { "bigint", _bigint },
        { "decimal", _decimal },
        { "double", _double },
        { "float", _float },
        { "int", _int},
        { "mediumint", _int },
        { "real", _real },
        { "smallint", _smallint },
        { "tinyint", _tinyint },
        { "char", _char },
        { "varchar", _varchar},
        { "tinytext", _tinyText},
        { "text", _text},
        { "mediumtext", _mediumText},
        { "longtext", _longText},
        { "datetime", _datetime },
        { "datetimeoffset", _datetimeoffset },
        { "date", _date },
        { "time", _time },
        { "timestamp", _datetime },
        { "year", _smallint },
        { "bit", _bit },
        { "string", _varchar },
        { "tinyblob", _tinyblob },
        { "blob", _blob },
        { "mediumblob", _mediumblob },
        { "longblob", _longblob },
        { "binary", _binary },
        { "varbinary", _varbinary },
        { "enum", _enum },
        { "set", _set },
        { "json", _varchar }
      };

      _clrTypeMappings = new Dictionary<Type, RelationalTypeMapping>
      {
        { typeof(int), _int },
        { typeof(long), _bigint },
        { typeof(DateTime), _datetime },
        { typeof(DateTimeOffset), _datetimeoffset },
        { typeof(bool), _bit },
        { typeof(byte), _tinyint },
        { typeof(float), _float },
        { typeof(double), _double },
        { typeof(char), _int },
        { typeof(sbyte), new RelationalTypeMapping("smallint", _smallint.GetType()) },
        { typeof(ushort), new RelationalTypeMapping("int", _int.GetType()) },
        { typeof(uint), new RelationalTypeMapping("bigint", _bigint.GetType()) },
        { typeof(ulong), new RelationalTypeMapping("numeric(20, 0)" ,_decimal.GetType()) },
        { typeof(short), _smallint },
        { typeof(decimal), _decimal },
        { typeof(string), _varchar },
        { typeof(byte[]), _varbinary },
        { typeof(TimeSpan), _time }
      };


      StringMapper
            = new StringRelationalTypeMapper(
                _textMaxLength,
                _varchar,
                _varchar,
                _varchar,
                size => new MySQLSizeableMapping(
                    "varchar(" + size + ")",
                    typeof(string),
                    dbType: DbType.AnsiString,
                    unicode: false,
                    size: size,
                    hasNonDefaultUnicode: true,
                    hasNonDefaultSize: true),
                _textMaxLength,
                _nvarchar,
                _nvarchar,
                _nvarchar,
                size => new MySQLSizeableMapping(
                    "nvarchar(" + size + ")",
                    typeof(string),
                    dbType: null,
                    unicode: true,
                    size: size,
                    hasNonDefaultUnicode: false,
                    hasNonDefaultSize: true));



      ByteArrayMapper
          = new ByteArrayRelationalTypeMapper(
              _longTextMaxLength,
              _varbinary,
              _varbinary,
              _varbinary,
              _rowversion, _ => _varbinary);
    }

    protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
    => _clrTypeMappings;

    protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
    => _storeTypeMappings;


    protected override string GetColumnType(IProperty property)
    {
      return property.MySQL().ColumnType;
    }


    public override RelationalTypeMapping FindMapping(Type clrType)
    {
      ThrowIf.Argument.IsNull(clrType, "clrType");
      var sType = Nullable.GetUnderlyingType(clrType) ?? clrType;
      return sType == typeof(string)
          ? _nvarchar
          : (sType == typeof(byte[])
              ? _varbinary
              : base.FindMapping(clrType));
    }

    protected override RelationalTypeMapping GetStringMapping([NotNullAttribute] IProperty property)
    {
      if (RequiresKeyMapping(property))
        return _varcharkey;
      return _text;
    }
  }
}
