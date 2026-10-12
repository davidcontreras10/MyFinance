using System;
using System.Collections.Generic;

namespace Domain.Models
{
	public class BccrVentanillaModel
	{
		#region Attributes

		public string EntityName { get; set; }
		public float Purchase { get; set; }
		public float Sell { get; set; }
		public DateTime LastUpdate { get; set; }

		#endregion
	}


	public record class BccrSingleVentanillaModelResponse(
		IEnumerable<BccrSingleVentanillaModel> BccrSingleVentanillaModels,
		DateTime RequestedDate,
		DateTime InitialReqDate
	)
	{
        public DateTime EndReqDate { get; set; }
    }

	public class BccrSingleVentanillaModel
	{
		public string EntityName { get; set; }
		public float Value { get; set; }
		public DateTime LastUpdate { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = (BccrSingleVentanillaModel)obj;
			return EntityName == other.EntityName && Value == other.Value && LastUpdate == other.LastUpdate;
		}

		//override the == and != operator to use same logic as Equals and alos the getHashCode
		public static bool operator ==(BccrSingleVentanillaModel a, BccrSingleVentanillaModel b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if (a is null || b is null)
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(BccrSingleVentanillaModel a, BccrSingleVentanillaModel b)
		{
			return !(a == b);
		}


		public override int GetHashCode()
		{
			return HashCode.Combine(EntityName, Value, LastUpdate);
		}
	}

}