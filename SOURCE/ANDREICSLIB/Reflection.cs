﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ANDREICSLIB
{
    public static class Reflection
    {
        public static char separator = '\f';
        public static string newline = "\r\n";

        /// <summary>
        /// get the name of a passed parameter
        /// </summary>
        /// <param name="memberExpression">() => variable</param>
        /// <returns>variable name</returns>
        public static string GetFieldName(Expression<Func<object>> memberExpression)
        {
            MemberExpression me=null;
            if (memberExpression.Body is MemberExpression)
                me = ((MemberExpression) memberExpression.Body);
            else if (memberExpression.Body is UnaryExpression)
            {
                var ue = ((UnaryExpression) memberExpression.Body);
                me = ue.Operand as MemberExpression;
            }

            if (me == null)
                return null;

            return me.Member.Name;
        }

        /// <summary>
        /// get a field or property of a class instance
        /// </summary>
        /// <param name="classInstance"></param>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        public static object GetFieldValue(object classInstance, String fieldname)
        {
            object ret = null;
            var ty = classInstance.GetType();
            var field = ty.GetField(fieldname);
            var field2 = ty.GetProperty(fieldname);

            if (field != null)
                ret = field.GetValue(classInstance);
            else if (field2 != null)
                ret = field2.GetValue(classInstance,null);
            return ret;
        }

        /// <summary>
        /// get a tuple list of the type name and type values of an object
        /// </summary>
        /// <param name="classInstance">the class you want the values for</param>
        /// <returns></returns>
        public static List<Tuple<string, object>> GetFieldNamesAndValues(object classInstance)
        {
            var ty = classInstance.GetType();
            var fields = ty.GetFields();

            return fields.Select(v => new Tuple<string, object>(v.Name, v.GetValue(classInstance))).ToList();
        }

        public static List<string> GetFieldNames(Type ty)
        {
            var fields = ty.GetFields();

            return fields.Select(v => v.Name).ToList();
        }

        /// <summary>
        /// serialise an object to a file
        /// </summary>
        /// <param name="classInstance"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool SerialiseObject(object classInstance, string filename)
        {
            if (File.Exists(filename) == false)
                FileUpdates.CreateFile(filename);

            var r = SerialiseObject(classInstance);

            FileUpdates.SaveToFile(filename, r);
            return true;
        }

        /// <summary>
        /// serialise an object to a return string
        /// </summary>
        /// <param name="classInstance"></param>
        /// <returns></returns>
        public static string SerialiseObject(object classInstance)
        {
            String r = "";
            var ol = GetFieldNamesAndValues(classInstance);

            foreach (var o in ol)
            {
                r += o.Item1 + separator + o.Item2 + newline;
            }
            return r;
        }

        /// <summary>
        /// deserialise a file to an object from a file
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="filename"></param>
        /// <param name="ignoreErrors"></param>
        /// <returns></returns>
        public static object DeserialiseObject(String filename, Type objectType, bool ignoreErrors = true)
        {
            if (File.Exists(filename) == false)
                return null;

            var s = FileUpdates.LoadFile(filename);

            if (string.IsNullOrEmpty(s))
                return null;

            var instance = DeserialiseObject(objectType, s, ignoreErrors);
            return instance;
        }

        /// <summary>
        /// deserialise an object from a serialised string
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="serialisedObjectString"></param>
        /// <param name="ignoreErrors"></param>
        /// <returns></returns>
        public static object DeserialiseObject(Type objectType, String serialisedObjectString, bool ignoreErrors = true)
        {
            var s2 = StringUpdates.SplitString(serialisedObjectString, newline);

            var tl = new List<Tuple<string, String>>();
            foreach (var s3 in s2)
            {
                var s4 = StringUpdates.SplitString(s3, separator.ToString());
                if (s4.Length != 2)
                {
                    if (ignoreErrors)
                        continue;
                    return null;
                }

                var fieldname = s4[0];
                var fieldval = s4[1];

                tl.Add(new Tuple<string, string>(fieldname, fieldval));
            }
            var instance = DeserialiseObject(objectType, tl, ignoreErrors);
            return instance;
        }

        /// <summary>
        /// deserialise an object from a list of tuple string,string s
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectFieldNameAndValues">field name,field val</param>
        /// <param name="ignoreErrors"></param>
        /// <returns></returns>
        public static object DeserialiseObject(Type objectType, List<Tuple<String, String>> objectFieldNameAndValues,
                                         bool ignoreErrors = true)
        {
            var instance = Activator.CreateInstance(objectType);
            foreach (var t in objectFieldNameAndValues)
            {
                var field = objectType.GetField(t.Item1);
                if (field == null)
                {
                    if (ignoreErrors)
                        continue;
                    return null;
                }
                
                try
                {
                    field.SetValue(instance, Convert.ChangeType(t.Item2, field.FieldType));
                }
                catch (Exception)
                {
                    if (ignoreErrors)
                        continue;
                    return null;
                }
                
            }
            return instance;
        }
    }
}