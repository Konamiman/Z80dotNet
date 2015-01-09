using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Konamiman.NestorMSX.Misc
{
	public class IniDeserializer
	{
		private IniDeserializer()	{}

		public static string pTrueString=Boolean.TrueString;
		public static string pFalseString=Boolean.FalseString;

		public static string TrueString
		{
			get {return pTrueString;}
			set
			{
				if(value==null || value.Trim()=="")
					pTrueString=Boolean.TrueString;
				else
					pTrueString=value.Trim();
			}
		}

		public static string FalseString
		{
			get {return pFalseString;}
			set
			{
				if(value==null || value.Trim()=="")
					pFalseString=Boolean.FalseString;
				else
					pFalseString=value.Trim();
			}
		}

		#region Deserialization

		protected static StringDictionary DeserializeToDictionary(string fileName)
		{
			Stream stream=new FileStream(fileName,FileMode.Open,FileAccess.Read);
			StringDictionary sd=DeserializeToDictionary(stream);
			stream.Close();
			return sd;
		}

		protected static StringDictionary DeserializeToDictionary(Stream stream)
		{
			StringDictionary Items=new CasePreserverStringDictionary();
			string RawText=(new StreamReader(stream,Encoding.GetEncoding(1252))).ReadToEnd();

			Match m=Regex.Match(RawText,@"^[ \t]*(?<key>[^;#=[][^=\n\r\t ]*)[ \t]*(=[ \t]*(?<value>[^\n\r]*))?[ \t]*(?=\r\n)",RegexOptions.Multiline);
			while(m.Success)
			{
				Items.Add(m.Groups["key"].ToString(),m.Groups["value"].ToString());
				m=m.NextMatch();
			}
			return Items;
		}
		
        public static T Deserialize<T>(string fileName) where T : new()
        {
            return (T)Deserialize(fileName, typeof(T));
        }
        
        public static object Deserialize(Stream stream, Type graphType)
		{
			StringDictionary Items=DeserializeToDictionary(stream);
			return Deserialize(Items,graphType);
		}

		public static object Deserialize(string fileName, Type graphType)
		{
			StringDictionary Items=DeserializeToDictionary(fileName);
			return Deserialize(Items,graphType);
		}

		protected static object Deserialize(StringDictionary items,Type graphType)
		{
			string[] TrueValues=new string[] {"yes","1",Boolean.TrueString.ToLower(),pTrueString.ToLower()};
			string[] FalseValues=new string[] {"no","0",Boolean.FalseString.ToLower(),pFalseString.ToLower()};

			object graph=graphType.InvokeMember(null,BindingFlags.CreateInstance,null,null,null);

			MethodInfo OnUnknownKeyMethod=graphType.GetMethod("OnUnknownKey",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.IgnoreCase,
				null,new Type[] {typeof(string),typeof(string)},null);

			MethodInfo ParseBooleanMethod=graphType.GetMethod("ParseBoolean",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.IgnoreCase,
				null,new Type[] {typeof(string),typeof(string)},null);

			MethodInfo OnDeserializationExceptionMethod=graphType.GetMethod("OnDeserializationException",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.IgnoreCase,
				null,new Type[] {typeof(string),typeof(string),typeof(Exception)},null);

			string[] Keys=new string[items.Count];
			items.Keys.CopyTo(Keys,0);

			foreach(string key in Keys)
			{
				MethodInfo DeserializeMethod=graphType.GetMethod("Deserialize"+key,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase,
					null,new Type[] {typeof(string)},null);
				if(DeserializeMethod!=null)
				{
					try
					{
						DeserializeMethod.Invoke(graph,new object[] {items[key]});
					}
					catch(Exception ex)
					{
						if(OnDeserializationExceptionMethod!=null)
						{
							OnDeserializationExceptionMethod.Invoke(graph,new object[] {key,items[key],ex.InnerException});
						}
						else
							throw new DeserializationException(
								"Deserialization error (method), key={" + key + "}, value={" + items[key] + "}",
								key,items[key],ex.InnerException);
					}
					continue;
				}

				PropertyInfo DeserializeField=graphType.GetProperty(key,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

				if(DeserializeField!=null)
				{
					if(DeserializeField.PropertyType==typeof(string))
					{
						DeserializeField.SetValue(graph,items[key]);
						continue;
					}

					if(DeserializeField.PropertyType==typeof(bool))
					{
						string Val=items[key].ToLower();
						if(Array.IndexOf(TrueValues,Val)!=-1)
						{
							DeserializeField.SetValue(graph,true);
							continue;
						}
						if(Array.IndexOf(FalseValues,Val)!=-1)
						{
							DeserializeField.SetValue(graph,false);
							continue;
						}
						if(ParseBooleanMethod!=null && ParseBooleanMethod.ReturnType==typeof(bool))
						{
							DeserializeField.SetValue(graph,
								(bool)ParseBooleanMethod.Invoke(graph,new string[] {key,items[key]}));
							continue;
						}
					}
					
					MethodInfo FieldParseMethod=ParseMethod(DeserializeField.PropertyType);
					if(FieldParseMethod!=null && FieldParseMethod.ReturnType==DeserializeField.PropertyType)
					{
						object ParsedObject;
						
						try
						{
							ParsedObject=FieldParseMethod.Invoke(null,new object[] {items[key]});
						}
						catch(Exception ex)
						{
							if(OnDeserializationExceptionMethod!=null)
							{
								OnDeserializationExceptionMethod.Invoke(graph,new object[] {key,items[key],ex.InnerException});
								continue;
							}
							else
								throw new DeserializationException(
									"Deserialization error (field), key={" + key + "}, value={" + items[key] + "}",
									key,items[key],ex.InnerException);
						}
						
						DeserializeField.SetValue(graph,ParsedObject);
						continue;
					}
				}

				if(OnUnknownKeyMethod!=null)
				{
					OnUnknownKeyMethod.Invoke(graph,new object[] {key,items[key]});
				}

			}
			
			MethodInfo OnDeserializationMethod=graphType.GetMethod("OnDeserialization",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase,
				null,new Type[0],null);
			if(OnDeserializationMethod!=null)
			{
				OnDeserializationMethod.Invoke(graph,null);
			}

			return graph;
		}

        #endregion

        #region Auxiliary methods

		protected static MethodInfo ToStringMethod(Type type)
		{
			return type.GetMethod("ToString",
				BindingFlags.Instance | BindingFlags.Public,
				null,new Type[0],null);
		}

		protected static MethodInfo ParseMethod(Type type)
		{
			return type.GetMethod("Parse",
				BindingFlags.Static | BindingFlags.Public,
				null,new[] {typeof(string)},null);
		}

	    private static MemberInfo[] GetSerializableMembers(Type type,bool methods)
		{
			return type.FindMembers(methods?MemberTypes.Method:MemberTypes.Field,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				new MemberFilter(IsSerializableMember),methods);
		}

		private static bool IsSerializableMember(MemberInfo m,object filterCriteria)
		{
			if((bool)filterCriteria)
			{
				MethodInfo Method=(MethodInfo)m;
				return(Method.Name.ToUpper().StartsWith("SERIALIZE") &&
					Method.Name.Length>"SERIALIZE".Length &&
					Method.GetParameters().Length==0 &&
					Method.ReturnType==typeof(string));
			}
			else
			{
				FieldInfo Field=(FieldInfo)m;
				return (ToStringMethod(Field.FieldType)!=null  && !Field.IsNotSerialized);
			}
		}

        #endregion
    }

	#region CasePreserverStringDictionary class

	public class CasePreserverStringDictionary:StringDictionary
	{
		protected StringDictionary OriginalKeys=new StringDictionary();

		public override void Add(string key,string value)
		{
			OriginalKeys.Add(key,key);
			base.Add(key,value);
		}

		public override void Clear()
		{
			OriginalKeys.Clear();
			base.Clear();
		}

		public override void Remove(string key)
		{
			OriginalKeys.Remove(key);
			base.Remove(key);
		}

		public override ICollection Keys
		{
			get	{return OriginalKeys.Values;}
		}

		public string OriginalKeyName(string key)
		{
			return OriginalKeys[key];
		}

	}

	#endregion

    #region DeserializationException class

	public class DeserializationException:ApplicationException
	{
		public string Key;
		public string Value;

		public DeserializationException(string message, string key, string value, Exception ex):
			base(message,ex)
		{
			Key=key;
			Value=value;
		}

		public DeserializationException(string message, string key, string value):
			base(message)
		{
			Key=key;
			Value=value;
		}

		public DeserializationException(string key, string value):
			base()
		{
			Key=key;
			Value=value;
		}
	}
	
	#endregion
 }
