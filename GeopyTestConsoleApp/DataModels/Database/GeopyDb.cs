using LinqToDB;

namespace GeopyTestConsoleApp.DataModels.Database
{
    public partial class GeopyDb : LinqToDB.Data.DataConnection
    {
        #region Tables

        public ITable<Division> Divisions => this.GetTable<Division>();
        public ITable<Measurement> Measurements => this.GetTable<Measurement>();
        public ITable<Well> Wells => this.GetTable<Well>();

        #endregion

        partial void InitMappingSchema()
        {
            MappingSchema.SetConverter<int, MeasurementType>(num => (MeasurementType)num);
            MappingSchema.SetConverter<MeasurementType, int>(type => (int)type);
        }

        #region .ctor

        public GeopyDb()
        {
            InitDataContext();
            InitMappingSchema();
        }

        public GeopyDb(string configuration)
            : base(configuration)
        {
            InitDataContext();
            InitMappingSchema();
        }

        public GeopyDb(DataOptions options)
            : base(options)
        {
            InitDataContext();
            InitMappingSchema();
        }

        public GeopyDb(DataOptions<GeopyDb> options)
            : base(options.Options)
        {
            InitDataContext();
            InitMappingSchema();
        }

        partial void InitDataContext();
        partial void InitMappingSchema();

        #endregion
    }
}