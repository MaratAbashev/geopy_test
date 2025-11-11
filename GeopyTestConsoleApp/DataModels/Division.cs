using System.Collections.Generic;
using LinqToDB.Mapping;

namespace GeopyTestConsoleApp.DataModels
{
    [Table(Schema="public", Name="divisions")]
    public class Division
    {
        [Column("id")]
        [PrimaryKey] 
        [Identity]
        public int Id { get; set; } // integer
        
        [Column("name")]
        [NotNull]
        public string Name { get; set; } // character varying(200)

        #region Associations

        /// <summary>
        /// wells_division_id_fkey_BackReference (public.wells)
        /// </summary>
        [Association(ThisKey=nameof(Id), OtherKey=nameof(Well.DivisionId), CanBeNull=true)]
        public IEnumerable<Well> Wellsdivisionidfkeys { get; set; }

        #endregion
    }
}