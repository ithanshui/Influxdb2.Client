﻿using System;
using System.Reflection;

namespace Influxdb2.Client.Datas
{
    /// <summary>
    /// 表示数据点的属性描述
    /// </summary>
    sealed class PointDataPropertyDescriptor : PropertyDescriptor<object, object?>
    {
        /// <summary>
        /// 获取列类型
        /// </summary>
        public ColumnType ColumnType { get; }

        /// <summary>
        /// 值转换器
        /// </summary>
        private readonly Func<object?, string?> valueConverter;

        /// <summary>
        /// 属性描述
        /// </summary>
        /// <param name="property">属性信息</param>
        public PointDataPropertyDescriptor(PropertyInfo property)
            : base(property)
        {
            var attr = property.GetCustomAttribute<ColumnTypeAttribute>();
            if (attr != null)
            {
                this.ColumnType = attr.ColumnType;
            }

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (this.ColumnType == ColumnType.Timestamp)
            {
                this.valueConverter = GetTimestampConverter(type);
            }
            else if (this.ColumnType == ColumnType.Field)
            {
                this.valueConverter = LineProtocolUtil.CreateFieldValueEncoder(type);
            }
            else // 标签
            {
                this.valueConverter = LineProtocolUtil.EncodeTagValue;
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public string? GetStringValue(object instance)
        {
            var value = base.GetValue(instance);
            return this.valueConverter.Invoke(value);
        }

        /// <summary>
        /// 获取时间戳转换器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Func<object?, string?> GetTimestampConverter(Type type)
        {
            if (type == typeof(DateTime))
            {
                return value => LineProtocolUtil.GetNsTimestamp((DateTime?)value).ToString();
            }

            if (type == typeof(DateTimeOffset))
            {
                return value => LineProtocolUtil.GetNsTimestamp((DateTimeOffset?)value)?.ToString();
            }

            throw new NotSupportedException($"属性{type} {this.Name}不支持转换为Timestamp");
        }
    }
}