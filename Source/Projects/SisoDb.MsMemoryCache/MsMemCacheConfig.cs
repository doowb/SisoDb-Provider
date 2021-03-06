using System;
using System.Runtime.Caching;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures;

namespace SisoDb.MsMemoryCache
{
    public class MsMemCacheConfig
    {
        public const int DefaultSlidingExpirationSeconds = 3600;

        public Type StructureType { get; private set; }
        public DateTimeOffset AbsoluteExpiration { get; private set; }
        public TimeSpan SlidingExpiration { get; private set; }
        public string CacheEntryKeyPrefix { get; private set; }
        public Func<IStructureId, string> CacheEntryKeyGenerator { get; private set; }

        private MsMemCacheConfig(Type structureType, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            StructureType = structureType;
            CacheEntryKeyPrefix = string.Concat(structureType.Name, ":");
            CacheEntryKeyGenerator = id => string.Concat(CacheEntryKeyPrefix, id.Value.ToString());

            AbsoluteExpiration = absoluteExpiration;
            SlidingExpiration = slidingExpiration;
        }

        public static MsMemCacheConfig CreateAbsolute(Type structureType, TimeSpan expiresIn)
        {
            return new MsMemCacheConfig(structureType, DateTime.UtcNow.Add(expiresIn), ObjectCache.NoSlidingExpiration);
        }

        public static MsMemCacheConfig CreateSliding(Type structureType)
        {
            return new MsMemCacheConfig(structureType, ObjectCache.InfiniteAbsoluteExpiration, TimeSpan.FromSeconds(DefaultSlidingExpirationSeconds));
        }

        public static MsMemCacheConfig CreateSliding(Type structureType, TimeSpan expiresIn)
        {
            return new MsMemCacheConfig(structureType, ObjectCache.InfiniteAbsoluteExpiration, expiresIn);
        }
    }
}