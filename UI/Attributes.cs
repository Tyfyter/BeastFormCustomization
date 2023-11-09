using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BeastCustomization.UI {
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class OldHairDyeFieldAttribute : Attribute {
		readonly string fieldName;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName">the name of the field the hair dye was formerly stored in</param>
		public OldHairDyeFieldAttribute(string fieldName) {
			this.fieldName = fieldName;
		}
		public string FieldName => fieldName;
	}
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class OldOverrideShaderFieldAttribute : Attribute {
		readonly string fieldName;
		readonly bool invert;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName">the name of the field the value was formerly stored in</param>
		/// <param name="fieldName">whether the resulting value should be inverted</param>
		public OldOverrideShaderFieldAttribute(string fieldName, bool invert = false) {
			this.fieldName = fieldName;
			this.invert = invert;
		}
		public string FieldName => fieldName;
		public bool Invert => invert;
	}
}
