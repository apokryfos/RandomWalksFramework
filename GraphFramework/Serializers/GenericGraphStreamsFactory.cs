using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace GraphFramework.Serializers {


	static class ReaderWriterTypes<TVertex>
		where TVertex : IComparable<TVertex>		
	{		
		private static string[] extensions = { ".bin", ".csv", ".txt" };
		private static Type[] readers = { typeof(BinaryGraphReader), typeof(TextGraphReader<TVertex>), typeof(TextGraphReader<TVertex>) };
		private static Type[] writers = { typeof(BinaryGraphWriter), typeof(CSVGraphWriter<TVertex>), typeof(TextGraphWriter<TVertex>) };

		public static KeyValuePair<Type, Type> GetReaderAndWriterType(string extention) {
			
			for (int i = 0; i < extensions.Length; i++) {
				if (extensions[i] == extention) {
					return new KeyValuePair<Type, Type>(readers[i], writers[i]);
				}
			}
			 
			return default(KeyValuePair<Type, Type>);
		}
	}


	public static class GenericGraphReaderFactory<TVertex>	
		where TVertex : IComparable<TVertex> {

		#region Reader Per Extention
		public static IGraphReader<TVertex> GetGraphReaderForExtension(string extension, string filename, int bufferSize) {
			var kv = ReaderWriterTypes<TVertex>.GetReaderAndWriterType(extension);
			if (kv.Equals(default(KeyValuePair<Type, Type>)))
				return null;
			var ctor = kv.Key.GetConstructor(new Type[] { typeof(string), typeof(int) });
			var gwinterface = kv.Key.GetInterface(typeof(IGraphReader<TVertex>).Name);

			if (gwinterface == null) {
				throw new InvalidOperationException();
			}
			if (ctor == null) {
				ctor = kv.Key.GetConstructor(new Type[] { typeof(string) });
				if (ctor != null) {
					return (IGraphReader<TVertex>)Activator.CreateInstance(kv.Key, filename);
				}
			} else {
				return (IGraphReader<TVertex>)Activator.CreateInstance(kv.Key, filename, bufferSize);
			}

			return null;
		}
		public static IGraphReader<TVertex> GetGraphReaderForExtension(string extension, string filename) {
			return GetGraphReaderForExtension(extension, filename, (int)Math.Pow(2, 16));
		}

		public static IGraphReader<TVertex> GetGraphReaderForExtension(string filename) {
			return GetGraphReaderForExtension(filename, (int)Math.Pow(2, 16));
		}

		public static IGraphReader<TVertex> GetGraphReaderForExtension(string filename, int bufferSize) {
			return GetGraphReaderForExtension(Path.GetExtension(filename), filename, bufferSize);

		}
		#endregion

	}


	public static class GenericGraphWriterFactory<TVertex>
		where TVertex : IComparable<TVertex> {




		#region Writer Per Extention
		public static IGraphWriter<TVertex> GetGraphWriterForExtension(string extension, string filename, int bufferSize) {
			var kv = ReaderWriterTypes<TVertex>.GetReaderAndWriterType(extension);
			if (kv.Equals(default(KeyValuePair<Type, Type>)))
				return null;

			var ctor = kv.Value.GetConstructor(new Type[] { typeof(string), typeof(int) });
			var gwinterface = kv.Value.GetInterface(typeof(IGraphWriter<TVertex>).Name);

			if (gwinterface == null)
				throw new InvalidOperationException();
			if (ctor != null) {
				return (IGraphWriter<TVertex>)Activator.CreateInstance(kv.Value, filename, bufferSize);
			} else {
				ctor = kv.Value.GetConstructor(new Type[] { typeof(string) });
				if (ctor != null) {
					return (IGraphWriter<TVertex>)Activator.CreateInstance(kv.Value, filename);
				}
			}
			return null;
		}
		public static IGraphWriter<TVertex> GetGraphWriterForExtension(string extension, string filename) {
			return GetGraphWriterForExtension(extension, filename, (int)Math.Pow(2, 11));
		}

		public static IGraphWriter<TVertex> GetGraphWriterForExtension(string filename) {
			return GetGraphWriterForExtension(filename, (int)Math.Pow(2, 11));
		}

		public static IGraphWriter<TVertex> GetGraphWriterForExtension(string filename, int bufferSize) {
			return GetGraphWriterForExtension(Path.GetExtension(filename), filename, bufferSize);

		}

		#endregion
	}

}
