using System;
using LinqToDB.Mapping;

namespace GeopyTestConsoleApp.DataModels
{
	[Table(Schema="public", Name="measurements")]
	public class Measurement
	{
		[Column("id")]
		[PrimaryKey]
		[Identity] 
		public int Id { get; set; } // integer
		
		[Column("measurement_time"),  NotNull             ] 
		public DateTime MeasurementTime { get; set; } // timestamp (6) without time zone
		
		[Column("well_id")]
		[NotNull] 
		public int WellId { get; set; } // integer
		
		[Column("measurement_value")]
		[NotNull] 
		public decimal MeasurementValue { get; set; } // numeric(10,4)
		
		[Column("measurement_type")]
		[NotNull] 
		public MeasurementType MeasurementType { get; set; } // integer

		#region Associations

		/// <summary>
		/// measurements_well_id_fkey (public.wells)
		/// </summary>
		[Association(ThisKey=nameof(WellId), OtherKey=nameof(GeopyTestConsoleApp.DataModels.Well.Id), CanBeNull=false)]
		public Well Well { get; set; }

		#endregion
	}
}