﻿using PineCone.Structures.Schemas;

namespace SisoDb.Providers
{
    public static class SqlStructureSchemaExtensions
    {
        public static string GetStructureTableName(this IStructureSchema structureSchema)
        {
            return structureSchema.Name + "Structure";
        }

        public static string GetIndexesTableName(this IStructureSchema structureSchema)
        {
            return structureSchema.Name + "Indexes";
        }

        public static string GetUniquesTableName(this IStructureSchema structureSchema)
        {
            return structureSchema.Name + "Uniques";
        }
    }
}