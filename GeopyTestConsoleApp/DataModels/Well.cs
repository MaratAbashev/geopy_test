using System;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace GeopyTestConsoleApp.DataModels
{
    [Table(Schema="public", Name="wells")]
    public class Well
    {
        [Column("id")]
        [PrimaryKey]
        [Identity]
        public int Id { get; set; } // integer
        
        [Column("name")]
        [NotNull] 
        public string Name { get; set; } // character varying(200)
        
        [Column("commissioning_date")]
        [NotNull] 
        public DateTime CommissioningDate { get; set; } // timestamp (6) without time zone
        
        [Column("division_id")]
        [NotNull] 
        public int DivisionId { get; set; } // integer

        #region Associations

        /// <summary>
        /// wells_division_id_fkey (public.divisions)
        /// </summary>
        [Association(ThisKey=nameof(DivisionId), OtherKey=nameof(global::GeopyTestConsoleApp.DataModels.Division.Id), CanBeNull=false)]
        public Division Division { get; set; }

        /// <summary>
        /// measurements_well_id_fkey_BackReference (public.measurements)
        /// </summary>
        [Association(ThisKey=nameof(Id), OtherKey=nameof(Measurement.WellId), CanBeNull=true)]
        public IEnumerable<Measurement> Measurements { get; set; }

        #endregion
    }
}