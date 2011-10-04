using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface ISqlDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}