// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.11.3
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace digital.thinkport
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.11.3")]
	public partial class Wishlist : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""Wishlist"",""namespace"":""digital.thinkport"",""fields"":[{""name"":""recipient"",""type"":""string""},{""name"":""age"",""type"":""int""},{""name"":""address"",""type"":""string""},{""name"":""presents"",""type"":{""type"":""array"",""items"":""string""}},{""name"":""price"",""type"":""double""}]}");
		private string _recipient;
		private int _age;
		private string _address;
		private IList<System.String> _presents;
		private double _price;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return Wishlist._SCHEMA;
			}
		}
		public string recipient
		{
			get
			{
				return this._recipient;
			}
			set
			{
				this._recipient = value;
			}
		}
		public int age
		{
			get
			{
				return this._age;
			}
			set
			{
				this._age = value;
			}
		}
		public string address
		{
			get
			{
				return this._address;
			}
			set
			{
				this._address = value;
			}
		}
		public IList<System.String> presents
		{
			get
			{
				return this._presents;
			}
			set
			{
				this._presents = value;
			}
		}
		public double price
		{
			get
			{
				return this._price;
			}
			set
			{
				this._price = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.recipient;
			case 1: return this.age;
			case 2: return this.address;
			case 3: return this.presents;
			case 4: return this.price;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.recipient = (System.String)fieldValue; break;
			case 1: this.age = (System.Int32)fieldValue; break;
			case 2: this.address = (System.String)fieldValue; break;
			case 3: this.presents = (IList<System.String>)fieldValue; break;
			case 4: this.price = (System.Double)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
