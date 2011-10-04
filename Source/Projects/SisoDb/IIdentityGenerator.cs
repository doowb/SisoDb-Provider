﻿using PineCone.Structures.Schemas;

namespace SisoDb
{
    public interface IIdentityGenerator
    {
        int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds);
    }
}