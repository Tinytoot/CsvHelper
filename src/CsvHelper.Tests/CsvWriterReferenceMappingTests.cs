﻿// Copyright 2009-2013 Josh Close
// This file is a part of CsvHelper and is licensed under the MS-PL
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html
// http://csvhelper.com
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvHelper.Tests
{
	[TestClass]
	public class CsvWriterReferenceMappingTests
	{
		[TestMethod]
		public void NestedReferencesTest()
		{
			using( var stream = new MemoryStream() )
			using( var reader = new StreamReader( stream ) )
			using( var writer = new StreamWriter( stream ) )
			using( var csv = new CsvWriter( writer ) )
			{
				csv.Configuration.RegisterClassMap<AMap>();

				var list = new List<A>();
				for( var i = 0; i < 4; i++ )
				{
					var row = i + 1;
					list.Add(
					         new A
					         {
						         Id = "a" + row,
						         B = new B
						         {
							         Id = "b" + row,
							         C = new C
							         {
								         Id = "c" + row,
								         D = new D
								         {
									         Id = "d" + row
								         }
							         }
						         }
					         } );
				};

				csv.WriteRecords( list );
				writer.Flush();
				stream.Position = 0;

				var data = reader.ReadToEnd();

				var expected = new StringBuilder();
				expected.AppendLine( "AId,BId,CId,DId" );
				expected.AppendLine( "a1,b1,c1,d1" );
				expected.AppendLine( "a2,b2,c2,d2" );
				expected.AppendLine( "a3,b3,c3,d3" );
				expected.AppendLine( "a4,b4,c4,d4" );
				Assert.AreEqual( expected.ToString(), data );
			}
		}

		private class A
		{
			public string Id { get; set; }

			public B B { get; set; }
		}

		private class B
		{
			public string Id { get; set; }

			public C C { get; set; }
		}

		private class C
		{
			public string Id { get; set; }

			public D D { get; set; }
		}

		private class D
		{
			public string Id { get; set; }
		}

		private sealed class AMap : CsvClassMap<A>
		{
			public override void CreateMap()
			{
				Map( m => m.Id ).Name( "AId" );
				References<BMap>( m => m.B );
			}
		}

		private sealed class BMap : CsvClassMap<B>
		{
			public override void CreateMap()
			{
				Map( m => m.Id ).Name( "BId" );
				References<CMap>( m => m.C );
			}
		}

		private sealed class CMap : CsvClassMap<C>
		{
			public override void CreateMap()
			{
				Map( m => m.Id ).Name( "CId" );
				References<DMap>( m => m.D );
			}
		}

		private sealed class DMap : CsvClassMap<D>
		{
			public override void CreateMap()
			{
				Map( m => m.Id ).Name( "DId" );
			}
		}
	}
}
