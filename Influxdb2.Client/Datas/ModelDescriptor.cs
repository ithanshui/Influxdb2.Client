﻿using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Influxdb2.Client.Datas
{
    /// <summary>
    /// 表示模型描述器
    /// </summary>
    sealed class ModelDescriptor
    {
        /// <summary>
        /// 获取所的属性描述
        /// </summary>
        public ModelPropertyDescriptor[] PropertyDescriptors { get; }

        /// <summary>
        /// 模型描述器
        /// </summary>
        /// <param name="modelType">模型类型</param>
        public ModelDescriptor(Type modelType)
        {
            this.PropertyDescriptors = modelType
                .GetProperties()
                .Where(item => item.CanWrite)
                .Select(item => new ModelPropertyDescriptor(item))
                .ToArray();
        }

        /// <summary>
        /// ModelDescriptor缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ModelDescriptor> cache = new ConcurrentDictionary<Type, ModelDescriptor>();

        /// <summary>
        /// 获取ModelDescriptor描述
        /// </summary>
        /// <param name="modelType">模型类型</param>
        /// <returns></returns>
        public static ModelDescriptor Get(Type modelType)
        {
            return cache.GetOrAdd(modelType, t => new ModelDescriptor(t));
        }
    }
}
